using GraphQL.Types;
using Litium.Accelerator.GraphQL.Models.Contents;
using Litium.Web.GraphQL;

namespace Litium.Accelerator.GraphQL.Types.Contents
{
    public class ProductWithVariantListContentType : ExtendableObjectGraphType<ProductWithVariantListContentType, ProductWithVariantListContentModel>,
        IGraphTypeBuilder<ContentInterfaceType>
    {
        public ProductWithVariantListContentType()
        {
            Field(x => x.Variants, type: typeof(ListGraphType<ProductItemType>))
                .Description("This products variants.");

            Interface<ProductContentInterfaceType>();
            Interface<TemplateAwareContentType>();
        }

        void IGraphTypeBuilder<ContentInterfaceType>.Build(ContentInterfaceType target)
        {
            target.Type<ProductWithVariantListContentType>();
        }
    }
}
