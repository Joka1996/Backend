using System;
using System.Globalization;
using Litium.Globalization;
using Litium.Runtime.DependencyInjection;
using Litium.Sales;

namespace Litium.Accelerator.GraphQL
{
    [Service(ServiceType = typeof(CartContextHelper), Lifetime = DependencyLifetime.Singleton)]
    public class CartContextHelper
    {
        private readonly ChannelService _channelService;
        private readonly LanguageService _languageService;

        public CartContextHelper(
            ChannelService channelService,
            LanguageService languageService)
        {
            _channelService = channelService;
            _languageService = languageService;
        }

        public void SetRequestContextFromCart(CartContext cartContext)
        {
            var channel = _channelService.Get(cartContext.ChannelSystemId.GetValueOrDefault());

            var websiteLanguage = _languageService.Get(channel.WebsiteLanguageSystemId ?? Guid.Empty);
            CultureInfo.CurrentUICulture = websiteLanguage?.CultureInfo ?? CultureInfo.InvariantCulture;

            var productLanguage = _languageService.Get(channel.ProductLanguageSystemId ?? Guid.Empty);
            CultureInfo.CurrentCulture = productLanguage?.CultureInfo ?? CultureInfo.InvariantCulture;
        }
    }
}
