namespace Litium.Accelerator.GraphQL.Models.Carts
{
    public class OptionItemModel
    {
        public string Id { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        public decimal? Price { get; set; }
        public string FormattedPrice { get; set; }
    }
}
