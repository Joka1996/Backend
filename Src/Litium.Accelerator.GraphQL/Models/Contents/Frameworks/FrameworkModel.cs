using System.Collections.Generic;

namespace Litium.Accelerator.GraphQL.Models.Contents.Frameworks
{
    public class FrameworkModel
    {
        public IEnumerable<FavIconModel> FavIcons { get; set; }
        public IEnumerable<LinkModel> HeaderNavigation { get; set; }
        public IEnumerable<PrimaryNavigationModel> PrimaryNavigation { get; set; }
        public IEnumerable<FooterNavigationModel> FooterNavigation { get; set; }
    }
}
