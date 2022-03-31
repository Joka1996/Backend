using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphQL;
using Litium.Accelerator.Constants;
using Litium.Accelerator.GraphQL.Models.Contents.Frameworks;
using Litium.Accelerator.Routing;
using Litium.Accelerator.ViewModels.Framework;
using Litium.FieldFramework;
using Litium.Runtime.AutoMapper;
using Litium.Web.GraphQL;

namespace Litium.Accelerator.GraphQL.Queries.Contents.Frameworks
{
    internal class FooterNavigationResolver : IFieldResolver<IEnumerable<FooterNavigationModel>>
    {
        private readonly RequestModelAccessor _requestModelAccessor;

        public FooterNavigationResolver(
            RequestModelAccessor requestModelAccessor)
        {
            _requestModelAccessor = requestModelAccessor;
        }

        public Task<IEnumerable<FooterNavigationModel>> ResolveAsync(IResolveFieldContext context)
        {
            var requestModel = _requestModelAccessor.RequestModel;

            var navigation = requestModel.WebsiteModel?
                .GetValue<IList<MultiFieldItem>>(AcceleratorWebsiteFieldNameConstants.Footer)?
                .MapEnumerableTo<SectionModel>()
                .Where(x => x != null)
                .MapEnumerableTo<FooterNavigationModel>()
                .ToList()
                .AsEnumerable();

            return Task.FromResult(navigation);
        }
    }
}
