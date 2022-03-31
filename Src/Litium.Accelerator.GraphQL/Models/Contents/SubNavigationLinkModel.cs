using System.Collections.Generic;
using AutoMapper;
using Litium.Runtime.AutoMapper;

namespace Litium.Accelerator.GraphQL.Models.Contents
{
    public class SubNavigationLinkModel
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public bool IsSelected { get; set; }
        public IList<SubNavigationLinkModel> Links { get; set; } = new List<SubNavigationLinkModel>();

        internal class Mapper : IAutoMapperConfiguration
        {
            void IAutoMapperConfiguration.Configure(IMapperConfigurationExpression cfg)
            {
                cfg.CreateMap<ViewModels.Framework.SubNavigationLinkModel, SubNavigationLinkModel>();
            }
        }
    }
}
