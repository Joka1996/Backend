using System;
using System.Linq;
using Litium.Sales;
using Litium.StateTransitions;
using Litium.Validations;

namespace Litium.Accelerator.StateTransitions.Shipment
{
    public class ProcessingToReadyToShipCondition : StateTransitionValidationRule<Sales.Shipment>
    {
        private readonly OrderOverviewService _orderOverviewService;

        public ProcessingToReadyToShipCondition(OrderOverviewService orderOverviewService)
        {
            _orderOverviewService = orderOverviewService;
        }

        public override string FromState => ShipmentState.Processing;

        public override string ToState => ShipmentState.ReadyToShip;

        public override ValidationResult Validate(Sales.Shipment entity)
        {
            var result = new ValidationResult();
            var orderOverview = _orderOverviewService.Get(entity.OrderSystemId);
            if (orderOverview == null)
            {
                throw new Exception($"The order for shipment ({entity.SystemId}) cannot be found.");
            }

            var remainingToAuthorize = orderOverview.GetRemainingToAuthorize();

            var transactionRows = orderOverview.PaymentOverviews.SelectMany(x => x.Transactions)
                .SelectMany(m => m.Rows)
                .Where(x => x.ShipmentRowSystemId is not null);

            if (remainingToAuthorize == 0
                || (transactionRows is not null && entity.Rows.All(x => transactionRows.Any(m => m.ShipmentRowSystemId == x.SystemId))))
            {
                return result;
            }

            result.AddError("Shipment", "All captures are not completed.");
            return result;
        }
    }
}
