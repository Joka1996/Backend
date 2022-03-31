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
    internal class CartResolver : IFieldResolver<Models.Carts.CartModel>
    {
        private readonly CartContextAccessor _cartContextAccessor;
        private readonly CurrencyService _currencyService;

        public CartResolver(
            CartContextAccessor cartContextAccessor,
            CurrencyService currencyService)
        {
            _cartContextAccessor = cartContextAccessor;
            _currencyService = currencyService;
        }

        public Task<Models.Carts.CartModel> ResolveAsync(IResolveFieldContext context)
        {
            var cartContext = _cartContextAccessor.CartContext;
            if (cartContext?.Cart is Sales.Cart cart && cart.Order is SalesOrder salesOrder)
            {
                var currency = _currencyService.Get(salesOrder.CurrencyCode);
                var model = new Models.Carts.CartModel()
                {
                    FormattedTotalPrice = currency.Format(salesOrder.GrandTotal, false, CultureInfo.CurrentUICulture),
                    PaymentOptionId = cart.PaymentOverviews.FirstOrDefault()?.Payment.PaymentOption.ToString(),
                    ShippingOptionId = salesOrder.ShippingInfo.FirstOrDefault()?.ShippingOption.ToString(),
                    TotalPrice = salesOrder.GrandTotal,
                    VatAmount = salesOrder.TotalVat,
                };

                return Task.FromResult(model);
            }
            return Task.FromResult<Models.Carts.CartModel>(default);
        }
    }
}
