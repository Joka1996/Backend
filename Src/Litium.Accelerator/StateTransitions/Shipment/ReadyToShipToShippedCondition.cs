using Litium.StateTransitions;
using Litium.Validations;

namespace Litium.Accelerator.StateTransitions.Shipment
{
    public class ReadyToShipToShippedCondition : StateTransitionValidationRule<Sales.Shipment>
    {
        public override string FromState => Sales.ShipmentState.ReadyToShip;

        public override string ToState => Sales.ShipmentState.Shipped;

        public override ValidationResult Validate(Sales.Shipment entity)
        {
            //Empty condition and always returns no error.
            return new ValidationResult();
        }
    }
}
