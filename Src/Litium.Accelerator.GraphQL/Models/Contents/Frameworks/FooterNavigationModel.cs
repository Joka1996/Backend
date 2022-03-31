using System.Collections.Generic;
using AutoMapper;
using Litium.Accelerator.ViewModels.Framework;
using Litium.Runtime.AutoMapper;

namespace Litium.Accelerator.GraphQL.Models.Contents.Frameworks
{
    public class FooterNavigationModel : IAutoMapperConfiguration
    {
        public virtual IList<LinkModel> Links { get; set; } = new List<LinkModel>();
        public virtual string Text { get; set; }
        public virtual string Title { get; set; }

        void IAutoMapperConfiguration.Configure(IMapperConfigurationExpression cfg)
        {
            cfg.CreateMap<SectionModel, FooterNavigationModel>()
                .ForMember(x => x.Links, m => m.MapFrom(x => x.SectionLinkList))
                .ForMember(x => x.Text, m => m.MapFrom(x => x.SectionText))
                .ForMember(x => x.Title, m => m.MapFrom(x => x.SectionTitle));
        }
    }
}
