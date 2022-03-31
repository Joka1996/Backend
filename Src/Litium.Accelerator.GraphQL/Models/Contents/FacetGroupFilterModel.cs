using System.Collections.Generic;
using AutoMapper;
using Litium.Accelerator.ViewModels.Search;
using Litium.Runtime.AutoMapper;

namespace Litium.Accelerator.GraphQL.Models.Contents
{
    public class FacetGroupFilterModel : IAutoMapperConfiguration
    {
        public string Label { get; set; }
        public string Id { get; set; }
        public string[] SelectedOptions { get; set; }
        public bool SingleSelect { get; set; }
        public List<FacetFilterModel> Options { get; set; }

        void IAutoMapperConfiguration.Configure(IMapperConfigurationExpression cfg)
        {
            cfg.CreateMap<FacetGroupFilter, FacetGroupFilterModel>();
        }
    }
}
