using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using JetBrains.Annotations;
using Litium.Accelerator.Fields;
using Litium.Accelerator.ViewModels;
using Litium.FieldFramework;
using Litium.Products;
using Litium.Runtime.AutoMapper;
using Litium.Runtime.DependencyInjection;
using Litium.Web.Models;
using Litium.Web.Models.Products;

namespace Litium.Accelerator.Builders.Product
{
    public class ProductFieldViewModelBuilder : IViewModelBuilder<ProductFieldViewModel>
    {
        private readonly FieldDefinitionService _fieldDefinitionService;
        private readonly NamedServiceFactory<FieldFormatter> _fieldFormatterServiceFactory;

        public ProductFieldViewModelBuilder(FieldDefinitionService fieldDefinitionService, NamedServiceFactory<FieldFormatter> fieldFormatterServiceFactory)
        {
            _fieldDefinitionService = fieldDefinitionService;
            _fieldFormatterServiceFactory = fieldFormatterServiceFactory;
        }

        public IEnumerable<ProductFieldViewModel> Build([NotNull] ProductModel productModel, [NotNull] string fieldGroup, bool includeBaseProductFields = true, bool includeVariantFields = true, bool includeHiddenFields = false, bool includeEmptyFields = false)
            => Build(productModel, fieldGroup, CultureInfo.CurrentUICulture, includeBaseProductFields, includeVariantFields, includeHiddenFields, includeEmptyFields);

        public IEnumerable<ProductFieldViewModel> Build([NotNull] ProductModel productModel, [NotNull] string fieldGroup, [NotNull] CultureInfo cultureInfo, bool includeBaseProductFields = true, bool includeVariantFields = true, bool includeHiddenFields = false, bool includeEmptyFields = false)
        {
            var result = new List<ProductFieldViewModel>();
            if (includeBaseProductFields)
            {
                var baseProductFields = productModel.FieldTemplate.ProductFieldGroups?.FirstOrDefault(x => fieldGroup.Equals(x.Id, StringComparison.OrdinalIgnoreCase))?.Fields;
                if (baseProductFields != null)
                {
                    foreach (var field in baseProductFields)
                    {
                        var fieldDefinition = _fieldDefinitionService.Get<ProductArea>(field);
                        if (fieldDefinition == null || fieldDefinition.Hidden && !includeHiddenFields)
                        {
                            continue;
                        }

                        var culture = fieldDefinition.MultiCulture ? cultureInfo.Name : "*";
                        if (productModel.BaseProduct.Fields.TryGetValue(field, culture, out var value))
                        {
                            result.Add(CreateModel(fieldDefinition, cultureInfo, value));
                        }
                        else if (includeEmptyFields)
                        {
                            result.Add(CreateModel(fieldDefinition, cultureInfo, culture));
                        }
                    }
                }
            }

            if (includeVariantFields)
            {
                var variantFields = productModel.FieldTemplate.VariantFieldGroups?.FirstOrDefault(x => fieldGroup.Equals(x.Id, StringComparison.OrdinalIgnoreCase))?.Fields;
                if (variantFields != null)
                {
                    foreach (var field in variantFields)
                    {
                        var fieldDefinition = _fieldDefinitionService.Get<ProductArea>(field);
                        if (fieldDefinition == null || fieldDefinition.Hidden && !includeHiddenFields)
                        {
                            continue;
                        }

                        var culture = fieldDefinition.MultiCulture ? cultureInfo.Name : "*";
                        if (productModel.SelectedVariant.Fields.TryGetValue(field, culture, out var value))
                        {
                            result.Add(CreateModel(fieldDefinition, cultureInfo, value));
                        }
                        else if (includeEmptyFields)
                        {
                            result.Add(CreateModel(fieldDefinition, cultureInfo, culture));
                        }
                    }
                }
            }

            return result.Where(x => x != null);
        }

        private ProductFieldViewModel CreateModel([NotNull] FieldDefinition fieldDefinition, CultureInfo cultureInfo, object value = null)
        {
            var fieldFormatter = _fieldFormatterServiceFactory.GetService(fieldDefinition.FieldType);

            if (fieldFormatter == null)
            {
                return null;
            }

            if (fieldDefinition.FieldType == SystemFieldTypeConstants.MediaPointerFile)
            {
                return CreateModel("FileField", fieldDefinition, cultureInfo, new MediaResourceFieldFormatArgs { Culture = cultureInfo }, fieldFormatter, value);
            }

            if (fieldDefinition.FieldType == SystemFieldTypeConstants.MediaPointerImage)
            {
                return CreateModel("ImageField", fieldDefinition, cultureInfo, new MediaResourceFieldFormatArgs { Culture = cultureInfo }, fieldFormatter, value);
            }

            if (fieldDefinition.FieldType == SystemFieldTypeConstants.Editor)
            {
                return CreateModel("Field", fieldDefinition, cultureInfo, new FieldFormatArgs { Culture = cultureInfo }, fieldFormatter, value.MapTo<EditorString>().Value);
            }

            return CreateModel("Field", fieldDefinition, cultureInfo, new FieldFormatArgs { Culture = cultureInfo }, fieldFormatter, value);
        }
        private ProductFieldViewModel CreateModel(string viewName, FieldDefinition fieldDefinition, CultureInfo cultureInfo, FieldFormatArgs fieldFormatArgs, FieldFormatter fieldFormatter, object value = null)
        {
            return new ProductFieldViewModel
            {
                ViewName = viewName,
                Name = fieldDefinition.Localizations[cultureInfo].Name,
                Value = fieldFormatter.Format(fieldDefinition, value, fieldFormatArgs),
                Args = fieldFormatArgs
            };
        }
    }
}
