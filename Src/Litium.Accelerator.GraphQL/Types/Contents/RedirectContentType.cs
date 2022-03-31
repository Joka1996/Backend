using Litium.Accelerator.GraphQL.Models.Contents;
using Litium.Web.GraphQL;

namespace Litium.Accelerator.GraphQL.Types.Contents
{
    public class RedirectContentType : ExtendableObjectGraphType<RedirectContentType, RedirectContentModel>,
        IGraphTypeBuilder<ContentInterfaceType>
    {
        public RedirectContentType()
        {
            Field(x => x.Redirect)
                .Description("The redirect url that should be used to redirect the client.");
            Field(x => x.Permanent)
                .Description("Indicate if the redirect is a permanent redirect.");
        }

        public void Build(ContentInterfaceType target)
        {
            target.Type<RedirectContentType>();
        }
    }
}
