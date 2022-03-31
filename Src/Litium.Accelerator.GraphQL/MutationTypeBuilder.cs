using GraphQL.Types;
using Litium.Accelerator.GraphQL.Mutations.Carts;
using Litium.Accelerator.GraphQL.Queries.Carts;
using Litium.Accelerator.GraphQL.Types.Carts;
using Litium.Accelerator.GraphQL.Models.Carts;
using Litium.Web.GraphQL;
using Litium.Web.GraphQL.MutationTypes;

namespace Litium.Accelerator.GraphQL
{
    internal class MutationTypeBuilder : IGraphTypeBuilder<StorefrontMutationType>
    {
        public void Build(StorefrontMutationType target)
        {
            target.ResolveField<MutationResponseGraphType, MutationResponse, CreateCartContextMutationResolver>("cartCreate")
                .Argument<NonNullGraphType<StringGraphType>>("url", x => x.Description = "The url for the content.");

            target.ResolveField<MutationResponseGraphType<CartType, CartModel, CartResolver>, MutationResponse, AddItemToCartMutationResolver>("cartAddItem")
                .Argument<NonNullGraphType<AddItemToCartType>>("item", x => x.Description = "The item to add to the cart.")
                .RequireCartContext();

            target.ResolveField<MutationResponseGraphType<CartType, CartModel, CartResolver>, MutationResponse, RemoveItemFromCartMutationResolver>("cartRemoveItem")
                .Argument<NonNullGraphType<RemoveItemFromCartType>>("item", x => x.Description = "The item to remove from the cart.")
                .RequireCartContext();

            target.ResolveField<MutationResponseGraphType<CartType, CartModel, CartResolver>, MutationResponse, UpdateItemInCartMutationResolver>("cartUpdateItem")
                .Argument<NonNullGraphType<UpdateItemInCartType>>("item", x => x.Description = "The item to update in the cart.")
                .RequireCartContext();

            target.ResolveField<MutationResponseGraphType<CartType, CartModel, CartResolver>, MutationResponse, CheckoutOptionsMutationResolver>("cartCheckoutOption")
                .Argument<NonNullGraphType<CheckoutOptionsType>>("item", x => x.Description = "The checkout options to use in the cart.")
                .RequireCartContext();

            target.ResolveField<MutationResponseGraphType<StringGraphType, string>, MutationResponse<string>, PlaceOrderMutationResolver>("cartPlaceOrder")
                .Argument<NonNullGraphType<CustomerDetailsType>>("customer", x => x.Description = "The customer details.")
                .Argument<NonNullGraphType<AddressInfoType>>("address", x => x.Description = "The address.")
                .Argument<AddressInfoType>("alternativeDeliveryAddress", x => x.Description = "The alternative delivery address.")
                .Argument<StringGraphType>("notes", x => x.Description = "The notes attached to the order.")
                .RequireCartContext();
        }
    }
}
