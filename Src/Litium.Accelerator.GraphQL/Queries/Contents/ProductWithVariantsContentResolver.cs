using System.Collections.Generic;
using System.Threading.Tasks;
using GraphQL;
using Litium.Accelerator.Builders.Product;
using Litium.Accelerator.GraphQL.Models.Contents;
using Litium.Accelerator.Routing;
using Litium.FieldFramework;
using Litium.Products;
using Litium.Runtime.AutoMapper;
using Litium.Runtime.DependencyInjection;
using Litium.Web.Rendering;
using Litium.Web.Routing;

namespace Litium.Accelerator.GraphQL.Queries.Contents
{
    [Service(Name = Litium.Accelerator.Constants.ProductTemplateNameConstants.Product)]
    internal class ProductWithVariantsContentResolver : IProductTemplateResolver<IContentModel>
    {
        private readonly IEnumerable<IRenderingValidator<BaseProduct>> _renderingValidators;
        private readonly RouteRequestInfoAccessor _routeRequestInfoAccessor;
        private readonly RequestModelAccessor _requestModelAccessor;
        private readonly ProductPageViewModelBuilder _productPageViewModelBuilder;

        public ProductWithVariantsContentResolver(
            IEnumerable<IRenderingValidator<BaseProduct>> renderingValidators,
            RouteRequestInfoAccessor routeRequestInfoAccessor,
            RequestModelAccessor requestModelAccessor,
            ProductPageViewModelBuilder productPageViewModelBuilder)
        {
            _renderingValidators = renderingValidators;
            _routeRequestInfoAccessor = routeRequestInfoAccessor;
            _requestModelAccessor = requestModelAccessor;
            _productPageViewModelBuilder = productPageViewModelBuilder;
        }

        public Task<IContentModel> ResolveAsync(IResolveFieldContext context)
        {
            var routeRequestInfo = _routeRequestInfoAccessor.RouteRequestInfo;
            var requestModel = _requestModelAccessor.RequestModel;
            var productModel = requestModel?.CurrentProductModel;
            var baseProduct = productModel?.BaseProduct;
            var variant = productModel?.SelectedVariant;

            if (baseProduct is null
                || variant is null
                || requestModel is null
                || !_renderingValidators.Validate(baseProduct))
            {
                return Task.FromResult<IContentModel>(default);
            }

            var viewModel = _productPageViewModelBuilder.Build(variant);
            var model = viewModel.MapTo<ProductWithVariantsContentModel>();

            model.Id = (productModel.UseVariantUrl ? productModel.SelectedVariant.SystemId : productModel.BaseProduct.SystemId).ToString();
            model.Template = routeRequestInfo.TemplateId;
            model.MetaTitle = productModel.GetValue<string>(SystemFieldDefinitionConstants.SeoTitle)
                ?? productModel.GetValue<string>(SystemFieldDefinitionConstants.Name);
            model.MetaDescription = productModel.GetValue<string>(SystemFieldDefinitionConstants.SeoDescription)
                ?? productModel.GetValue<string>(SystemFieldDefinitionConstants.Description);

            return Task.FromResult<IContentModel>(model);
        }
    }
}
