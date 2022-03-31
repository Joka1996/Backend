using System;
using Litium.Accelerator.ViewModels.Order;
using Litium.Runtime.AutoMapper;
using Litium.Sales;
using Litium.Web.Models.Websites;

namespace Litium.Accelerator.Builders.Order
{
    public class OrderConfirmationViewModelBuilder : IViewModelBuilder<OrderConfirmationViewModel>
    {
        private readonly OrderViewModelBuilder _orderViewModelBuilder;
        private readonly OrderOverviewService _orderOverviewService;
        private readonly CartContextAccessor _cartContextAccessor;

        public OrderConfirmationViewModelBuilder(
            OrderViewModelBuilder orderViewModelBuilder,
            OrderOverviewService orderOverviewService,
            CartContextAccessor cartContextAccessor)
        {
            _orderViewModelBuilder = orderViewModelBuilder;
            _orderOverviewService = orderOverviewService;
            _cartContextAccessor = cartContextAccessor;
        }

        public OrderConfirmationViewModel Build(PageModel pageModel)
        {
            var model = pageModel.MapTo<OrderConfirmationViewModel>();
            var cartContext = _cartContextAccessor.CartContext;
            if (cartContext != null)
            {
                var order = _orderOverviewService.Get(cartContext.Cart.Order.SystemId);
                if (order != null)
                {
                    model.Order = _orderViewModelBuilder.Build(order);
                }
            }
            return model;
        }

        public OrderConfirmationViewModel Build(PageModel pageModel, Guid orderId)
        {
            var model = pageModel.MapTo<OrderConfirmationViewModel>();
            var order = _orderOverviewService.Get(orderId);
            if (order != null)
            {
                model.Order = _orderViewModelBuilder.Build(order);
            }
            return model;
        }
    }
}
