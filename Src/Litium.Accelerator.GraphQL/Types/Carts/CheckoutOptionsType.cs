using GraphQL.Types;
using Litium.Accelerator.GraphQL.Models.Carts;

namespace Litium.Accelerator.GraphQL.Types.Carts
{
    internal class CheckoutOptionsType : InputObjectGraphType<CheckoutOptionsModel>
    {
        public CheckoutOptionsType()
        {
            Field(x => x.PaymentOptionId, nullable: true)
                .Description("The payment option id that should be used for the cart.");

            Field(x => x.ShippingOptionId, nullable: true)
                .Description("The shipment option id that should be used for the cart.");
        }
    }
}
