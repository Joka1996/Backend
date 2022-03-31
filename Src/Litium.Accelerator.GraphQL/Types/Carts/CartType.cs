using System.Collections.Generic;
using GraphQL.Types;
using Litium.Accelerator.GraphQL.Queries.Carts;
using Litium.Accelerator.GraphQL.Models.Carts;
using Litium.Web.GraphQL;

namespace Litium.Accelerator.GraphQL.Types.Carts
{
    internal class CartType : ExtendableObjectGraphType<CartType, CartModel>
    {
        public CartType()
        {
            ResolveField<ListGraphType<CartItemType>, IEnumerable<CartItemModel>, CartItemsResolver>(x => x.Items)
                .Description("Cart item rows.");

            Field(x => x.PaymentOptionId, nullable: true)
                .Description("Selected payment option id.");

            ResolveField<ListGraphType<CartOptionType>, IEnumerable<OptionItemModel>, SupportedPaymnetOptionsResolver>(x => x.SupportedPaymentOptions)
                .Description("Supported payment options.");

            ResolveField<PaymentWidgetType, PaymentWidgetModel, PaymnetWidgetResolver>(x => x.PaymentWidget)
                .Description("Payment widget result.");

            Field(x => x.ShippingOptionId, nullable: true)
                .Description("Selected shipping option id.");

            ResolveField<ListGraphType<CartOptionType>, IEnumerable<OptionItemModel>, SupportedShipmentOptionsResolver>(x => x.SupportedShippingOptions)
                .Description("Suppoerted shipping options.");

            Interface<PriceInterfaceType>();
        }
    }
}
