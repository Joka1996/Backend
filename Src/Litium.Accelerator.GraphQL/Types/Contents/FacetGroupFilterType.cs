using GraphQL.Types;
using Litium.Accelerator.GraphQL.Models.Contents;
using Litium.Web.GraphQL;

namespace Litium.Accelerator.GraphQL.Types.Contents
{
    public class FacetGroupFilterType : ExtendableObjectGraphType<FacetGroupFilterType, FacetGroupFilterModel>
    {
        public FacetGroupFilterType()
        {
            Field(x => x.Id)
                .Description("The facet group id.");

            Field(x => x.Label)
                .Description("The facet group label.");

            Field(x => x.Options, type: typeof(ListGraphType<FacetFilterType>))
                .Description("The facet items.");

            Field(x => x.SelectedOptions, type: typeof(ListGraphType<StringGraphType>))
                .Description("The selected options.");

            Field(x => x.SingleSelect)
                .Description("If single select facet group.");
        }
    }
}
