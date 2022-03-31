using System.Threading.Tasks;
using GraphQL.Language.AST;
using GraphQL.Validation;
using Litium.Accelerator.GraphQL.DocumentExecutionListeners;
using Litium.Runtime.DependencyInjection;
using Litium.Web.GraphQL;
using Microsoft.AspNetCore.Http;

namespace Litium.Accelerator.GraphQL.Validations
{
    [Service(ServiceType = typeof(IValidationRule))]
    public class RequireCartContextRule : IValidationRule
    {
        private readonly INodeVisitor _nodeVisitor;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RequireCartContextRule(
            IHttpContextAccessor httpContextAccessor)
        {
            var operationNodeVisitor = new MatchingNodeVisitor<Operation>((op, context) =>
            {
                var requiredCartContext = context.TypeInfo.GetFieldDef().RequiredCartContext();
                if (requiredCartContext
                    && _httpContextAccessor.HttpContext?.Request.Headers.ContainsKey(CartContextHeaderListener.CartContextHeader) != true)
                {
                    context.ReportError(new ValidationError(context.Document.OriginalQuery, "cart-context-id-required", "Cart-Context-Id is required as header.", op));
                }
            });

            var fieldNodeVisitor = new MatchingNodeVisitor<Field>((field, context) =>
            {
                var requiredCartContext = context.TypeInfo.GetFieldDef().RequiredCartContext();
                if (requiredCartContext
                    && _httpContextAccessor.HttpContext?.Request.Headers.ContainsKey(CartContextHeaderListener.CartContextHeader) != true)
                {
                    context.ReportError(new ValidationError(context.Document.OriginalQuery, "cart-context-id-required", "Cart-Context-Id is required as header.", field));
                }
            });

            _nodeVisitor = new NodeVisitors(operationNodeVisitor, fieldNodeVisitor);
            _httpContextAccessor = httpContextAccessor;
        }

        public Task<INodeVisitor> ValidateAsync(ValidationContext context)
        {
            return Task.FromResult(_nodeVisitor);
        }
    }
}
