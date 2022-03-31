using System.Threading.Tasks;
using GraphQL.Execution;
using Litium.Accelerator.GraphQL.Runtime;
using Litium.Runtime.DependencyInjection;
using Litium.Web.Routing;

namespace Litium.Accelerator.GraphQL.DocumentExecutionListeners
{
    [Service(ServiceType = typeof(IDocumentExecutionListener))]
    public class RequestContextCleanup : DocumentExecutionListenerBase
    {
        private readonly RouteRequestInfoAccessor _routeRequestInfoAccessor;
        private readonly RouteRequestLookupInfoAccessor _routeRequestLookupInfoAccessor;
        private readonly RequestModelService _requestModelService;

        public RequestContextCleanup(
            RouteRequestInfoAccessor routeRequestInfoAccessor,
            RouteRequestLookupInfoAccessor routeRequestLookupInfoAccessor,
            RequestModelService requestModelService)
        {
            _routeRequestInfoAccessor = routeRequestInfoAccessor;
            _routeRequestLookupInfoAccessor = routeRequestLookupInfoAccessor;
            _requestModelService = requestModelService;
        }

        public override Task AfterExecutionAsync(IExecutionContext context)
        {
            _requestModelService.Unassign();
            _routeRequestInfoAccessor.RouteRequestInfo = default;
            _routeRequestLookupInfoAccessor.RouteRequestLookupInfo = default;

            return Task.CompletedTask;
        }
    }
}
