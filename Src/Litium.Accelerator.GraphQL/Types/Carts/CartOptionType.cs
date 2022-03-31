using Litium.Accelerator.GraphQL.Models.Carts;
using Litium.Web.GraphQL;

namespace Litium.Accelerator.GraphQL.Types.Carts
{
    internal class CartOptionType : ExtendableObjectGraphType<CartOptionType, OptionItemModel>
    {
        public CartOptionType()
        {
            Field(x => x.Id)
                .Description("The id of the option.");

            Field(x => x.Description, nullable: true)
                .Description("The description.");

            Field(x => x.Name)
                .Description("The name.");
        }
    }
}
