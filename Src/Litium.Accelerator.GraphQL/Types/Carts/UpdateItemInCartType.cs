using GraphQL.Types;
using Litium.Accelerator.GraphQL.Models.Carts;

namespace Litium.Accelerator.GraphQL.Types.Carts
{
    internal class UpdateItemInCartType : InputObjectGraphType<UpdateItemInCartModel>
    {
        public UpdateItemInCartType()
        {
            Field(x => x.Id)
                .Description("The cart item id for the item that should be updated in cart.");

            Field(x => x.Quantity, nullable: true)
                .Description("The new quantity to use for the cart row.");
        }
    }
}
