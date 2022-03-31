using Litium.Accelerator.Builders;
using Litium.Sales;

namespace Litium.Accelerator.ViewModels.Checkout
{
    public class PaymentOptionViewModel : IViewModel
    {
        public ProviderOptionIdentifier Id { get; set; }
        public string Name { get; set; }
        public decimal? Price { get; set; }
        public string FormattedPrice { get; set; }
    }
}
