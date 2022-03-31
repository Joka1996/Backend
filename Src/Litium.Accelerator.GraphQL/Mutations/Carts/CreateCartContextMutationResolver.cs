using System;
using System.Threading.Tasks;
using GraphQL;
using Litium.Accelerator.GraphQL.DocumentExecutionListeners;
using Litium.Accelerator.GraphQL.Runtime;
using Litium.Sales;
using Litium.Web.GraphQL;
using Litium.Web.GraphQL.MutationTypes;
using Litium.Web.Routing;
using static Litium.Web.Routing.IChannelResolver;

namespace Litium.Accelerator.GraphQL.Mutations.Carts
{
    public class CreateCartContextMutationResolver : IFieldResolver<MutationResponse>
    {
        private readonly RouteRequestInfoAccessor _routeRequestInfoAccessor;
        private readonly RouteRequestLookupInfoAccessor _routeRequestLookupInfoAccessor;
        private readonly CartContextAccessor _cartContextAccessor;
        private readonly RouteRequestResolver _requestUrlResolver;
        private readonly RequestModelService _requestModelService;

        public CreateCartContextMutationResolver(
            RouteRequestInfoAccessor routeRequestInfoAccessor,
            RouteRequestLookupInfoAccessor routeRequestLookupInfoAccessor,
            CartContextAccessor cartContextAccessor,
            RouteRequestResolver requestUrlResolver,
            RequestModelService requestModelService)
        {
            _routeRequestInfoAccessor = routeRequestInfoAccessor;
            _routeRequestLookupInfoAccessor = routeRequestLookupInfoAccessor;
            _cartContextAccessor = cartContextAccessor;
            _requestUrlResolver = requestUrlResolver;
            _requestModelService = requestModelService;
        }

        public async Task<MutationResponse> ResolveAsync(IResolveFieldContext context)
        {
            if (!context.UserContext.TryGetValue(CartContextHeaderListener.CartContextHeader, out var o))
            {
                if (!context.Arguments.TryGetValue("url", out var urlArg)
                   || urlArg.Value is not string urlString
                   || string.IsNullOrEmpty(urlString)
                   || !Uri.TryCreate(urlString, UriKind.RelativeOrAbsolute, out var url))
                {
                    context.Errors.Add(new("Url argument is missing."));
                    return default;
                }

                if (!_requestUrlResolver.TryGet(new LookupInfo
                {
                    Scheme = url.Scheme,
                    Host = url.Port > 0 ? new(url.Host, url.Port) : new(url.Host),
                    Path = new(url.LocalPath),
                    Query = new QueryCollection(url),
                }, out var routeRequestLookupInfo, out var routeRequestInfo))
                {
                    return new()
                    {
                        Result = new()
                        {
                            Success = false,
                            Message = "Channel not found."
                        }
                    };
                }

                _routeRequestInfoAccessor.RouteRequestInfo = routeRequestInfo;
                _routeRequestLookupInfoAccessor.RouteRequestLookupInfo = routeRequestLookupInfo;
                _requestModelService.Assign();
                try
                {
                    await _cartContextAccessor.CartContext.TryInitializeCheckoutFlowAsync(() => Task.FromResult(new CheckoutFlowInfoArgs()));
                    return new()
                    {
                        Result = new()
                        {
                            Success = true,
                        }
                    };
                }
                catch (Exception e)
                {
                    return new()
                    {
                        Result = new()
                        {
                            Success = false,
                            Message = e.Message
                        }
                    };
                }
                finally
                {
                    _requestModelService.Unassign();
                    _routeRequestInfoAccessor.RouteRequestInfo = default;
                    _routeRequestLookupInfoAccessor.RouteRequestLookupInfo = default;
                }
            }

            return new()
            {
                Result = new()
                {
                    Success = false,
                    Message = "Can't create new cart-context when the Cart-Context-Id is included in header."
                }
            };
        }
    }
}
