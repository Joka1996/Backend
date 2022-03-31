using GraphQL.Types;
using Litium.ComponentModel;
using Litium.Web.GraphQL;
using Litium.Web.Models.Products;

namespace Litium.Accelerator.GraphQL.Types
{
    public class ProductPriceType : ExtendableObjectGraphType<ProductPriceType, ProductPriceModel>
    {
        public ProductPriceType()
        {
            Field<StringGraphType, string>("currency")
                .Description("The currency.")
                .Resolve(x => x.Source.Currency.Id);

            Field(x => x.FormattedCampaignPrice, nullable: true)
                .Description("The formatted campaign price this including the VAT based on the settings on the cart.")
                .Resolve(x => x.Source.FormattedCampaignPrice.NullIfEmpty());

            Field<DecimalGraphType, decimal?>("campaignPriceIncludingVat")
                .Description("The campaign price including VAT.")
                .Resolve(x => x.Source.CampaignPrice?.PriceWithVat);

            Field<DecimalGraphType, decimal?>("campaignPriceExcludingVat")
                .Description("The campaign price excluding VAT.")
                .Resolve(x => x.Source.CampaignPrice?.Price);

            Field(x => x.FormattedPrice, nullable: true)
                .Description("The formatted price this including the VAT based on the settings on the cart.")
                .Resolve(x => x.Source.FormattedPrice.NullIfEmpty());

            Field<DecimalGraphType, decimal?>("unitPriceIncludingVat")
                .Description("The unit price including VAT.")
                .Resolve(x => x.Source.Price?.PriceWithVat);

            Field<DecimalGraphType, decimal?>("unitPriceExcludingVat")
                .Description("The unit price excluding VAT.")
                .Resolve(x => x.Source.Price?.Price);

            Field<DecimalGraphType, decimal?>("vatRate")
                .Description("The vat rate.")
                .Resolve(x => x.Source.Price?.VatPercentage);
        }
    }
}
