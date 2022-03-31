using System.Threading.Tasks;
using GraphQL;
using Litium.Accelerator.Builders.Search;
using Litium.Accelerator.GraphQL.Models.Contents;
using Litium.Runtime.AutoMapper;
using Litium.Web.GraphQL;

namespace Litium.Accelerator.GraphQL.Queries.Contents
{
    internal class SubNavigationResolver : IFieldResolver<SubNavigationLinkModel>
    {
        private readonly SubNavigationViewModelBuilder _subNavigationViewModelBuilder;

        public SubNavigationResolver(
            SubNavigationViewModelBuilder subNavigationViewModelBuilder)
        {
            _subNavigationViewModelBuilder = subNavigationViewModelBuilder;
        }

        public async Task<SubNavigationLinkModel> ResolveAsync(IResolveFieldContext context)
        {
            var subNavigationViewModel = await _subNavigationViewModelBuilder.BuildAsync();
            var subNavigationModel = subNavigationViewModel.MapTo<SubNavigationLinkModel>();

            return subNavigationModel;
        }
    }
}
