using Litium.Accelerator.GraphQL.Models.Contents.Frameworks;

namespace Litium.Accelerator.GraphQL.Models.Contents
{
    public abstract class TemplateAwareContentModel : IContentModel
    {
        public string Id { get; set; }
        public string MetaTitle { get; set; }
        public string MetaDescription { get; set; }
        public string Template { get; set; }
        public FrameworkModel Framework { get; set; }
    }
}
