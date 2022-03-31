using GraphQL.Types;
using Litium.Accelerator.GraphQL.Models;

namespace Litium.Accelerator.GraphQL.Types
{
    public class ImageModelType : ObjectGraphType<ImageModel>
    {
        public ImageModelType()
        {
            Field(x => x.Dimension, type: typeof(SizeType))
                .Description("The dimension of the image.");
            Field(x => x.Url)
                .Description("The url of the image.");
        }
    }
}
