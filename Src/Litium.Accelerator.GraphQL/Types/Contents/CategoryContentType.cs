using Litium.Accelerator.GraphQL.Queries.Contents;
using Litium.Accelerator.GraphQL.Models.Contents;
using Litium.Web.GraphQL;
using GraphQL.Types;
using Litium.Accelerator.GraphQL.Models;
using System.Collections.Generic;

namespace Litium.Accelerator.GraphQL.Types.Contents
{
    public class CategoryContentType : ExtendableObjectGraphType<CategoryContentType, CategoryContentModel>,
        IGraphTypeBuilder<ContentInterfaceType>
    {
        public CategoryContentType()
        {
            ResolveField<ProductsSearchResultlType, ProductSearchResultModel, ProductSearchResultResolver>(x => x.Products);

            ResolveField<ListGraphType<FacetGroupFilterType>, List<FacetGroupFilterModel>, FacetFilterResolver>(x => x.FacetFilters)
                .Description("The facet filters.");

            ResolveField<SubNavigationLinkType, SubNavigationLinkModel, SubNavigationResolver>(x => x.Navigation)
                .Description("The sub navigation.");

            ResolveField<ListGraphType<LinkType>, List<LinkModel>, CategorySortResolver>(x => x.Sort);
            
            Interface<TemplateAwareContentType>();
        }

        public void Build(ContentInterfaceType target)
        {
            target.Type<CategoryContentType>();
        }
    }
}
