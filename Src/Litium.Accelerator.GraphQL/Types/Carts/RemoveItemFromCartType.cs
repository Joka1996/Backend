using GraphQL.Types;
using Litium.Accelerator.GraphQL.Models.Carts;

namespace Litium.Accelerator.GraphQL.Types.Carts
{
    internal class RemoveItemFromCartType : InputObjectGraphType<RemoveItemFromCartModel>
    {
        public RemoveItemFromCartType()
        {
            Field(x => x.Id)
                .Description("The cart item id for the item that should be removed from cart.");
        }
    }
}
