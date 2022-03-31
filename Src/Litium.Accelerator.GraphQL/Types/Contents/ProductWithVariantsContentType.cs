using GraphQL.Types;
using Litium.Accelerator.GraphQL.Models.Contents;
using Litium.Web.GraphQL;

namespace Litium.Accelerator.GraphQL.Types.Contents
{
    public class ProductWithVariantsContentType : ExtendableObjectGraphType<ProductWithVariantsContentType, ProductWithVariantsContentModel>,
        IGraphTypeBuilder<ContentInterfaceType>
    {
        public ProductWithVariantsContentType()
        {
            Field(x => x.Filter1Items, type: typeof(ListGraphType<ProductFilterType>))
                .Description("The filter items.");

            Field(x => x.Filter1Text, nullable: true)
                .Description("The filter text.");

            Field(x => x.Filter2Items, type: typeof(ListGraphType<ProductFilterType>))
                .Description("The filter items.");

            Field(x => x.Filter2Text, nullable: true)
                .Description("The filter text.");

            Interface<ProductContentInterfaceType>();
            Interface<TemplateAwareContentType>();
        }

        void IGraphTypeBuilder<ContentInterfaceType>.Build(ContentInterfaceType target)
        {
            target.Type<ProductWithVariantsContentType>();
        }

        public class ProductFilterType : ExtendableObjectGraphType<ProductFilterType, ProductWithVariantsContentModel.ProductFilterModel>
        {
            public ProductFilterType()
            {
                Field(x => x.IsActive)
                    .Description("If the filter is active.");

                Field(x => x.Value)
                    .Description("The value for the filter.");

                Interface<LinkInterfaceType>();
            }
        }
    }
}
