using System.Collections.Generic;
using System.Text.Json.Serialization;
using AutoMapper;
using Litium.Accelerator.ViewModels.Framework;
using Litium.Runtime.AutoMapper;

namespace Litium.Accelerator.GraphQL.Models.Contents.Frameworks
{
    public class PrimaryNavigationModel
    {
        public Dictionary<string, string> Attributes { get; set; }
        public ImageModel Image { get; set; }
        [JsonIgnore]
        public Web.Models.ImageModel ImageSource { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public bool Disabled { get; set; }
        public bool IsSelected { get; set; }
        public IList<PrimaryNavigationModel> Links { get; set; }
        public string ExtraInfo { get; set; }

        internal class Mapper : IAutoMapperConfiguration
        {
            void IAutoMapperConfiguration.Configure(IMapperConfigurationExpression cfg)
            {
                cfg.CreateMap<ContentLinkModel, PrimaryNavigationModel>()
                    .ForMember(x => x.Image, m => m.Ignore())
                    .ForMember(x => x.ImageSource, m => m.MapFrom(x => x.Image));
            }
        }

    }
}
