using System.Collections.Generic;
using Litium.Accelerator.Constants;

namespace Litium.Accelerator.GraphQL.Models.Contents
{
    public class CategoryContentModel : TemplateAwareContentModel
    {
        public List<FacetGroupFilterModel> FacetFilters { get; set; }
        public SubNavigationLinkModel Navigation { get; set; }
        public List<LinkModel> Sort { get; set; }
        public ProductSearchResultModel Products { get; set; }
        public NavigationType? NavigationType { get; set; }
    }
}
