using Litium.Accelerator.GraphQL.Models;
using Litium.Web.GraphQL;

namespace Litium.Accelerator.GraphQL.Types
{
    public class LinkInterfaceType : ExtendableInterfaceGraphType<LinkInterfaceType, LinkModel>
    {
        public LinkInterfaceType()
        {
            Field(x => x.Enabled)
                .Description("If the filter is enabled.");

            Field(x => x.Name)
                .Description("The name of the filter.");

            Field(x => x.Url)
                .Description("The url for the filter.");
        }
    }
}
