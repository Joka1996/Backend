using GraphQL.Builders;
using GraphQL.Types;

namespace Litium.Web.GraphQL
{
    public static class CartContextExtensions
    {
        private static readonly string _requireCartContextKey = "REQUIRE_CART_CONTEXT";
        private static readonly CartContextRequirement _requireCartContextValue = new();

        public static bool RequiredCartContext(this IProvideMetadata type)
        {
            return type?.GetMetadata<CartContextRequirement>(_requireCartContextKey) is not null;
        }

        public static void RequireCartContext(this IProvideMetadata type)
        {
            if (!type.Metadata.ContainsKey(_requireCartContextKey))
            {
                type.Metadata[_requireCartContextKey] = _requireCartContextValue;
            }
        }

        public static FieldBuilder<TSourceType, TReturnType> RequireCartContext<TSourceType, TReturnType>(this FieldBuilder<TSourceType, TReturnType> builder)
        {
            builder.FieldType.RequireCartContext();
            return builder;
        }

        private class CartContextRequirement { }
    }
}
