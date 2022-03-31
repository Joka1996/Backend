using System.Collections.Generic;
using AutoMapper;
using Litium.Accelerator.ViewModels.Product;
using Litium.Runtime.AutoMapper;

namespace Litium.Accelerator.GraphQL.Models.Contents
{
    public class ProductWithVariantsContentModel : ProductContentModel, IAutoMapperConfiguration
    {
        public string Filter1Text { get; set; }
        public List<ProductFilterModel> Filter1Items { get; set; }
        public string Filter2Text { get; set; }
        public List<ProductFilterModel> Filter2Items { get; set; }

        public class ProductFilterModel : LinkModel
        {
            public bool IsActive { get; set; }
            public string Value { get; set; }
        }

        void IAutoMapperConfiguration.Configure(IMapperConfigurationExpression cfg)
        {
            cfg.CreateMap<ProductPageViewModel, ProductWithVariantsContentModel>()
                .IncludeBase<ProductPageViewModel, ProductContentModel>()
                .ForMember(x => x.Filter1Text, m => m.MapFrom(x => x.ColorText))
                .ForMember(x => x.Filter1Items, m => m.MapFrom(x => x.Colors))
                .ForMember(x => x.Filter2Text, m => m.MapFrom(x => x.SizeText))
                .ForMember(x => x.Filter2Items, m => m.MapFrom(x => x.Sizes))
                ;

            cfg.CreateMap<ProductPageViewModel.ProductFilterItem, ProductFilterModel>()
                .ForMember(x => x.Name, m => m.MapFrom(x => x.Title))
                ;
        }
    }
}
