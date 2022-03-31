using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using GraphQL;
using Litium.Accelerator.GraphQL.Models.Carts;
using Litium.Globalization;
using Litium.Runtime.DependencyInjection;
using Litium.Sales;
using Litium.Web.GraphQL;

namespace Litium.Accelerator.GraphQL.Queries.Carts
{
    [Service]
    internal class SupportedShipmentOptionsResolver : IFieldResolver<IEnumerable<OptionItemModel>>
    {
        private readonly CartContextAccessor _cartContextAccessor;
        private readonly CurrencyService _currencyService;
        private readonly CountryService _countryService;
        private readonly ChannelService _channelService;

        public SupportedShipmentOptionsResolver(
            CartContextAccessor cartContextAccessor,
            CurrencyService currencyService,
            CountryService countryService,
            ChannelService channelService)
        {
            _cartContextAccessor = cartContextAccessor;
            _currencyService = currencyService;
            _countryService = countryService;
            _channelService = channelService;
        }

        public Task<IEnumerable<OptionItemModel>> ResolveAsync(IResolveFieldContext context)
        {
            var model = Enumerable.Empty<OptionItemModel>();
            var cartContext = _cartContextAccessor.CartContext;
            if (cartContext?.Cart is Sales.Cart cart && cart.Order is SalesOrder salesOrder)
            {
                var currency = _currencyService.Get(salesOrder.CurrencyCode);
                var channel = _channelService.Get(cartContext.ChannelSystemId.GetValueOrDefault());
                var countrySystemId = _countryService.Get(cartContext.CountryCode)?.SystemId;

                model = channel?.CountryLinks?
                    .Where(x => x.CountrySystemId == countrySystemId)?
                    .SelectMany(x => x.ShippingOptions)
                    .Select(x =>
                    {
                        return new OptionItemModel
                        {
                            Id = x.Id,
                            Description = x.Description,
                            FormattedPrice = currency.Format(x.Fee ?? 0m, true, CultureInfo.CurrentUICulture),
                            Name = string.IsNullOrWhiteSpace(x.Name) ? x.Id.ToString() : x.Name,
                            Price = x.Fee,
                        };
                    })
                    .ToList();
            }

            return Task.FromResult(model);
        }
    }
}
