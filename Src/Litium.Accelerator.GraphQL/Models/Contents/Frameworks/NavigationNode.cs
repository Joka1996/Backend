using System.Collections.Generic;

namespace Litium.Accelerator.GraphQL.Models.Contents.Frameworks
{
    public class NavigationNode : LinkModel
    {
        public IEnumerable<NavigationNode> ChildNodes { get; set; }
    }
}
