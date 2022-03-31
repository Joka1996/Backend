using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Litium.Accelerator.Routing;
using Litium.Accelerator.ViewModels.Checkout;
using Litium.Globalization;

namespace Litium.Accelerator.Builders.Checkout
{
    public class DeliveryMethodViewModelBuilder : IViewModelBuilder<DeliveryMethodViewModel>
    {
        private readonly RequestModelAccessor _requestModelAccessor;
        private readonly CurrencyService _currencyService;
        private readonly CountryService _countryService;
        private readonly ChannelService _channelService;

        public DeliveryMethodViewModelBuilder(
            RequestModelAccessor requestModelAccessor,
            CurrencyService currencyService,
            CountryService countryService,
            ChannelService channelService)
        {
            _requestModelAccessor = requestModelAccessor;
            _currencyService = currencyService;
            _countryService = countryService;
            _channelService = channelService;
        }

        public virtual List<DeliveryMethodViewModel> Build(string countryCode, string currencyCode)
        {
            var currency = _currencyService.Get(currencyCode);
            if (currency == null)
            {
                return new List<DeliveryMethodViewModel>();
            }
            var countrySystemId = _countryService.Get(countryCode)?.SystemId;
            var channelSystemId = _requestModelAccessor.RequestModel.ChannelModel?.Channel?.SystemId;

            List<DeliveryMethodViewModel> deliveryMethods = new List<DeliveryMethodViewModel>();
            var channel = _channelService.Get(channelSystemId.HasValue ? channelSystemId.Value : Guid.Empty);
            foreach (var shippingOption in channel?.CountryLinks?.Where(x => x.CountrySystemId == countrySystemId)?.SelectMany(x => x.ShippingOptions))
            {
                var fee = shippingOption.Fee ?? 0m;
                deliveryMethods.Add(new DeliveryMethodViewModel
                {
                   Id = shippingOption.Id,
                   FormattedPrice = currency.Format(shippingOption.Fee ?? 0, true, CultureInfo.CurrentCulture),
                   Name = string.IsNullOrWhiteSpace(shippingOption.Name) ? shippingOption.Id.ToString() : shippingOption.Name,
                   Price = shippingOption.Fee
                });
            }

            return deliveryMethods;
        }
    }
}
