using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GraphQL;
using Litium.Accelerator.Constants;
using Litium.Accelerator.GraphQL.Models;
using Litium.Accelerator.Routing;
using Litium.FieldFramework.FieldTypes;
using Litium.Runtime.AutoMapper;
using Litium.Web.GraphQL;

namespace Litium.Accelerator.GraphQL.Queries.Contents.Frameworks
{
    internal class HeaderNavigationResolver : IFieldResolver<IEnumerable<LinkModel>>
    {
        private readonly RequestModelAccessor _requestModelAccessor;

        public HeaderNavigationResolver(
            RequestModelAccessor requestModelAccessor)
        {
            _requestModelAccessor = requestModelAccessor;
        }

        public Task<IEnumerable<LinkModel>> ResolveAsync(IResolveFieldContext context)
        {
            var requestModel = _requestModelAccessor.RequestModel;

            var topLinkList = requestModel.WebsiteModel?
                .GetValue<IList<PointerItem>>(AcceleratorWebsiteFieldNameConstants.AdditionalHeaderLinks)?
                .OfType<PointerPageItem>()
                .MapEnumerableTo<Web.Models.LinkModel>()
                .Where(x => x != null)
                .MapEnumerableTo<LinkModel>()
                .ToList()
                .AsEnumerable();

            return Task.FromResult(topLinkList);
        }
    }
}
