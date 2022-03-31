using GraphQL.Types;
using Litium.Accelerator.GraphQL.Models.Carts;

namespace Litium.Accelerator.GraphQL.Types.Carts
{
    internal class AddItemToCartType : InputObjectGraphType<AddItemToCartModel>
    {
        public AddItemToCartType()
        {
            Field(x => x.ArticleNumber)
                .Description("The article number for the variant that should be added to cart.");

            Field(x => x.Quantity, nullable: true)
                .Description("The quantity to add to the cart.")
                .DefaultValue(1);
        }
    }
}
