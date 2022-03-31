using System;
using Litium.Accelerator.Builders.Home;
using Litium.Security;
using Litium.Web.Models.Websites;
using Microsoft.AspNetCore.Mvc;

namespace Litium.Accelerator.Mvc.Controllers.Home
{
    public class HomeController : ControllerBase
    {
        private readonly HomeViewModelBuilder _builder;
        private readonly AuthorizationService _authorizationService;

        public HomeController(HomeViewModelBuilder builder, AuthorizationService authorizationService)
        {
            _builder = builder;
            _authorizationService = authorizationService;
        }

        public IActionResult Index(PageModel currentPageModel, Guid? previewGlobalBlock = null)
        {
            if (previewGlobalBlock != null && _authorizationService.HasOperation(Operations.Function.Websites.UI))
            {
                return View("Index", _builder.ForPreviewGlobalBlock(previewGlobalBlock.Value));
            }
            return View(_builder.Build(currentPageModel));
        }
    }
}