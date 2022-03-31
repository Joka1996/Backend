using Litium.Accelerator.GraphQL.Models.Contents;
using Litium.Web.GraphQL;

namespace Litium.Accelerator.GraphQL.Types.Contents
{
    public class PageContentType :
        ExtendableObjectGraphType<PageContentType, PageContentModel>,
        IGraphTypeBuilder<ContentInterfaceType>
    {
        public PageContentType()
        {
            Interface<TemplateAwareContentType>();
        }

        public void Build(ContentInterfaceType target)
        {
            target.Type<PageContentType>();
        }
    }
}
