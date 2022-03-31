using AutoMapper;
using Litium.Accelerator.ViewModels.Search;
using Litium.Runtime.AutoMapper;

namespace Litium.Accelerator.GraphQL.Models.Contents
{
    public class FacetFilterModel: IAutoMapperConfiguration
    {
        public string Label { get; set; }
        public string Id { get; set; }
        public int? Quantity { get; set; }

        void IAutoMapperConfiguration.Configure(IMapperConfigurationExpression cfg)
        {
            cfg.CreateMap<FacetFilter, FacetFilterModel>();
        }
    }
}
