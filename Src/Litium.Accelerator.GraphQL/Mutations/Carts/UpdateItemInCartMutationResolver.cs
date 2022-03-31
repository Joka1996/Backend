using System.Threading.Tasks;
using GraphQL;
using Litium.Accelerator.GraphQL.Models.Carts;
using Litium.Sales;
using Litium.Web.GraphQL;
using Litium.Web.GraphQL.MutationTypes;

namespace Litium.Accelerator.GraphQL.Mutations.Carts
{
    internal class UpdateItemInCartMutationResolver : IFieldResolver<MutationResponse>
    {
        private readonly CartContextAccessor _cartContextAccessor;

        public UpdateItemInCartMutationResolver(
            CartContextAccessor cartContextAccessor)
        {
            _cartContextAccessor = cartContextAccessor;
        }

        public async Task<MutationResponse> ResolveAsync(IResolveFieldContext context)
        {
            var cart = _cartContextAccessor.CartContext;
            var arg = context.Arguments["item"].Value as UpdateItemInCartModel;
            await cart.AddOrUpdateItemAsync(new()
            {
                RowSelector = (o, or) => or.Id == arg.Id,
                Quantity = arg.Quantity ?? 1,
                ConstantQuantity = true,
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
