using Litium.Sales;
using Litium.Validations;
using Litium.StateTransitions;

namespace Litium.Accelerator.StateTransitions
{
    public class ConfirmedToProcessingCondition : StateTransitionValidationRule<SalesOrder>
    {
        public ConfirmedToProcessingCondition()
        {
        }

        public override string FromState => Sales.OrderState.Confirmed;

        public override string ToState => Sales.OrderState.Processing;

        public override ValidationResult Validate(SalesOrder entity)
        {
            //Empty condition and always returns no error.
            return new ValidationResult();
        }
    }
}
