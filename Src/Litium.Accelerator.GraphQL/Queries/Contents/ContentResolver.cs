using System;
using System.Threading.Tasks;
using GraphQL;
using Litium.Accelerator.GraphQL.Runtime;
using Litium.Accelerator.GraphQL.Models.Contents;
using Litium.Runtime.DependencyInjection;
using Litium.Web.GraphQL;
using Litium.Web.Products.Routing;
using Litium.Web.Routing;
using static Litium.Web.Routing.IChannelResolver;
using System.Threading;
using Litium.Globalization;

namespace Litium.Accelerator.GraphQL.Queries.Contents
{
    internal class ContentResolver : IFieldResolver<IContentModel>
    {
        private readonly RouteRequestResolver _requestUrlResolver;
        private readonly IPageNotFoundResolver _pageNotFoundResolver;
        private readonly IUrlRedirectResolver _urlRedirectResolver;
        private readonly RouteRequestInfoAccessor _routeRequestInfoAccessor;
        private readonly RouteRequestLookupInfoAccessor _routeRequestLookupInfoAccessor;
        private readonly RequestModelService _requestModelService;
        private readonly LanguageService _languageService;

        public ContentResolver(
            RouteRequestResolver requestUrlResolver,
            IPageNotFoundResolver pageNotFoundResolver,
            IUrlRedirectResolver urlRedirectResolver,
            RouteRequestInfoAccessor routeRequestInfoAccessor,
            RouteRequestLookupInfoAccessor routeRequestLookupInfoAccessor,
            RequestModelService requestModelService,
            LanguageService languageService)
        {
            _requestUrlResolver = requestUrlResolver;
            _pageNotFoundResolver = pageNotFoundResolver;
            _urlRedirectResolver = urlRedirectResolver;
            _routeRequestInfoAccessor = routeRequestInfoAccessor;
            _routeRequestLookupInfoAccessor = routeRequestLookupInfoAccessor;
            _requestModelService = requestModelService;
            _languageService = languageService;
        }

        public async Task<IContentModel> ResolveAsync(IResolveFieldContext context)
        {
            var urlString = context.GetArgument<string>("url");
            if (string.IsNullOrEmpty(urlString)
                || !Uri.TryCreate(urlString, UriKind.RelativeOrAbsolute, out var url))
            {
                context.Errors.Add(new("Url argument is missing."));
                return default;
            }

            if (!_requestUrlResolver.TryGet(new LookupInfo
            {
                Scheme = url.Scheme,
                Host = url.Port > 0 ? new(url.Host, url.Port) : new(url.Host),
                Path = new(url.LocalPath),
                Query = new QueryCollection(url),
            }, out var routeRequestLookupInfo, out var routeRequestInfo))
            {
                // find url redirect
                if (!_urlRedirectResolver.TryGet(routeRequestLookupInfo, out routeRequestInfo))
                {
                    context.Errors.Add(new("No page or redirect is found."));

                    // find the page not found page if that exists and return the data instead.
                    if (!_pageNotFoundResolver.TryGet(routeRequestLookupInfo, out routeRequestInfo))
                    {
                        return default;
                    }
                }
            }

            if (routeRequestInfo?.IsRedirect == true)
            {
                return new RedirectContentModel
                {
                    Redirect = routeRequestInfo.RedirectPermanent ?? routeRequestInfo.RedirectTemporary,
                    Permanent = !string.IsNullOrEmpty(routeRequestInfo.RedirectPermanent),
                };
            }

            var channel = routeRequestLookupInfo.Channel;
            var language = _languageService.Get(channel.WebsiteLanguageSystemId.GetValueOrDefault());
            void SetCultureInfo()
            {
                Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture = language?.CultureInfo;
                if (channel.ProductLanguageSystemId != null)
                {
                    var assortmentCulture = _languageService.Get(channel.ProductLanguageSystemId.GetValueOrDefault())?.CultureInfo;
                    if (assortmentCulture != null)
                    {
                        Thread.CurrentThread.CurrentCulture = assortmentCulture;
                    }
                }
            }
            SetCultureInfo();
            context.BeforeExecuteFieldResolver(SetCultureInfo);

            IFieldResolver<IContentModel> resolver;
            if (routeRequestInfo.Data is ProductPageData productData)
            {
                if (productData.BaseProductSystemId is not null)
                {
                    resolver = context.RequestServices.GetNamedService<IProductTemplateResolver<IContentModel>>(routeRequestInfo.TemplateId);
                    if (resolver is null)
                    {
                        context.Errors.Add(new($"Resolver for product template '{routeRequestInfo.TemplateId}' is missing."));
                        return default;
                    }
                }
                else
                {
                    resolver = context.RequestServices.GetNamedService<ICategoryTemplateResolver<IContentModel>>(routeRequestInfo.TemplateId);
                    if (resolver is null)
                    {
                        context.Errors.Add(new($"Resolver for category template '{routeRequestInfo.TemplateId}' is missing."));
                        return default;
                    }
                }
            }
            else
            {
                resolver = context.RequestServices.GetNamedService<IPageTemplateResolver<IContentModel>>(routeRequestInfo.TemplateId);
                if (resolver is null)
                {
                    context.Errors.Add(new($"Resolver for page template '{routeRequestInfo.TemplateId}' is missing."));
                    return default;
                }
            }

            _routeRequestInfoAccessor.RouteRequestInfo = routeRequestInfo;
            _routeRequestLookupInfoAccessor.RouteRequestLookupInfo = routeRequestLookupInfo;
            _requestModelService.Assign();

            return await resolver.ResolveAsync(context);
        }
    }
}
