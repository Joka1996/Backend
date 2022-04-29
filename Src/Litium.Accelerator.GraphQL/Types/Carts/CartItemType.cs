using Litium.Accelerator.GraphQL.Models.Carts;
using Litium.Web.GraphQL;

namespace Litium.Accelerator.GraphQL.Types.Carts
{
    internal class CartItemType : ExtendableObjectGraphType<CartItemType, CartItemModel>
    {
        public CartItemType()
        {
            Field(x => x.Id)
                .Description("The row id.");

            Field(x => x.ArticleNumber)
                .Description("The article number.");

            Field(x => x.Description, nullable: true)
                .Description("The description.");

            Field(x => x.FormattedUnitPrice)
                .Description("The formatted unit price.");

            Field(x => x.Quantity)
                .Description("The quantity of the item.");

            Field(x => x.UnitPrice)
                .Description("The unit price.");
            Field(x => x.Image, nullable: true)
                .Description("Image");
            Field(x => x.Size, nullable: true)
                .Description("Size");
            Field(x => x.Color, nullable: true)
                .Description("Color");
            

            Interface<PriceInterfaceType>();
        }
    }
}
