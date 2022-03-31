using Litium.Sales;
using Litium.Validations;
using Litium.StateTransitions;
using System.Linq;

namespace Litium.Accelerator.StateTransitions 
{
    public class ProcessingToCompletedCondition : StateTransitionValidationRule<SalesOrder>
    {
        private readonly OrderOverviewService _orderOverviewService;
        private readonly StateTransitionsService _stateTransitionsService;

        public ProcessingToCompletedCondition(OrderOverviewService orderOverviewService, StateTransitionsService stateTransitionsService)
        {
            _orderOverviewService = orderOverviewService;
            _stateTransitionsService = stateTransitionsService;
        }

        public override string FromState => Sales.OrderState.Processing;

        public override string ToState => Sales.OrderState.Completed;

        public override ValidationResult Validate(SalesOrder entity)
        {
            var result = new ValidationResult();
            var order = _orderOverviewService.Get(entity.SystemId);

            //order can only be completed if shipments shipped.
            var hasAllShipmentShipped = HasAllShipmentsShipped(order);
            if(!hasAllShipmentShipped)
            {
                result.AddError("Shipment", "All shipments are not shipped.");
            }

            return result;
        }

        private bool HasAllShipmentsShipped(OrderOverview order)
        {
            var orderCountPerArticle = order.SalesOrder.Rows.Where(x => x.OrderRowType == Sales.OrderRowType.Product)
                                                            .GroupBy(r => r.ArticleNumber).ToDictionary(g => g.Key, g => g.Sum(t => t.Quantity));
            var shippedCountPerArticle = order.Shipments.Where(x => _stateTransitionsService.GetState<Sales.Shipment>(x.SystemId) == ShipmentState.Shipped)
                                                        .SelectMany(x => x.Rows.Where(r => r.OrderRowType == Sales.OrderRowType.Product))
                                                        .GroupBy(r => r.ArticleNumber).ToDictionary(g => g.Key, g => g.Sum(t => t.Quantity));

            return shippedCountPerArticle.Sum(x => x.Value) == orderCountPerArticle.Sum(x => x.Value)
                   && orderCountPerArticle.All(x => shippedCountPerArticle.TryGetValue(x.Key, out var value) && value == x.Value);
        }
    }
}
