using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Litium.Accelerator.Constants;
using Litium.Events;
using Litium.Runtime;
using Litium.Sales;
using Litium.StateTransitions;
using Litium.Tagging.Events;
using Litium.Taggings;

namespace Litium.Accelerator.StateTransitions
{
    [Autostart]
    public class TagEventListener : IAsyncAutostart
    {
        private readonly TaggingService _taggingService;
        private readonly StateTransitionsService _stateTransitionsService;
        private readonly EventBroker _eventBroker;

        public TagEventListener(
            TaggingService taggingService,
            StateTransitionsService stateTransitionsService,
            EventBroker eventBroker)
        {
            _taggingService = taggingService;
            _stateTransitionsService = stateTransitionsService;
            _eventBroker = eventBroker;
        }

        ValueTask IAsyncAutostart.StartAsync(CancellationToken cancellationToken)
        {
            _eventBroker.Subscribe<TagRemoved>(x => TryPutOrderToComfirmed(x));
            return ValueTask.CompletedTask;
        }

        private void TryPutOrderToComfirmed(TagRemoved tagRemoved)
        {
            if (tagRemoved.EntityType != typeof(Order) || tagRemoved.Item != OrderTaggingConstants.AwaitOrderApproval)
            {
                return;
            }

            var tags = _taggingService.GetAll<Order>(tagRemoved.SystemId);
            if (tags.Contains(OrderTaggingConstants.ApprovalDenied))
            {
                return;
            }

            _stateTransitionsService.SetState<SalesOrder>(tagRemoved.SystemId, OrderState.Confirmed);
        }
    }
}
