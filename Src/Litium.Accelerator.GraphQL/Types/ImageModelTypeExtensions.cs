using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using GraphQL;
using GraphQL.Types;
using Litium.Accelerator.GraphQL.Models;

namespace Litium.Accelerator.GraphQL.Types
{
    public static class ImageModelTypeExtensions
    {
        public static void ImageModels<TType>(this ComplexGraphType<TType> graphType, Expression<Func<TType, IEnumerable<ImageModel>>> field, Func<TType, IEnumerable<Web.Models.ImageModel>> sourceModels)
        {
            graphType.Field(field, nullable: true, typeof(ListGraphType<ImageModelType>))
                .Description("The images.")
                .Argument<SizeInputType>("max", cfg => cfg.Description = "The max dimension of the image.")
                .Argument<SizeInputType>("min", cfg => cfg.Description = "The min dimension of the image.")
                .Resolve(x =>
                {
                    var minSize = x.GetArgument<SizeInputModel>("min")?.AsSize() ?? Size.Empty;
                    var maxSize = x.GetArgument<SizeInputModel>("max")?.AsSize() ?? Size.Empty;
                    return sourceModels(x.Source).Select(i =>
                    {
                        var urlData = i.GetUrlToImage(minSize, maxSize);
                        return new ImageModel
                        {
                            Dimension = urlData.Dimension.IsEmpty ? i.Dimension : urlData.Dimension,
                            Url = urlData.Url,
                        };
                    }).ToList();
                });
        }

        public static void ImageModel<TType>(this ComplexGraphType<TType> graphType, Expression<Func<TType, ImageModel>> field, Func<TType, Web.Models.ImageModel> sourceModel)
        {
            graphType.Field(field, nullable: true, typeof(ImageModelType))
                .Description("The image.")
                .Argument<SizeInputType>("max", cfg => cfg.Description = "The max dimension of the image.")
                .Argument<SizeInputType>("min", cfg => cfg.Description = "The min dimension of the image.")
                .Resolve(x =>
                {
                    var minSize = x.GetArgument<SizeInputModel>("min")?.AsSize() ?? Size.Empty;
                    var maxSize = x.GetArgument<SizeInputModel>("max")?.AsSize() ?? Size.Empty;
                    var i = sourceModel(x.Source);
                    var urlData = i.GetUrlToImage(minSize, maxSize);
                    return new ImageModel
                    {
                        Dimension = urlData.Dimension.IsEmpty ? i.Dimension : urlData.Dimension,
                        Url = urlData.Url,
                    };
                });
        }
    }
}
