using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphQL;
using Litium.Accelerator.Builders.Search;
using Litium.Accelerator.GraphQL.Models.Contents;
using Litium.Accelerator.ViewModels.Search;
using Litium.Runtime.AutoMapper;
using Litium.Web.GraphQL;

namespace Litium.Accelerator.GraphQL.Queries.Contents
{
    internal class FacetFilterResolver : IFieldResolver<List<FacetGroupFilterModel>>
    {
        private readonly FilterViewModelBuilder _filterViewModelBuilder;

        public FacetFilterResolver(
            FilterViewModelBuilder filterViewModelBuilder)
        {
            _filterViewModelBuilder = filterViewModelBuilder;
        }

        public async Task<List<FacetGroupFilterModel>> ResolveAsync(IResolveFieldContext context)
        {
            var filterViewModel = await _filterViewModelBuilder.BuildAsync();
            var facetFilters = filterViewModel?.Items.MapEnumerableTo<FacetGroupFilter>().MapEnumerableTo<FacetGroupFilterModel>().ToList();
            return facetFilters ?? new List<FacetGroupFilterModel>();
        }
    }
}
