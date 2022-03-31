﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Litium.Accelerator.Payments;
using Litium.Accelerator.ViewModels.Checkout;
using Litium.Globalization;
using Litium.Sales;
using Litium.Sales.Payments.PaymentFlowActions;

namespace Litium.Accelerator.Builders.Checkout
{
    public class PaymentOptionViewModelBuilder : IViewModelBuilder<PaymentOptionViewModel>
    {
        private readonly CurrencyService _currencyService;
        private readonly CountryService _countryService;
        private readonly ChannelService _channelService;
        private readonly PaymentService _paymentService;


        public PaymentOptionViewModelBuilder(
            CurrencyService currencyService,
            CountryService countryService,
            ChannelService channelService,
            PaymentService paymentService)
        {
            _currencyService = currencyService;
            _countryService = countryService;
            _channelService = channelService;
            _paymentService = paymentService;
        }

        public virtual List<PaymentOptionViewModel> Build(CartContext cartContext)
        {
            var paymentOptions = new List<PaymentOptionViewModel>();
            if (cartContext != null)
            {
                var channel = _channelService.Get(cartContext.ChannelSystemId.GetValueOrDefault());
                var countrySystemId = _countryService.Get(cartContext.CountryCode)?.SystemId;
                var currency = _currencyService.Get(cartContext.CurrencyCode);

                foreach (var paymentOption in channel?.CountryLinks?.Where(x => x.CountrySystemId == countrySystemId)?.SelectMany(x => x.PaymentOptions))
                {
                    paymentOptions.Add(new PaymentOptionViewModel
                    {
                        Id = paymentOption.Id,
                        FormattedPrice = currency?.Format(paymentOption.Fee ?? 0m, true, CultureInfo.CurrentCulture),
                        Name = string.IsNullOrWhiteSpace(paymentOption.Name) ? paymentOption.Id.ToString() : paymentOption.Name,
                        Price = paymentOption.Fee
                    });
                }
            }
            return paymentOptions;
        }

        public virtual PaymentWidgetResult BuildWidget(CartContext cartContext, ProviderOptionIdentifier paymentOptionIdentifier)
        {
            if (cartContext == null || !cartContext.PaymentFlowResults.Any() || !cartContext.Cart.Order.OrderPaymentLinks.Any())
            {
                return null;
            }

            foreach (var paymentLink in cartContext.Cart.Order.OrderPaymentLinks)
            {
                var payment = _paymentService.Get(paymentLink.PaymentSystemId);
                if (payment.PaymentOption.ProviderId == paymentOptionIdentifier.ProviderId && payment.PaymentOption.OptionId == paymentOptionIdentifier.OptionId)
                {
                    var paymentFlowResult = cartContext.PaymentFlowResults.SingleOrDefault(x => x.PaymentSystemId == payment.SystemId
                                                                                                && x.PaymentFlowOperation == Sales.Payments.PaymentFlowOperation.InitializePayment
                                                                                                && x.Success);
                    if (paymentFlowResult != null && paymentFlowResult.PaymentFlowAction is ShowHtmlSnippet paymentFlowAction)
                    {
                        return new PaymentWidgetResult
                        {
                            Id = paymentOptionIdentifier,
                            ResponseString = paymentFlowAction.HtmlSnippet
                        };
                    }
                    break;
                }
            }

            return null;
        }
    }
}
