using System.Collections.Generic;
using System.Threading.Tasks;
using GraphQL;
using Litium.Accelerator.Constants;
using Litium.Accelerator.Extensions;
using Litium.Accelerator.GraphQL.Models.Contents;
using Litium.Accelerator.Routing;
using Litium.Products;
using Litium.Runtime.DependencyInjection;
using Litium.Web.Products.Routing;
using Litium.Web.Rendering;
using Litium.Web.Routing;

namespace Litium.Accelerator.GraphQL.Queries.Contents
{
    [Service(Name = ProductTemplateNameConstants.Category)]
    internal class CategoryContentResolver : ICategoryTemplateResolver<IContentModel>
    {
        private readonly IEnumerable<IRenderingValidator<Category>> _renderingValidators;
        private readonly RouteRequestInfoAccessor _routeRequestInfoAccessor;
        private readonly RequestModelAccessor _requestModelAccessor;
        private readonly CategoryService _categoryService;

        public CategoryContentResolver(
            IEnumerable<IRenderingValidator<Category>> renderingValidators,
            RouteRequestInfoAccessor routeRequestInfoAccessor,
            RequestModelAccessor requestModelAccessor,
            CategoryService categoryService)
        {
            _renderingValidators = renderingValidators;
            _routeRequestInfoAccessor = routeRequestInfoAccessor;
            _requestModelAccessor = requestModelAccessor;
            _categoryService = categoryService;
        }

        public Task<IContentModel> ResolveAsync(IResolveFieldContext context)
        {
            var routeRequestInfo = _routeRequestInfoAccessor.RouteRequestInfo;
            var productData = routeRequestInfo?.Data as ProductPageData;
            var requestModel = _requestModelAccessor.RequestModel;
            if (productData?.CategorySystemId is null
                || requestModel is null)
            {
                return Task.FromResult<IContentModel>(default);
            }

            var category = _categoryService.Get(productData.CategorySystemId.Value);
            if (!_renderingValidators.Validate(category))
            {
                return Task.FromResult<IContentModel>(default);
            }

            var model = new CategoryContentModel();
            var navigationType = requestModel.WebsiteModel?.GetNavigationType();
            var localiazed = category.Localizations.CurrentCulture;

            model.Id = category.SystemId.ToString();
            model.Template = routeRequestInfo.TemplateId;
            model.MetaTitle = localiazed.SeoTitle ?? localiazed.Name;
            model.MetaDescription = localiazed.SeoDescription ?? localiazed.Description;
            model.NavigationType = navigationType;

            return Task.FromResult<IContentModel>(model);
        }
    }
}
