using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using AutoMapper;
using Litium.Accelerator.ViewModels.Product;
using Litium.Runtime.AutoMapper;
using Litium.Web.Models.Products;

namespace Litium.Accelerator.GraphQL.Models.Contents
{
    public class ProductItemModel : IAutoMapperConfiguration
    {
        public Guid SystemId { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public IList<ImageModel> Images { get; set; }
        [JsonIgnore]
        public IList<Web.Models.ImageModel> ImagesSource { get; set; }
        public string Url { get; set; }
        public string Brand { get; set; }
        public string Color { get; set; }
        public string Description { get; set; }
        public bool IsInStock { get; set; }
        public bool ShowBuyButton { get; set; }
        public bool ShowQuantityField { get; set; }
        public string Size { get; set; }
        public string StockStatusDescription { get; set; }

        public ProductPriceModel Price { get; set; }

        void IAutoMapperConfiguration.Configure(IMapperConfigurationExpression cfg)
        {
            cfg.CreateMap<ProductItemViewModel, ProductItemModel>()
                .ForMember(x => x.Images, m => m.Ignore())
                .ForMember(x => x.ImagesSource, m => m.MapFrom(x => x.Images))
                .ForMember(x => x.Url, m => m.MapFrom(x => x.Url))
                ;
            cfg.CreateMap<ProductPageViewModel, ProductItemModel>()
                .ForMember(x => x.Color, m => m.MapFrom(x => x.ColorText))
                .ForMember(x => x.Name, m => m.MapFrom(x => x.ProductItem.Name))
                .ForMember(x => x.Images, m => m.Ignore())
                .ForMember(x => x.ImagesSource, m => m.MapFrom(x => x.ProductItem.Images))
                .ForMember(x => x.Brand, m => m.MapFrom(x => x.BrandPage.Title))
                .ForMember(x => x.Id, m => m.MapFrom(x => x.ProductItem.Id))
                .ForMember(x => x.Url, m => m.MapFrom(x => x.ProductItem.Url))
                .ForMember(x => x.Description, m => m.MapFrom(x => x.ProductItem.Description))
                ;
        }
    }
}
