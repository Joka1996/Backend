using System.Threading.Tasks;
using GraphQL;
using Litium.Accelerator.GraphQL.Models.Carts;
using Litium.Sales;
using Litium.Web.GraphQL;
using Litium.Web.GraphQL.MutationTypes;

namespace Litium.Accelerator.GraphQL.Mutations.Carts
{
    internal class AddItemToCartMutationResolver : IFieldResolver<MutationResponse>
    {
        private readonly CartContextAccessor _cartContextAccessor;
        private readonly CartContextHelper _cartContextHelper;

        public AddItemToCartMutationResolver(
            CartContextAccessor cartContextAccessor,
            CartContextHelper cartContextHelper)
        {
            _cartContextAccessor = cartContextAccessor;
            _cartContextHelper = cartContextHelper;
        }

        public async Task<MutationResponse> ResolveAsync(IResolveFieldContext context)
        {
            var cartContext = _cartContextAccessor.CartContext;
            var arg = context.Arguments["item"].Value as AddItemToCartModel;

            _cartContextHelper.SetRequestContextFromCart(cartContext);

            await cartContext.AddOrUpdateItemAsync(new()
            {
                ArticleNumber = arg.ArticleNumber,
                Quantity = arg.Quantity ?? 1,
            });

            return new()
            {
                Result = new()
                {
                    Success = true
                },
            };
        }
    }
}
