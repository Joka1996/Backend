using System.Collections.Generic;
using Litium.Accelerator.GraphQL.Models;
using Litium.Accelerator.GraphQL.Models.Contents.Frameworks;
using Litium.Accelerator.GraphQL.Queries.Contents.Frameworks;
using Litium.Web.GraphQL;

namespace Litium.Accelerator.GraphQL.Types.Contents.Frameworks
{
    internal class FrameworkType : ExtendableObjectGraphType<FrameworkType, FrameworkModel>
    {
        public FrameworkType()
        {
            ResolveField<JsonGraphType<IEnumerable<FavIconModel>>, IEnumerable<FavIconModel>, FavIconResolver>(x => x.FavIcons)
                .Description("The fav icons for the site.");

            ResolveField<JsonGraphType<IEnumerable<FooterNavigationModel>>, IEnumerable<FooterNavigationModel>, FooterNavigationResolver>(x => x.FooterNavigation)
                .Description("Footer navigation.");

            ResolveField<JsonGraphType<IEnumerable<LinkModel>>, IEnumerable<LinkModel>, HeaderNavigationResolver>(x => x.HeaderNavigation)
                .Description("Header navigation.");

            ResolveField<JsonGraphType<IEnumerable<PrimaryNavigationModel>>, IEnumerable<PrimaryNavigationModel>, PrimaryNavigationResolver>(x => x.PrimaryNavigation)
                .Description("Primary navigation.");
        }
    }
}
