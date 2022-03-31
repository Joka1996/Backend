using Litium.StateTransitions;
using Litium.Validations;

namespace Litium.Accelerator.StateTransitions
{
    public class InitToProcessingCondition : StateTransitionValidationRule<Sales.Shipment>
    {
        public override string FromState => Sales.ShipmentState.Init;

        public override string ToState => Sales.ShipmentState.Processing;

        public override ValidationResult Validate(Sales.Shipment entity)
        {
            //Empty condition and always returns no error.
            return new ValidationResult();
        }
    }
}
