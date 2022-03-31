using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphQL;
using Litium.Accelerator.Builders.Framework;
using Litium.Accelerator.GraphQL.Models.Contents.Frameworks;
using Litium.Runtime.AutoMapper;
using Litium.Web.GraphQL;

namespace Litium.Accelerator.GraphQL.Queries.Contents.Frameworks
{
    internal class PrimaryNavigationResolver : IFieldResolver<IEnumerable<PrimaryNavigationModel>>
    {
        private readonly NavigationViewModelBuilder _navigationViewModelBuilder;

        public PrimaryNavigationResolver(
            NavigationViewModelBuilder navigationViewModelBuilder)
        {
            _navigationViewModelBuilder = navigationViewModelBuilder;
        }

        public async Task<IEnumerable<PrimaryNavigationModel>> ResolveAsync(IResolveFieldContext context)
        {
            var navigationViewModel = await _navigationViewModelBuilder.BuildAsync();
            var navigation = navigationViewModel.ContentLinks
                .MapEnumerableTo<PrimaryNavigationModel>()
                .ToList();
            return navigation;
        }
    }
}
