using System;
using System.Linq;
using Litium.Globalization;
using Litium.Sales;
using Litium.Sales.Factory;
using Litium.Security;
using Litium.Validations;
using Litium.Web;

namespace Litium.Accelerator.ValidationRules
{
    /// <summary>
    /// Validates whether product is still available to buy, since it was last put to cart.
    /// </summary>
    public class ProductIsAvailableForSale : ValidationRuleBase<ValidateCartContextArgs>
    {
        private readonly ISalesOrderRowFactory _salesOrderRowFactory;
        private readonly SecurityContextService _securityContextService;
        private readonly CountryService _countryService;
        private readonly CurrencyService _currencyService;

        public ProductIsAvailableForSale(
            ISalesOrderRowFactory salesOrderRowFactory,
            SecurityContextService securityContextService,
            CountryService countryService,
            CurrencyService currencyService)
        {
            _salesOrderRowFactory = salesOrderRowFactory;
            _securityContextService = securityContextService;
            _countryService = countryService;
            _currencyService = currencyService;
        }

        public override ValidationResult Validate(ValidateCartContextArgs entity, ValidationMode validationMode)
        {
            var result = new ValidationResult();
            var order = entity.Cart.Order;

            if (order.Rows.Count > 0)
            {
                var personId = order.CustomerInfo?.PersonSystemId ?? _securityContextService.GetIdentityUserSystemId() ?? Guid.Empty;
                var orderRows = order.Rows.Where(x => x.OrderRowType == OrderRowType.Product)
                                          .Select(orderRow => _salesOrderRowFactory.Create(new CreateSalesOrderRowArgs
                                          {
                                              ArticleNumber = orderRow.ArticleNumber,
                                              Quantity = orderRow.Quantity,
                                              PersonSystemId = personId,
                                              ChannelSystemId = order.ChannelSystemId ?? Guid.Empty,
                                              CountrySystemId = _countryService.Get(order.CountryCode)?.SystemId ?? Guid.Empty,
                                              CurrencySystemId = _currencyService.Get(order.CurrencyCode)?.SystemId ?? Guid.Empty
                                          }));

                if (orderRows.Any(result => result == null))
                {
                    result.AddError("Cart", "sales.validation.product.nolongeravailableforsale".AsWebsiteText());
                }
            }

            return result;
        }
    }
}
