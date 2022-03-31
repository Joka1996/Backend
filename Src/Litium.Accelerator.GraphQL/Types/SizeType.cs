using GraphQL.Types;
using Litium.Accelerator.GraphQL.Models;

namespace Litium.Accelerator.GraphQL.Types
{
    public class SizeType : ObjectGraphType<SizeModel>
    {
        public SizeType()
        {
            Field(x => x.Height)
                .Description("The height of the image.");
            Field(x => x.Width)
                .Description("The width of the image.");
        }
    }
}
