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
    internal class CartItemsResolver : IFieldResolver<IEnumerable<CartItemModel>>
    {
        private readonly CartContextAccessor _cartContextAccessor;
        private readonly CurrencyService _currencyService;

        public CartItemsResolver(
            CartContextAccessor cartContextAccessor,
            CurrencyService currencyService)
        {
            _cartContextAccessor = cartContextAccessor;
            _currencyService = currencyService;
        }

        public Task<IEnumerable<CartItemModel>> ResolveAsync(IResolveFieldContext context)
        {
            var model = Enumerable.Empty<CartItemModel>();
            var cartContext = _cartContextAccessor.CartContext;
            if (cartContext?.Cart is Sales.Cart cart && cart.Order is SalesOrder salesOrder)
            {
                var currency = _currencyService.Get(salesOrder.CurrencyCode);
                model = salesOrder.Rows.Select(x =>
                    {
                        return new CartItemModel
                        {
                            ArticleNumber = x.ArticleNumber,
                            Description = x.Description,
                            FormattedTotalPrice = currency.Format(x.TotalIncludingVat, false, CultureInfo.CurrentUICulture),
                            FormattedUnitPrice = currency.Format(x.UnitPriceIncludingVat, false, CultureInfo.CurrentUICulture),
                            Id = x.Id,
                            Quantity = x.Quantity,
                            SystemGenerated = x.SystemGenerated,
                            TotalPrice = x.TotalIncludingVat,
                            UnitPrice = x.UnitPriceIncludingVat,
                            VatAmount = x.TotalVat,
                            VatRate = x.VatRate,
                        };
                    })
                    .ToList();
            }

            return Task.FromResult(model);
        }
    }
}
