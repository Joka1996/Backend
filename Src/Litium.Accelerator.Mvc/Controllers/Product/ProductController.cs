using System.Collections.Generic;
using Litium.Accelerator.Builders.Product;
using Litium.Products;
using Litium.Web.Models.Products;
using Microsoft.AspNetCore.Mvc;
using Litium.Web.Rendering;

namespace Litium.Accelerator.Mvc.Controllers.Product
{
    public class ProductController : ControllerBase
    {
        private readonly ProductPageViewModelBuilder _productPageViewModelBuilder;
        private readonly BaseProductService _baseProductService;
        private readonly IEnumerable<IRenderingValidator<BaseProduct>> _renderingValidators;

        public ProductController(
            ProductPageViewModelBuilder productPageViewModelBuilder,
            BaseProductService baseProductService,
            IEnumerable<IRenderingValidator<BaseProduct>> renderingValidators)
        {
            _productPageViewModelBuilder = productPageViewModelBuilder;
            _baseProductService = baseProductService;
            _renderingValidators = renderingValidators;
        }

        [HttpGet]
        public ActionResult ProductWithVariants(Variant variant)
        {
            if (variant == null)
            {
                return NotFound();
            }

            var baseProduct = _baseProductService.Get(variant.BaseProductSystemId);
            if (baseProduct == null || !_renderingValidators.Validate(baseProduct))
            {
                return NotFound();
            }
            var productPageModel = _productPageViewModelBuilder.Build(variant);
            return View(productPageModel);
        }

        [HttpGet]
        public ActionResult ProductWithVariantListing(BaseProduct baseProduct)
        {
            if (!_renderingValidators.Validate(baseProduct))
            {
                return NotFound();
            }
            var productPageModel = _productPageViewModelBuilder.Build(baseProduct);
            return View(productPageModel);
        }
    }
}
