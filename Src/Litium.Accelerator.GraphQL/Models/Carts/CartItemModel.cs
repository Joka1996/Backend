namespace Litium.Accelerator.GraphQL.Models.Carts
{
    public class CartItemModel : CartPriceModel
    {
        public string Id { get; set; }
        public string ArticleNumber { get; set; }
        public decimal Quantity { get; set; }
        public string FormattedUnitPrice { get; set; }
        public decimal UnitPrice { get; set; }
        public string Description { get; set; }
        public bool SystemGenerated { get; set; }
        //lagt till allt under kommentaren
        public string Image { get; set; }
        public string Color { get; set; }
        public string Size { get; set; }

        public string Url { get; set; }
    }
}
