using Litium.Accelerator.GraphQL.Models.Contents;
using Litium.Accelerator.GraphQL.Models.Contents.Frameworks;
using Litium.Accelerator.GraphQL.Types.Contents.Frameworks;
using Litium.Web.GraphQL;

namespace Litium.Accelerator.GraphQL.Types.Contents
{
    public class TemplateAwareContentType : ExtendableInterfaceGraphType<TemplateAwareContentType, TemplateAwareContentModel>
    {
        public TemplateAwareContentType()
        {
            Field(x => x.Id)
                .Description("The id for the item.");

            Field(x => x.Template)
                .Description("The template id for the content.");

            Field(x => x.MetaTitle, nullable: true)
                .Description("The meta title.");

            Field(x => x.MetaDescription, nullable: true)
                .Description("The meta description.");

            Field(x => x.Framework, type: typeof(FrameworkType))
                .Description("Framework data for the content.")
                .Resolve(_ => new FrameworkModel());
        }
    }
}
