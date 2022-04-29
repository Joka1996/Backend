using GraphQL;
using Litium.Accelerator.GraphQL.Models.Carts;
using Litium.FieldFramework;
using Litium.FieldFramework.FieldTypes;
using Litium.Globalization;
using Litium.Products;
using Litium.Runtime.AutoMapper;
using Litium.Runtime.DependencyInjection;
using Litium.Sales;
using Litium.Web.GraphQL;
using Litium.Web.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Litium.Accelerator.GraphQL.Queries.Carts
{
    [Service]
    internal class CartItemsResolver : IFieldResolver<IEnumerable<CartItemModel>>
    {
        private readonly CartContextAccessor _cartContextAccessor;
        private readonly CurrencyService _currencyService;
        private readonly VariantService _variantService;
        private readonly FieldDefinitionService _fieldDefinitionService;

        public CartItemsResolver(
            CartContextAccessor cartContextAccessor,
            CurrencyService currencyService,
            VariantService variantService,
            FieldDefinitionService fieldDefinitionService)
        {
            _cartContextAccessor = cartContextAccessor;
            _currencyService = currencyService;
            _variantService = variantService;
            _fieldDefinitionService = fieldDefinitionService;
        }

        public Task<IEnumerable<CartItemModel>> ResolveAsync(IResolveFieldContext context)
        {
            var model = Enumerable.Empty<CartItemModel>();
            var cartContext = _cartContextAccessor.CartContext;
            if (cartContext?.Cart is Sales.Cart cart && cart.Order is SalesOrder salesOrder)
            {
                var currency = _currencyService.Get(salesOrder.CurrencyCode);
                model = salesOrder.Rows.Select(x =>
                    {
                        return new CartItemModel
                        {
                            ArticleNumber = x.ArticleNumber,
                            Description = x.Description,
                            FormattedTotalPrice = currency.Format(x.TotalIncludingVat, false, CultureInfo.CurrentUICulture),
                            FormattedUnitPrice = currency.Format(x.UnitPriceIncludingVat, false, CultureInfo.CurrentUICulture),
                            Id = x.Id,
                            Quantity = x.Quantity,
                            SystemGenerated = x.SystemGenerated,
                            TotalPrice = x.TotalIncludingVat,
                            UnitPrice = x.UnitPriceIncludingVat,
                            VatAmount = x.TotalVat,
                            VatRate = x.VatRate,
                            Image = _variantService.Get(x.ArticleNumber).Fields.GetValue<IList<Guid>>("_images")?.First().MapTo<ImageModel>()?.GetUrlToImage(new System.Drawing.Size(), new System.Drawing.Size())?.Url ?? "",
                            Color = GetTranslation(_fieldDefinitionService.Get<ProductArea>("Color"), _variantService.Get(x.ArticleNumber).Fields.GetValue<string>("Color")),
                            Size = GetTranslation(_fieldDefinitionService.Get<ProductArea>("Size"), _variantService.Get(x.ArticleNumber).Fields.GetValue<string>("Size"))
                        };
                    })
                    .ToList();
            }

            return Task.FromResult(model);
        }

        public string GetTranslation(IFieldDefinition field, string key, CultureInfo culture = null)
        {
            if (key == null)
            {
                return null;
            }

            var options = field.Option as TextOption;
            var option = options?.Items.FirstOrDefault(x => x.Value == key);
            if (option == null)
            {
                return key;
            }

            if (option.Name.TryGetValue(culture?.Name ?? CultureInfo.CurrentUICulture.Name, out string translation) && !string.IsNullOrEmpty(translation))
            {
                return translation;
            }

            return key;
        }
    }
}
