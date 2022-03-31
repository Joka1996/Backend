using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphQL;
using Litium.Accelerator.Builders.Product;
using Litium.Accelerator.GraphQL.Models;
using Litium.Web.GraphQL;

namespace Litium.Accelerator.GraphQL.Queries.Contents
{
    internal class CategorySortResolver : IFieldResolver<List<LinkModel>>
    {
        private readonly CategoryFilteringViewModelBuilder _categoryFilteringViewModelBuilder;

        public CategorySortResolver(
            CategoryFilteringViewModelBuilder categoryFilteringViewModelBuilder)
        {
            _categoryFilteringViewModelBuilder = categoryFilteringViewModelBuilder;
        }

        public Task<List<LinkModel>> ResolveAsync(IResolveFieldContext context)
        {
            var model = _categoryFilteringViewModelBuilder.Build(int.MaxValue)
                .SortItems
                .Select(x => new LinkModel
                {
                    Enabled = x.Selected,
                    Name = x.Text,
                    Url = x.Value,
                })
                .ToList();

            return Task.FromResult(model);
        }
    }
}
