using System.Collections.Generic;
using GraphQL.Types;
using Litium.Accelerator.GraphQL.Models.Contents;
using Litium.Accelerator.GraphQL.Queries.Contents;
using Litium.Web.GraphQL;

namespace Litium.Accelerator.GraphQL.Types.Contents
{
    public class ProductsSearchResultlType : ExtendableObjectGraphType<ProductsSearchResultlType, ProductSearchResultModel>
    {
        public ProductsSearchResultlType()
        {
            Field(x => x.TotalProducts)
                .Description("Total product results.");

            ResolveField<ListGraphType<ProductItemType>, IEnumerable<ProductItemModel>, ProductSearchResultItemResolver>(x => x.Items)
                .Description("Product items");
        }
    }
}
