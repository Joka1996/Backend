using System.Collections.Generic;

namespace Litium.Accelerator.GraphQL.Models.Contents
{
    public abstract class ContentSectionModel
    {
        public IDictionary<string, object> Fields { get; set; }
    }
}
