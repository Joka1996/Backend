using System.Linq;
using System.Threading.Tasks;
using GraphQL;
using Litium.Accelerator.GraphQL.Models.Carts;
using Litium.Runtime.DependencyInjection;
using Litium.Sales;
using Litium.Sales.Payments.PaymentFlowActions;
using Litium.Web.GraphQL;

namespace Litium.Accelerator.GraphQL.Queries.Carts
{
    [Service]
    internal class PaymnetWidgetResolver : IFieldResolver<PaymentWidgetModel>
    {
        private readonly CartContextAccessor _cartContextAccessor;
        private readonly PaymentService _paymentService;

        public PaymnetWidgetResolver(
            CartContextAccessor cartContextAccessor,
            PaymentService paymentService)
        {
            _cartContextAccessor = cartContextAccessor;
            _paymentService = paymentService;
        }

        public Task<PaymentWidgetModel> ResolveAsync(IResolveFieldContext context)
        {
            var model = default(PaymentWidgetModel);
            var cartContext = _cartContextAccessor.CartContext;
            if (cartContext.PaymentFlowResults.Count > 0
                && cartContext.Cart.Order.OrderPaymentLinks.Count > 0)
            {
                foreach (var paymentLink in cartContext.Cart.Order.OrderPaymentLinks)
                {
                    var payment = _paymentService.Get(paymentLink.PaymentSystemId);
                    var paymentFlowResult = cartContext.PaymentFlowResults.SingleOrDefault(x => x.PaymentSystemId == payment.SystemId
                                                                                                && x.PaymentFlowOperation == Sales.Payments.PaymentFlowOperation.InitializePayment
                                                                                                && x.Success);
                    if (paymentFlowResult != null && paymentFlowResult.PaymentFlowAction is ShowHtmlSnippet paymentFlowAction)
                    {
                        model = new()
                        {
                            ResponseString = paymentFlowAction.HtmlSnippet
                        };
                        break;
                    }
                }
            }

            return Task.FromResult(model);
        }
    }
}
