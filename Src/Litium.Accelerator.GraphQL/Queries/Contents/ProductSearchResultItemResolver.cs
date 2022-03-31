using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphQL;
using Litium.Accelerator.Builders.Product;
using Litium.Accelerator.GraphQL.Models.Contents;
using Litium.Runtime.AutoMapper;
using Litium.Web.GraphQL;

namespace Litium.Accelerator.GraphQL.Queries.Contents
{
    public class ProductSearchResultItemResolver : IFieldResolver<IEnumerable<ProductItemModel>>
    {
        private readonly ProductItemViewModelBuilder _productItemBuilder;

        public ProductSearchResultItemResolver(
            ProductItemViewModelBuilder productItemBuilder)
        {
            _productItemBuilder = productItemBuilder;
        }

        public Task<IEnumerable<ProductItemModel>> ResolveAsync(IResolveFieldContext context)
        {
            var source = (ProductSearchResultModel)context.Source;
            var model = source.ItemsSource
                .Select(x => _productItemBuilder.Build(x.Item))
                .MapEnumerableTo<ProductItemModel>()
                .ToList()
                .AsEnumerable();

            return Task.FromResult(model);
        }
    }
}
