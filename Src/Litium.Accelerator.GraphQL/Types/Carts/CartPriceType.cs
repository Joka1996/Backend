using Litium.Accelerator.GraphQL.Models.Carts;
using Litium.Web.GraphQL;

namespace Litium.Accelerator.GraphQL.Types.Carts
{
    public class CartPriceType :  ExtendableObjectGraphType<CartPriceType, CartPriceModel>
    {
        public CartPriceType()
        {
            Interface<PriceInterfaceType>();
        }
    }
}
