namespace Litium.Accelerator.GraphQL.Models.Carts
{
    public class CartPriceModel
    {
        public decimal TotalPrice { get; set; }
        public string FormattedTotalPrice { get; set; }
        public decimal VatRate { get; set; }
        public string FormattedVatAmount { get; set; }
        public decimal VatAmount { get; set; }
    }
}
