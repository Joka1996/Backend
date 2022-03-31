using System;
using System.Threading.Tasks;
using GraphQL;
using Litium.Accelerator.Constants;
using Litium.Accelerator.GraphQL.Models.Carts;
using Litium.FieldFramework.FieldTypes;
using Litium.Globalization;
using Litium.Sales;
using Litium.Web;
using Litium.Web.GraphQL;
using Litium.Web.GraphQL.MutationTypes;
using Litium.Websites;

namespace Litium.Accelerator.GraphQL.Mutations.Carts
{
    internal class PlaceOrderMutationResolver : IFieldResolver<MutationResponse<string>>
    {
        private readonly CartContextAccessor _cartContextAccessor;
        private readonly WebsiteService _websiteService;
        private readonly ChannelService _channelService;
        private readonly PageService _pageService;
        private readonly UrlService _urlService;

        public PlaceOrderMutationResolver(
            CartContextAccessor cartContextAccessor,
            WebsiteService websiteService,
            ChannelService channelService,
            PageService pageService,
            UrlService urlService)
        {
            _cartContextAccessor = cartContextAccessor;
            _websiteService = websiteService;
            _channelService = channelService;
            _pageService = pageService;
            _urlService = urlService;
        }

        public async Task<MutationResponse<string>> ResolveAsync(IResolveFieldContext context)
        {
            var customerDetails = context.Arguments["customer"].Value as CustomerDetailsModel;
            var address = context.Arguments["address"].Value as AddressInfoModel;
            var alternativeDeliveryAddress = context.Arguments["alternativeDeliveryAddress"].Value as AddressInfoModel
                ?? address;
            var notes = context.Arguments["notes"].Value as string;

            var cartContext = _cartContextAccessor.CartContext;
            await cartContext.AddOrUpdateCustomerInfoAsync(new()
            {
                CustomerInfo = new CustomerInfo
                {
                    CustomerNumber = cartContext.Cart.Order.CustomerInfo?.CustomerNumber,
                    CustomerType = cartContext.Cart.Order.CustomerInfo?.CustomerType ?? CustomerType.Person,
                    PersonSystemId = cartContext.PersonSystemId,
                    OrganizationSystemId = cartContext.OrganizationSystemId,
                    FirstName = customerDetails.FirstName,
                    LastName = customerDetails.LastName,
                    Email = customerDetails.Email,
                    Phone = address.PhoneNumber
                }
            }, context.CancellationToken);
            await cartContext.AddOrUpdateBillingAddressAsync(GetAddress(address), context.CancellationToken);
            await cartContext.AddOrUpdateDeliveryAddressAsync(GetAddress(alternativeDeliveryAddress), context.CancellationToken);
            await cartContext.UpdateOrderCommentAsync(new()
            {
                Comments = notes
            }, context.CancellationToken);
            await cartContext.PlaceOrderAsync(new(), context.CancellationToken);

            return new()
            {
                Result = new()
                {
                    Success = true
                },
                Data = GetOrderConfirmationPageUrl(cartContext.Cart.Order)
            };

            Address GetAddress(AddressInfoModel address)
            {
                return new()
                {
                    Address1 = address.Address1,
                    Address2 = address.Address2,
                    CareOf = address.CareOf,
                    City = address.City,
                    Country = address.Country,
                    Email = customerDetails.Email,
                    FirstName = customerDetails.FirstName,
                    LastName = customerDetails.LastName,
                    ZipCode = address.ZipCode,
                };
            }

            string GetOrderConfirmationPageUrl(SalesOrder order)
            {
                var channel = _channelService.Get(order.ChannelSystemId.GetValueOrDefault());
                var website = _websiteService.Get(channel.WebsiteSystemId.GetValueOrDefault());
                var pointerPage = website.Fields.GetValue<PointerPageItem>(AcceleratorWebsiteFieldNameConstants.OrderConfirmationPage);

                var channelSystemId = pointerPage.ChannelSystemId != Guid.Empty ? pointerPage.ChannelSystemId : order.ChannelSystemId.Value;
                var url = _urlService.GetUrl(_pageService.Get(pointerPage.EntitySystemId), new PageUrlArgs(channelSystemId));

                return $"{url}?orderId={order.SystemId}";
            }
        }
    }
}
