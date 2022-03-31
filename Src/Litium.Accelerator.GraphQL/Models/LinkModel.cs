using AutoMapper;
using Litium.Runtime.AutoMapper;

namespace Litium.Accelerator.GraphQL.Models
{
    public class LinkModel : IAutoMapperConfiguration
    {
        public bool Enabled { get; set; }
        public string Url { get; set; }
        public string Name { get; set; }

        void IAutoMapperConfiguration.Configure(IMapperConfigurationExpression cfg)
        {
            cfg.CreateMap<Web.Models.LinkModel, LinkModel>()
                .ForMember(x => x.Enabled, m => m.MapFrom(x => x.AccessibleByUser))
                .ForMember(x => x.Url, m => m.MapFrom(x => x.Href))
                .ForMember(x => x.Name, m => m.MapFrom(x => x.Text));
        }
    }
}
