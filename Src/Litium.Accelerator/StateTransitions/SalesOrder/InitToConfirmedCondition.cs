using System.Linq;
using Litium.Sales;
using Litium.Validations;
using Litium.StateTransitions;
using Litium.Taggings;
using Litium.Accelerator.Constants;

namespace Litium.Accelerator.StateTransitions
{
    public class InitToConfirmedCondition : StateTransitionValidationRule<SalesOrder>
    {
        private readonly OrderOverviewService _orderOverviewService;
        private readonly TaggingService _taggingService;

        public InitToConfirmedCondition(OrderOverviewService orderOverviewService,
            TaggingService taggingService)
        {
            _orderOverviewService = orderOverviewService;
            _taggingService = taggingService;
        }

        public override string FromState => Sales.OrderState.Init;

        public override string ToState => Sales.OrderState.Confirmed;

        public override ValidationResult Validate(SalesOrder entity)
        {
            var result = new ValidationResult();
            //Administrator is not allowed to Confirm orders, they are automatically confirmed when payment is done.
            // At least one payment is guaranteed. In paymentInfo, there is at least one transaction that has TransactionType = Authorize
            // and TransactionResult = success.
            var orderOverview = _orderOverviewService.Get(entity.SystemId);
            
            var isPaymentGuaranteed = orderOverview
                .PaymentOverviews.Any(x => x.Transactions.Any(t => t.TransactionType == TransactionType.Authorize
                                                                    && (t.TransactionResult == TransactionResult.Success
                                                                    || t.TransactionResult == TransactionResult.Pending)));
            if (!isPaymentGuaranteed)
            {
                result.AddError("Payment", "Payment was not guaranteed.");
            }

            if (orderOverview.SalesOrder.CustomerInfo.OrganizationSystemId.HasValue)
            {
                var tags = _taggingService.GetAll<Order>(entity.SystemId);
                //With order is placed by organization
                //The order should not have _awaitOrderApproval tag
                if (tags.Contains(OrderTaggingConstants.AwaitOrderApproval))
                {
                    result.AddError("Order", "Require order approval.");
                }

                //The order should not have _approvalDenied tag
                if (tags.Contains(OrderTaggingConstants.ApprovalDenied))
                {
                    result.AddError("Order", "Order not approved.");
                }
            }

            return result;
        }
    }
}
