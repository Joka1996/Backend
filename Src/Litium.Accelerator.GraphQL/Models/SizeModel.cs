using System.Drawing;

namespace Litium.Accelerator.GraphQL.Models
{
    public class SizeModel
    {
        public int Height { get; set; }
        public int Width { get; set; }

        public static implicit operator SizeModel(Size size)
        {
            if (size.IsEmpty)
            {
                return null;
            }

            return new SizeModel
            {
                Height = size.Height,
                Width = size.Width,
            };
        }
    }
}
