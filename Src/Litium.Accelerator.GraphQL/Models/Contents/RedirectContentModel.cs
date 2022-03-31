namespace Litium.Accelerator.GraphQL.Models.Contents
{
    public class RedirectContentModel : IContentModel
    {
        public string Redirect { get; set; }
        public bool Permanent { get; set; }
    }
}
