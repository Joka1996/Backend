using GraphQL.Types;
using Litium.Accelerator.GraphQL.Queries.Carts;
using Litium.Accelerator.GraphQL.Queries.Contents;
using Litium.Accelerator.GraphQL.Types.Carts;
using Litium.Accelerator.GraphQL.Types.Contents;
using Litium.Accelerator.GraphQL.Models.Carts;
using Litium.Accelerator.GraphQL.Models.Contents;
using Litium.Web.GraphQL;
using Litium.Web.GraphQL.QueryTypes;

namespace Litium.Accelerator.GraphQL
{
    internal class QueryTypeBuilder : IGraphTypeBuilder<StorefrontQueryType>
    {
        public void Build(StorefrontQueryType target)
        {
            target.ResolveField<ContentInterfaceType, IContentModel, ContentResolver>("content")
                .Description("Content for the specified url.")
                .Argument<NonNullGraphType<StringGraphType>>("url", x => x.Description = "The url for the content.");

            target.ResolveField<CartType, CartModel, CartResolver>("cart")
                .Description("Cart for the provided Cart-Context-Id header.")
                .RequireCartContext();
        }
    }
}
