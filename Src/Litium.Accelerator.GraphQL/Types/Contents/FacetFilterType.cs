using Litium.Accelerator.GraphQL.Models.Contents;
using Litium.Web.GraphQL;

namespace Litium.Accelerator.GraphQL.Types.Contents
{
    public class FacetFilterType : ExtendableObjectGraphType<FacetFilterType, FacetFilterModel>
    {
        public FacetFilterType()
        {
            Field(x => x.Id)
                .Description("The facet id.");

            Field(x => x.Label)
                .Description("The facet label.");

            Field(x => x.Quantity, nullable: true)
                .Description("The facet quantity.");
        }
    }
}
