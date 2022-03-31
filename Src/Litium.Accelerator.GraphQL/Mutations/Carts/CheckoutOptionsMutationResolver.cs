using System;
using System.Threading.Tasks;
using GraphQL;
using Litium.Accelerator.Constants;
using Litium.Accelerator.GraphQL.Models.Carts;
using Litium.FieldFramework.FieldTypes;
using Litium.Globalization;
using Litium.Sales;
using Litium.Web;
using Litium.Web.GraphQL;
using Litium.Web.GraphQL.MutationTypes;
using Litium.Websites;

namespace Litium.Accelerator.GraphQL.Mutations.Carts
{
    internal class CheckoutOptionsMutationResolver : IFieldResolver<MutationResponse>
    {
        private readonly CartContextAccessor _cartContextAccessor;
        private readonly WebsiteService _websiteService;
        private readonly ChannelService _channelService;
        private readonly PageService _pageService;
        private readonly UrlService _urlService;

        public CheckoutOptionsMutationResolver(
            CartContextAccessor cartContextAccessor,
            WebsiteService websiteService,
            ChannelService channelService,
            PageService pageService,
            UrlService urlService)
        {
            _cartContextAccessor = cartContextAccessor;
            _websiteService = websiteService;
            _channelService = channelService;
            _pageService = pageService;
            _urlService = urlService;
        }

        public async Task<MutationResponse> ResolveAsync(IResolveFieldContext context)
        {
            var cartContext = _cartContextAccessor.CartContext;
            var arg = context.Arguments["item"].Value as CheckoutOptionsModel;

            await cartContext.TryInitializeCheckoutFlowAsync(() =>
                new()
                {
                    CheckoutFlowInfo = GetCheckoutFlowInfo()
                },
                context.CancellationToken);

            if (!string.IsNullOrEmpty(arg.PaymentOptionId))
            {
                try
                {
                    await cartContext.SelectPaymentOptionAsync(new()
                    {
                        PaymentOptionId = (ProviderOptionIdentifier)arg.PaymentOptionId,
                    },
                    context.CancellationToken);
                }
                catch (Exception e)
                {
                    context.Errors.Add(new ExecutionError("Could not change payment option.", e));
                    return new()
                    {
                        Result = new()
                        {
                            Success = false,
                            Message = e.Message,
                        }
                    };
                }
            }

            if (!string.IsNullOrEmpty(arg.ShippingOptionId))
            {
                try
                {
                    await cartContext.SelectShippingOptionAsync(new()
                    {
                        ShippingOptionId = (ProviderOptionIdentifier)arg.ShippingOptionId,
                    },
                    context.CancellationToken);
                }
                catch (Exception e)
                {
                    context.Errors.Add(new ExecutionError("Could not change shipment option.", e));
                    return new()
                    {
                        Result = new()
                        {
                            Success = false,
                            Message = e.Message,
                        }
                    };
                }
            }

            return new()
            {
                Result = new()
                {
                    Success = true,
                },
            };

            CheckoutFlowInfo GetCheckoutFlowInfo()
            {
                var channel = _channelService.Get(cartContext.ChannelSystemId.GetValueOrDefault());
                if (channel is null)
                {
                    return default;
                }

                var website = _websiteService.Get(channel.WebsiteSystemId.GetValueOrDefault());
                if (website is null)
                {
                    return default;
                }

                var checkoutPagePointer = website.Fields.GetValue<PointerPageItem>(AcceleratorWebsiteFieldNameConstants.CheckoutPage);
                var checkoutPage = _pageService.Get(website.Fields.GetValue<PointerPageItem>(AcceleratorWebsiteFieldNameConstants.CheckoutPage)?.EntitySystemId ?? Guid.Empty);
                var checkoutPageUrl = GetAbsolutePageUrl(checkoutPagePointer);

                return new CheckoutFlowInfo
                {
                    CheckoutPageUrl = checkoutPageUrl,
                    TermsUrl = GetAbsolutePageUrl(checkoutPage.Fields.GetValue<PointerPageItem>(CheckoutPageFieldNameConstants.TermsAndConditionsPage)),
                    ReceiptPageUrl = GetAbsolutePageUrl(website.Fields.GetValue<PointerPageItem>(AcceleratorWebsiteFieldNameConstants.OrderConfirmationPage)),
                    CancelPageUrl = checkoutPageUrl
                };

                string GetAbsolutePageUrl(PointerPageItem pointer)
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

                    var channelSystemId = pointer.ChannelSystemId != Guid.Empty ? pointer.ChannelSystemId : channel.SystemId;
                    return _urlService.GetUrl(page, new PageUrlArgs(channelSystemId) { AbsoluteUrl = true });
                }
            }
        }
    }
}
