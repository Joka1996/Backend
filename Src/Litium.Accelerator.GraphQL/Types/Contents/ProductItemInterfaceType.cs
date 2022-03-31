using GraphQL.Types;
using Litium.Accelerator.GraphQL.Models.Contents;
using Litium.Web.GraphQL;

namespace Litium.Accelerator.GraphQL.Types.Contents
{
    public class ProductItemInterfaceType : ExtendableInterfaceGraphType<ProductItemInterfaceType, ProductItemModel>
    {
        public ProductItemInterfaceType()
        {
            Field(p => p.SystemId, type: typeof(IdGraphType))
                .Description("The system Id");

            Field(p => p.Id)
                .Description("The article number");

            Field(p => p.Name, nullable: true)
                .Description("The name.");

            this.ImageModels(x => x.Images, x => x.ImagesSource);

            Field(p => p.Brand, nullable: true)
                .Description("The brand name.");

            Field(p => p.Color, nullable: true)
                .Description("The color.");

            Field(p => p.Description, nullable: true)
                .Description("The description.");

            Field(p => p.IsInStock)
                .Description("If the item is in stock.");

            Field(p => p.ShowBuyButton)
                .Description("If the buy button should be visible.");

            Field(p => p.ShowQuantityField)
                .Description("If the quantity field should be visible.");

            Field(p => p.Size, nullable: true)
                .Description("The size of the item.");

            Field(p => p.StockStatusDescription, nullable: true)
                .Description("Stock status information.");

            Field(p => p.Url)
                .Description("The url for the item.");

            Field(x => x.Price, type: typeof(ProductPriceType))
                .Description("Product price.");
        }
    }
}
