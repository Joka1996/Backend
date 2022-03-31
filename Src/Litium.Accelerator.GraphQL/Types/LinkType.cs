using Litium.Accelerator.GraphQL.Models;
using Litium.Web.GraphQL;

namespace Litium.Accelerator.GraphQL.Types
{
    public class LinkType : ExtendableObjectGraphType<LinkType, LinkModel>
    {
        public LinkType()
        {
            Interface<LinkInterfaceType>();
        }
    }
}
