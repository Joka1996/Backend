using System.Collections.Generic;

namespace Litium.Accelerator.GraphQL.Models.Carts
{
    public class CartModel : CartPriceModel
    {
        public IEnumerable<CartItemModel> Items { get; set; }
        public string PaymentOptionId { get; set; }
        public PaymentWidgetModel PaymentWidget { get; set; }
        public string ShippingOptionId { get; set; }
        public IEnumerable<OptionItemModel> SupportedPaymentOptions { get; set; }
        public IEnumerable<OptionItemModel> SupportedShippingOptions { get; set; }
    }
}
