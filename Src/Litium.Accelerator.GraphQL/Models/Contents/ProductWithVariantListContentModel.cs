using System.Collections.Generic;
using AutoMapper;
using Litium.Accelerator.ViewModels.Product;
using Litium.Runtime.AutoMapper;

namespace Litium.Accelerator.GraphQL.Models.Contents
{
    public class ProductWithVariantListContentModel : ProductContentModel, IAutoMapperConfiguration
    {
        public List<ProductItemModel> Variants { get; set; }

        void IAutoMapperConfiguration.Configure(IMapperConfigurationExpression cfg)
        {
            cfg.CreateMap<ProductPageViewModel, ProductWithVariantListContentModel>()
                .IncludeBase<ProductPageViewModel, ProductContentModel>();
        }
    }
}
