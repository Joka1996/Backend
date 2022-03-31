using System.Linq;
using System.Threading.Tasks;
using GraphQL;
using Litium.Accelerator.Constants;
using Litium.Accelerator.GraphQL.Models.Contents;
using Litium.Accelerator.Routing;
using Litium.Accelerator.Search;
using Litium.Accelerator.ViewModels.Search;
using Litium.Web.GraphQL;

namespace Litium.Accelerator.GraphQL.Queries.Contents
{
    public class ProductSearchResultResolver : IFieldResolver<ProductSearchResultModel>
    {
        private readonly RequestModelAccessor _requestModelAccessor;
        private readonly ProductSearchService _productSearchService;

        public ProductSearchResultResolver(
            RequestModelAccessor requestModelAccessor,
            ProductSearchService productSearchService)
        {
            _requestModelAccessor = requestModelAccessor;
            _productSearchService = productSearchService;
        }

        public async Task<ProductSearchResultModel> ResolveAsync(IResolveFieldContext context)
        {
            var searchQuery = _requestModelAccessor.RequestModel.SearchQuery.Clone();
            if (searchQuery.PageSize == null)
            {
                searchQuery.PageSize = _requestModelAccessor.RequestModel.WebsiteModel?.GetValue<int?>(AcceleratorWebsiteFieldNameConstants.ProductsPerPage)
                    ?? DefaultWebsiteFieldValueConstants.ProductsPerPage;
            }

            var searchResults = await _productSearchService.SearchAsync(searchQuery, searchQuery.Tags, true, true, true);
            if (searchResults is null)
            {
                return default;
            }

            return new ()
            {
                TotalProducts = searchResults.Total,
                ItemsSource = searchResults.Items.Value.Cast<ProductSearchResult>(),
            };
        }
    }
}
