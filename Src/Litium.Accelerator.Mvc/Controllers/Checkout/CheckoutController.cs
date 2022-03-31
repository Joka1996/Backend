using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Threading.Tasks;
using Litium.Accelerator.Builders.Checkout;
using Litium.Accelerator.Constants;
using Litium.Accelerator.Routing;
using Litium.Accelerator.ValidationRules;
using Litium.FieldFramework.FieldTypes;
using Litium.Globalization;
using Litium.Runtime.AutoMapper;
using Litium.Sales;
using Litium.Sales.Payments.PaymentFlowActions;
using Litium.Web;
using Litium.Web.Models;
using Litium.Web.Models.Websites;
using Litium.Websites;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Litium.Accelerator.Mvc.Controllers.Checkout
{
    public class CheckoutController : ControllerBase
    {
        private readonly CheckoutViewModelBuilder _checkoutViewModelBuilder;
        private readonly RequestModelAccessor _requestModelAccessor;
        private readonly PageService _pageService;
        private readonly UrlService _urlService;
        private readonly CountryService _countryService;
        private readonly Sales.PaymentService _paymentService;
        private readonly PaymentProviderService _paymentProviderService;
        private readonly ChannelService _channelService;
        private readonly WebsiteService _websiteService;
        private readonly ILogger<CheckoutController> _logger;

        public CheckoutController(
            CheckoutViewModelBuilder checkoutViewModelBuilder,
            RequestModelAccessor requestModelAccessor,
            UrlService urlService,
            PageService pageService,
            CountryService countryService,
            Sales.PaymentService paymentService,
            PaymentProviderService paymentProviderService,
            ChannelService channelService,
            WebsiteService websiteService,
            ILogger<CheckoutController> logger)
        {
            _checkoutViewModelBuilder = checkoutViewModelBuilder;
            _requestModelAccessor = requestModelAccessor;
            _urlService = urlService;
            _pageService = pageService;
            _countryService = countryService;
            _paymentService = paymentService;
            _paymentProviderService = paymentProviderService;
            _channelService = channelService;
            _websiteService = websiteService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult> Index(bool AddressUpdated)
        {
            var cartContext = HttpContext.GetCartContext();

            var updateAddressAction = cartContext.PaymentFlowResults?
                .Select(r => r.PaymentFlowAction)
                .FirstOrDefault() as UpdateAddress;
            if (updateAddressAction is not null && updateAddressAction.CountryCode != cartContext.CountryCode)
            {
                await cartContext.SelectCountryAsync(new SelectCountryArgs { CountryCode = updateAddressAction.CountryCode });
            }

            await cartContext.TryInitializeCheckoutFlowAsync(() => new CheckoutFlowInfoArgs
            {
                CheckoutFlowInfo = GetCheckoutFlowInfo()
            });

            var model = _checkoutViewModelBuilder.Build(cartContext);
            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> PlaceOrderDirect()
        {
            var cartContext = HttpContext.GetCartContext();

            try
            {
                await cartContext.ValidateAsync();
            }
            catch (CartContextValidationException ex)
            {
                await SetDefaultPaymentMethod(cartContext);
                var model = _checkoutViewModelBuilder.Build(cartContext);
                model.ErrorMessages.Add("general", new List<string> { ex.Message });
                return View("Index", model);
            }

            try
            {
                await cartContext.ConfirmOrderAsync();
                return Redirect(GetOrderConfirmationPageUrl(cartContext.Cart.Order));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when placing an order");
                await SetDefaultPaymentMethod(cartContext);
                var model = _checkoutViewModelBuilder.Build(cartContext);
                model.ErrorMessages.Add("general", new List<string> { "checkout.generalerror".AsWebsiteText() });
                return View("Index", model);
            }
        }

        private string GetAbsolutePageUrl(PointerPageItem pointer)
        {
            if (pointer == null)
            {
                return null;
            }
            var page = _pageService.Get(pointer.EntitySystemId);
            if (page == null)
            {
                return null;
            }

            var channelSystemId = pointer.ChannelSystemId != Guid.Empty ? pointer.ChannelSystemId : _requestModelAccessor.RequestModel.ChannelModel.SystemId;
            return _urlService.GetUrl(page, new PageUrlArgs(channelSystemId) { AbsoluteUrl = true });
        }

        public virtual async Task SetDefaultPaymentMethod(CartContext cartContext)
        {
            var country = _countryService.Get(cartContext?.CountryCode);
            var paymentOption = _requestModelAccessor.RequestModel.ChannelModel?.Channel?.CountryLinks?.FirstOrDefault(x => x.CountrySystemId == country?.SystemId)?.PaymentOptions.FirstOrDefault();
            if (paymentOption != null)
            {
                var paymentProvider = _paymentProviderService.Get(paymentOption.Id.ProviderId);
                var orderPaymentLink = _requestModelAccessor.RequestModel.Cart.Order.OrderPaymentLinks.FirstOrDefault();
                if (orderPaymentLink != null)
                {
                    var payment = _paymentService.Get(orderPaymentLink.PaymentSystemId);
                    // Set default payment option
                    if (payment != null && payment.PaymentOption.ProviderId != paymentProvider.Id && payment.PaymentOption.OptionId != paymentOption.Id.OptionId)
                    {
                        var selectPaymentArgs = new SelectPaymentOptionArgs
                        {
                            PaymentOptionId = paymentOption.Id
                        };
                        await cartContext.SelectPaymentOptionAsync(selectPaymentArgs);
                    }
                }
            }
        }

        private CheckoutFlowInfo GetCheckoutFlowInfo()
        {
            var checkoutPage = _requestModelAccessor.RequestModel.WebsiteModel.GetValue<PointerPageItem>(AcceleratorWebsiteFieldNameConstants.CheckoutPage);
            var checkoutPageUrl = GetAbsolutePageUrl(checkoutPage);

            return new CheckoutFlowInfo
            {
                CheckoutPageUrl = checkoutPageUrl,
                TermsUrl = GetAbsolutePageUrl(checkoutPage?.EntitySystemId.MapTo<PageModel>().GetValue<PointerPageItem>(CheckoutPageFieldNameConstants.TermsAndConditionsPage)),
                ReceiptPageUrl = GetAbsolutePageUrl(_requestModelAccessor.RequestModel.WebsiteModel.GetValue<PointerPageItem>(AcceleratorWebsiteFieldNameConstants.OrderConfirmationPage)),
                CancelPageUrl = checkoutPageUrl
            };
        }

        private string GetOrderConfirmationPageUrl(SalesOrder order)
        {
            var channel = _channelService.Get(order.ChannelSystemId.GetValueOrDefault());
            var website = _websiteService.Get(channel.WebsiteSystemId.GetValueOrDefault());
            var pointerPage = website.Fields.GetValue<PointerPageItem>(AcceleratorWebsiteFieldNameConstants.OrderConfirmationPage);

            if (pointerPage == null)
            {
                throw new CheckoutException("Order is created, order confirmation page is missing.");
            }

            var channelSystemId = pointerPage.ChannelSystemId != Guid.Empty ? pointerPage.ChannelSystemId : order.ChannelSystemId.Value;
            var url = _urlService.GetUrl(_pageService.Get(pointerPage.EntitySystemId), new PageUrlArgs(channelSystemId));

            if (string.IsNullOrEmpty(url))
            {
                throw new CheckoutException("Order is created, order confirmation page is missing.");
            }

            return $"{url}?orderId={order.SystemId}";
        }
    }
}
