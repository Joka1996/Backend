using System.Collections.Generic;
using AutoMapper;
using Litium.Accelerator.ViewModels.Product;
using Litium.Runtime.AutoMapper;

namespace Litium.Accelerator.GraphQL.Models.Contents
{
    public abstract class ProductContentModel : TemplateAwareContentModel
    {
        public ProductItemModel ProductItem { get; set; }
        public List<ProductItemModel> SimilarProducts { get; set; }
        public List<ProductItemModel> AccessoriesProducts { get; set; }
        public List<ProductItemModel> PopularProducts { get; set; }
        public List<ProductItemModel> BundleProducts { get; set; }

        internal class MappingConfiguration : IAutoMapperConfiguration
        {
            void IAutoMapperConfiguration.Configure(IMapperConfigurationExpression cfg)
            {
                cfg.CreateMap<ProductPageViewModel, ProductContentModel>()
                    .ForMember(x => x.PopularProducts, m => m.MapFrom(x => x.MostSoldProducts))
                    .ForMember(x => x.AccessoriesProducts, m => m.MapFrom(x => x.AccessoriesProducts.Value))
                    .ForMember(x => x.SimilarProducts, m => m.MapFrom(x => x.SimilarProducts.Value))
                    ;
            }
        }
    }
}
