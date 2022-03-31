using GraphQL.Types;
using Litium.Accelerator.GraphQL.Models.Contents;
using Litium.Web.GraphQL;

namespace Litium.Accelerator.GraphQL.Types.Contents
{
    public class ProductContentInterfaceType : ExtendableInterfaceGraphType<ProductContentInterfaceType, ProductContentModel>
    {
        public ProductContentInterfaceType()
        {
            Field(x => x.PopularProducts, type: typeof(ListGraphType<ProductItemType>))
                .Description("The popular products that are sold together with this product.");

            Field(x => x.ProductItem, type: typeof(ProductItemType))
                .Description("Product item");

            Field(x => x.SimilarProducts, type: typeof(ListGraphType<ProductItemType>))
                .Description("Similar products to this product.");
        }
    }
}
