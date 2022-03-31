using System;
using System.Linq;
using System.Threading.Tasks;
using GraphQL.Execution;
using GraphQL.Language.AST;
using Litium.Runtime.DependencyInjection;
using Litium.Sales;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;

namespace Litium.Accelerator.GraphQL.DocumentExecutionListeners
{
    [Service(ServiceType = typeof(IDocumentExecutionListener))]
    public class CartContextHeaderListener : DocumentExecutionListenerBase
    {
        public const string CartContextHeader = "Cart-Context-Id";
        private readonly IDataProtector _cartContextIdProtector;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CartContextHeaderListener(
            IDataProtectionProvider dataProtectorProvider,
            IHttpContextAccessor httpContextAccessor)
        {
            _cartContextIdProtector = dataProtectorProvider.CreateProtector(typeof(CartContext).FullName, CartContextHeader, "v1");
            _httpContextAccessor = httpContextAccessor;
        }

        public override async Task BeforeExecutionAsync(IExecutionContext context)
        {
            var allSystemFields = context.Operation.SelectionSet.Children.OfType<IHaveName>().All(x => x.NameNode.Name.StartsWith("__"));
            if (!allSystemFields)
            {
                var httpContext = _httpContextAccessor.HttpContext;
                if (httpContext is not null
                    && httpContext.Request.Headers.TryGetValue(CartContextHeader, out var cartContextIdValue)
                    && cartContextIdValue.Count > 0
                    && TryUnprotect(cartContextIdValue[0], out var cartContextId))
                {
                    await httpContext.SwitchCartContextAsync(cartContextId, httpContext.RequestAborted).ConfigureAwait(false);
                    context.UserContext[CartContextHeader] = cartContextId;
                }
            }
        }

        public override Task AfterExecutionAsync(IExecutionContext context)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext?.GetCartContext().Cart is Cart cart
                && cart.SystemId != Guid.Empty)
            {
                if (context.UserContext.TryGetValue(CartContextHeader, out var cartContextHeaderValue)
                    && cartContextHeaderValue is Guid contextId
                    && contextId == cart.SystemId)
                {
                    return Task.CompletedTask;
                }

                context.Extensions[CartContextHeader] = Protect(cart.SystemId);
            }
            return Task.CompletedTask;
        }

        private string Protect(Guid cartContextId)
        {
            var protectedBytes = _cartContextIdProtector.Protect(cartContextId.ToByteArray());
            return Base64UrlTextEncoder.Encode(protectedBytes);
        }

        private bool TryUnprotect(string value, out Guid cartContextId)
        {
            try
            {
                var decodedValue = Base64UrlTextEncoder.Decode(value);
                var unprotectedValue = _cartContextIdProtector.Unprotect(decodedValue);
                cartContextId = new Guid(unprotectedValue);
                if (cartContextId != Guid.Empty)
                {
                    return true;
                }
            }
            catch
            {
                // swallow exception
                // the protected value is not protected with this secret.
            }
            cartContextId = default;
            return false;
        }
    }
}
