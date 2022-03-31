using GraphQL.Types;
using Litium.Accelerator.GraphQL.Models.Contents;
using Litium.Web.GraphQL;

namespace Litium.Accelerator.GraphQL.Types.Contents
{
    public class ContentInterfaceType : UnionGraphType,
        ISupportGraphTypeBuilder<ContentInterfaceType, IContentModel>
    {
        public ContentInterfaceType()
        {
        }
    }
}
