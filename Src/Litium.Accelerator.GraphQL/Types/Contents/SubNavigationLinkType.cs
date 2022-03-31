using GraphQL.Types;
using Litium.Accelerator.GraphQL.Models.Contents;
using Litium.Web.GraphQL;

namespace Litium.Accelerator.GraphQL.Types.Contents
{
    public class SubNavigationLinkType : ExtendableObjectGraphType<SubNavigationLinkType, SubNavigationLinkModel>
    {
        public SubNavigationLinkType()
        {
            Field(x => x.IsSelected)
                .Description("If navigation is selected.");

            Field(x => x.Name)
                .Description("The navigation link name.");

            Field(x => x.Url, nullable: true)
                .Description("The navigation link url.");

            Field(x => x.Links, type: typeof(ListGraphType<SubNavigationLinkType>))
                .Description("The facet items.");
        }
    }
}
