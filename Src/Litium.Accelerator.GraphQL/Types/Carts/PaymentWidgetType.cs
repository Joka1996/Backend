using Litium.Accelerator.GraphQL.Models.Carts;
using Litium.Web.GraphQL;

namespace Litium.Accelerator.GraphQL.Types.Carts
{
    internal class PaymentWidgetType : ExtendableObjectGraphType<PaymentWidgetType, PaymentWidgetModel>
    {
        public PaymentWidgetType()
        {
            Field(x => x.RedirectUrl, nullable: true)
                .Description("The redirect url the client transfered to.");

            Field(x => x.ResponseString, nullable: true)
                .Description("The response string that should be rendered as html, " +
                "the result may contains JavaScript tags that should be executed.");
        }
    }
}
