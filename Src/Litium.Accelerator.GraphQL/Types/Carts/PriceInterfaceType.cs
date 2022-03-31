using Litium.Accelerator.GraphQL.Models.Carts;
using Litium.Web.GraphQL;

namespace Litium.Accelerator.GraphQL.Types.Carts
{
    internal class PriceInterfaceType : ExtendableInterfaceGraphType<PriceInterfaceType, CartPriceModel>
    {
        public PriceInterfaceType()
        {
            Field(x => x.FormattedTotalPrice)
                .Description("The formatted price for the channel.");

            Field(x => x.TotalPrice)
                .Description("The total price.");

            Field(x => x.FormattedVatAmount)
                .Description("Formatted vat amount.");

            Field(x => x.VatAmount)
                .Description("The vat amount.");

            Field(x => x.VatRate)
                .Description("The vat rate as decimal value (0.25 = 25%).");
        }
    }
}
