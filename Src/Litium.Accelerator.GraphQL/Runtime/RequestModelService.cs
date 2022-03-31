using System;
using Litium.Accelerator.Routing;
using Litium.Accelerator.Search;
using Litium.Globalization;
using Litium.Products;
using Litium.Runtime.AutoMapper;
using Litium.Runtime.DependencyInjection;
using Litium.Sales;
using Litium.Web.Models.Globalization;
using Litium.Web.Models.Products;
using Litium.Web.Models.Websites;
using Litium.Web.Products.Routing;
using Litium.Web.Routing;
using Microsoft.AspNetCore.Http;

namespace Litium.Accelerator.GraphQL.Runtime
{
    [Service(ServiceType = typeof(RequestModelService), Lifetime = DependencyLifetime.Singleton)]
    public class RequestModelService
    {
        private readonly RouteRequestLookupInfoAccessor _routeRequestLookupInfoAccessor;
        private readonly RequestModelAccessor _requestModelAccessor;
        private readonly RouteRequestInfoAccessor _routeRequestInfoAccessor;
        private readonly CountryService _countryService;
        private readonly CartContextAccessor _cartContextAccessor;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RequestModelService(
            RouteRequestLookupInfoAccessor routeRequestLookupInfoAccessor,
            RequestModelAccessor requestModelAccessor,
            RouteRequestInfoAccessor routeRequestInfoAccessor,
            CountryService countryService,
            CartContextAccessor cartContextAccessor,
            IHttpContextAccessor httpContextAccessor)
        {
            _routeRequestLookupInfoAccessor = routeRequestLookupInfoAccessor;
            _requestModelAccessor = requestModelAccessor;
            _routeRequestInfoAccessor = routeRequestInfoAccessor;
            _countryService = countryService;
            _cartContextAccessor = cartContextAccessor;
            _httpContextAccessor = httpContextAccessor;
        }

        public void Assign()
        {
            var routeRequestLookupInfo = _routeRequestLookupInfoAccessor.RouteRequestLookupInfo;
            _requestModelAccessor.RequestModel = new RequestModelImpl(_cartContextAccessor.CartContext, _countryService)
            {
                _channelModel = new Lazy<ChannelModel>(() => routeRequestLookupInfo.Channel.MapTo<ChannelModel>()),
                _searchQuery = new Lazy<SearchQuery>(() => _requestModelAccessor.RequestModel.MapTo<SearchQuery>()),
                _currentPageModel = new Lazy<PageModel>(() => _routeRequestInfoAccessor.RouteRequestInfo.PageSystemId.MapTo<PageModel>()),
                _currentCategoryModel = new Lazy<CategoryModel>(() => (_routeRequestInfoAccessor.RouteRequestInfo?.Data as ProductPageData)?.CategorySystemId.MapTo<CategoryModel>()),
                _currentProductModel = new Lazy<ProductModel>(()
                    => (_routeRequestInfoAccessor.RouteRequestInfo?.Data as ProductPageData)?.VariantSystemId.MapTo<Variant>()?.MapTo<ProductModel>()
                    ?? (_routeRequestInfoAccessor.RouteRequestInfo?.Data as ProductPageData)?.BaseProductSystemId.MapTo<BaseProduct>()?.MapTo<ProductModel>()),
            };
        }

        public void Unassign()
        {
            _requestModelAccessor.RequestModel = null;
        }

        private class RequestModelImpl : RequestModel
        {
            public Lazy<ChannelModel> _channelModel;
            public Lazy<SearchQuery> _searchQuery;
            public Lazy<PageModel> _currentPageModel;
            public Lazy<ProductModel> _currentProductModel;
            public Lazy<CategoryModel> _currentCategoryModel;

            public RequestModelImpl(
                CartContext cartContext,
                CountryService countryService)
                : base(
                      cartContext,
                      countryService)
            {
            }

            public override ChannelModel ChannelModel => _channelModel?.Value;
            public override SearchQuery SearchQuery => _searchQuery?.Value;
            public override PageModel CurrentPageModel => _currentPageModel?.Value;
            public override ProductModel CurrentProductModel => _currentProductModel?.Value;
            public override CategoryModel CurrentCategoryModel => _currentCategoryModel?.Value;
        }
    }
}
