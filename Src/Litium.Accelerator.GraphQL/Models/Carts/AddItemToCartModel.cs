namespace Litium.Accelerator.GraphQL.Models.Carts
{
    public class AddItemToCartModel
    {
        public string ArticleNumber { get; set; }
        public decimal? Quantity { get; set; }
    }
}
