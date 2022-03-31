using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphQL;
using Litium.Accelerator.Builders.Framework;
using Litium.Accelerator.Constants;
using Litium.Accelerator.GraphQL.Models.Contents.Frameworks;
using Litium.Accelerator.Routing;
using Litium.Runtime.AutoMapper;
using Litium.Web.GraphQL;

namespace Litium.Accelerator.GraphQL.Queries.Contents.Frameworks
{
    internal class FavIconResolver : IFieldResolver<IEnumerable<FavIconModel>>
    {
        private readonly FaviconViewModelBuilder _faviconViewModelBuilder;
        private readonly RequestModelAccessor _requestModelAccessor;

        public FavIconResolver(
            FaviconViewModelBuilder faviconViewModelBuilder,
            RequestModelAccessor requestModelAccessor)
        {
            _faviconViewModelBuilder = faviconViewModelBuilder;
            _requestModelAccessor = requestModelAccessor;
        }

        public Task<IEnumerable<FavIconModel>> ResolveAsync(IResolveFieldContext context)
        {
            var model = default(IEnumerable<FavIconModel>);

            var requestModel = _requestModelAccessor.RequestModel;
            var favicon = requestModel?.WebsiteModel?.GetValue<Guid?>(AcceleratorWebsiteFieldNameConstants.LogotypeIcon);
            if (favicon.HasValue)
            {
                var viewModel = _faviconViewModelBuilder.Build(favicon.Value);
                model = viewModel
                    .MapEnumerableTo<FavIconModel>()
                    .ToList();
            }

            return Task.FromResult(model);
        }
    }
}
