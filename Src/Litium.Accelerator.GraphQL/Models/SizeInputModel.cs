using System.Drawing;

namespace Litium.Accelerator.GraphQL.Models
{
    public class SizeInputModel
    {
        public int Height { get; set; }
        public int Width { get; set; }

        public Size AsSize()
        {
            return new Size(Width, Height);
        }
    }
}
