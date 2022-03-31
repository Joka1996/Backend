using AutoMapper;
using Litium.Accelerator.ViewModels.Framework;
using Litium.ComponentModel;
using Litium.Runtime.AutoMapper;

namespace Litium.Accelerator.GraphQL.Models.Contents.Frameworks
{
    public class FavIconModel : IAutoMapperConfiguration
    {
        public string Href { get; set; }
        public string Rel { get; set; }
        public string Size { get; set; }
        public string Type { get; set; }

        void IAutoMapperConfiguration.Configure(IMapperConfigurationExpression cfg)
        {
            cfg.CreateMap<FaviconViewModel, FavIconModel>()
                .ForMember(x => x.Type, m => m.MapFrom(x => x.Type.NullIfEmpty()));
        }
    }
}
