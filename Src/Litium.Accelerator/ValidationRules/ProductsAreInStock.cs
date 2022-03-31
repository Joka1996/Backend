using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Litium.Globalization;
using Litium.Products;
using Litium.Products.StockStatusCalculator;
using Litium.Sales;
using Litium.Security;
using Litium.Validations;
using Litium.Web;

namespace Litium.Accelerator.ValidationRules
{
    /// <summary>
    /// Validates whether product is still in stock.
    /// </summary>
    public class ProductsAreInStock : ValidationRuleBase<ValidateCartContextArgs>
    {
        private readonly IStockStatusCalculator _stockStatusCalculator;
        private readonly SecurityContextService _securityContextService;
        private readonly CountryService _countryService;
        private readonly VariantService _variantService;
        private readonly CartContextAccessor _cartContextAccessor;

        public ProductsAreInStock(
            IStockStatusCalculator stockStatusCalculator,
            SecurityContextService securityContextService,
            CountryService countryService,
            VariantService variantService,
            CartContextAccessor cartContextAccessor)
        {
            _stockStatusCalculator = stockStatusCalculator;
            _securityContextService = securityContextService;
            _countryService = countryService;
            _variantService = variantService;
            _cartContextAccessor = cartContextAccessor;
        }

        public override ValidationResult Validate(ValidateCartContextArgs entity, ValidationMode validationMode)
        {
            throw new NotSupportedException("Validation need to be done async.");
        }

        public override async Task<ValidationResult> ValidateAsync(ValidateCartContextArgs entity, ValidationMode validationMode)
        {
            var result = new ValidationResult();
            var order = entity.Cart.Order;

            if (order.Rows.Count > 0)
            {
                var personId = order.CustomerInfo?.PersonSystemId ?? _securityContextService.GetIdentityUserSystemId() ?? Guid.Empty;
                var countryId = _countryService.Get(order.CountryCode)?.SystemId;

                var updatedItems = new List<AddOrUpdateCartItemArgs>();
                var outOfStocksProducts = new List<string>();
                var notEnoughInStocksProducts = new List<string>();
                foreach (var row in order.Rows.Where(x => x.OrderRowType == OrderRowType.Product))
                {
                    var variant = _variantService.Get(row.ArticleNumber);
                    if (variant != null)
                    {
                        _stockStatusCalculator.GetStockStatuses(new StockStatusCalculatorArgs
                        {
                            UserSystemId = personId,
                            CountrySystemId = countryId
                        }, new StockStatusCalculatorItemArgs
                        {
                            VariantSystemId = variant.SystemId,
                            Quantity = row.Quantity
                        }).TryGetValue(variant.SystemId, out var stockStatus);

                        var existingStocks = stockStatus?.InStockQuantity.GetValueOrDefault();
                        //If stock status is not returned or the actual stock level is zero or below.
                        if (stockStatus == null || existingStocks <= decimal.Zero)
                        {
                            //Remove the order row from the shopping cart.
                            updatedItems.Add(new AddOrUpdateCartItemArgs
                            {
                                ArticleNumber = row.ArticleNumber,
                                Quantity = 0,
                                ConstantQuantity = true,
                            });
                            outOfStocksProducts.Add(variant.Localizations[CultureInfo.CurrentCulture].Name ?? variant.Id);
                        }
                        else
                        {
                            if (row.Quantity > existingStocks)
                            {
                                //Update the order row with available amount in stock.
                                updatedItems.Add(new AddOrUpdateCartItemArgs
                                {
                                    ArticleNumber = row.ArticleNumber,
                                    Quantity = existingStocks.Value,
                                    ConstantQuantity = true,
                                });
                                notEnoughInStocksProducts.Add(variant.Localizations[CultureInfo.CurrentCulture].Name ?? variant.Id);
                            }
                        }
                    }
                }

                if (updatedItems.Count > 0)
                {
                    foreach (var item in updatedItems)
                    {
                        await _cartContextAccessor.CartContext.AddOrUpdateItemAsync(item);
                    }
                    await _cartContextAccessor.CartContext.CalculatePaymentsAsync();
                }

                if (outOfStocksProducts.Count > 0 || notEnoughInStocksProducts.Count > 0)
                {
                    var sb = new StringBuilder();
                    if (outOfStocksProducts.Count > 0)
                    {
                        outOfStocksProducts.ForEach(x => sb.AppendFormat("sales.validation.product.outofstock".AsWebsiteText(), x));
                    }
                    if (notEnoughInStocksProducts.Count > 0)
                    {
                        notEnoughInStocksProducts.ForEach(x => sb.AppendFormat("sales.validation.product.notenoughinstock".AsWebsiteText(), x));
                    }
                    result.AddError("Cart", sb.ToString());
                }
            }

            return result;
        }
    }
}
