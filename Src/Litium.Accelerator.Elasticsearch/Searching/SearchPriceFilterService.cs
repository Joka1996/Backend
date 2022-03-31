using System;
using System.Collections.Generic;
using System.Linq;
using Litium.Accelerator.Routing;
using Litium.Accelerator.Search;
using Litium.Account;
using Litium.Products.PriceCalculator;
using Litium.Runtime.DependencyInjection;
using Litium.Security;
using Nest;

namespace Litium.Accelerator.Searching
{
    [Service(ServiceType = typeof(SearchPriceFilterService), Lifetime = DependencyLifetime.Scoped)]
    internal class SearchPriceFilterService
    {
        private readonly RequestModelAccessor _requestModelAccessor;
        private readonly IPriceCalculator _priceCalculator;
        private readonly SecurityContextService _securityContextService;

        public SearchPriceFilterService(
            RequestModelAccessor requestModelAccessor,
            IPriceCalculator priceCalculator,
            SecurityContextService securityContextService)
        {
            _requestModelAccessor = requestModelAccessor;
            _priceCalculator = priceCalculator;
            _securityContextService = securityContextService;
        }

        public Container GetPrices()
        {
            return new Container(new Lazy<IEnumerable<Guid>>(GetPriceListForUser), new Lazy<IEnumerable<Guid>>(Enumerable.Empty<Guid>()));
        }

        public IEnumerable<Func<QueryContainerDescriptor<ProductDocument>, QueryContainer>> GetPriceFilterTags(
            SearchQuery searchQuery,
            Container container,
            Guid countrySystemId,
            bool filterForSorting = false)
        {
            if (searchQuery.ContainsPriceFilter())
            {
                foreach (var item in container.PriceLists)
                {
                    foreach (var priceItem in searchQuery.PriceRanges)
                    {
                        yield return q => q.Nested(n => n
                                .Path(x => x.Prices)
                                .Query(nq
                                    => nq.Term(t => t.Field(f => f.Prices[0].SystemId).Value(item))
                                    && nq.Term(t => t.Field(f => f.Prices[0].CountrySystemId).Value(countrySystemId))
                                    && nq.Term(t => t.Field(f => f.Prices[0].IsCampaignPrice).Value(false))
                                    && nq.Range(t => t.Field(f => f.Prices[0].Price)
                                    .GreaterThanOrEquals(priceItem.Item1)
                                    .LessThanOrEquals(priceItem.Item2))
                                )
                            );
                    }
                }

                foreach (var item in container.Campaigns)
                {
                    foreach (var priceItem in searchQuery.PriceRanges)
                    {
                        yield return q => q.Nested(n => n
                              .Path(x => x.Prices)
                              .Query(nq
                                  => nq.Term(t => t.Field(f => f.Prices[0].SystemId).Value(item))
                                  && nq.Term(t => t.Field(f => f.Prices[0].IsCampaignPrice).Value(true))
                                  && nq.Range(t => t.Field(f => f.Prices[0].Price)
                                    .GreaterThanOrEquals(priceItem.Item1)
                                    .LessThanOrEquals(priceItem.Item2))
                              )
                          );
                    }
                }
            }
            else if (filterForSorting)
            {
                foreach (var item in container.PriceLists)
                {
                    yield return q => q.Bool(b => b
                        .Must(nq
                            => nq.Term(t => t.Field(f => f.Prices[0].SystemId).Value(item))
                            && nq.Term(t => t.Field(f => f.Prices[0].CountrySystemId).Value(countrySystemId))
                            && nq.Term(t => t.Field(f => f.Prices[0].IsCampaignPrice).Value(false))
                        )
                    );
                }

                foreach (var item in container.Campaigns)
                {
                    yield return q => q.Bool(b => b
                        .Must(nq
                            => nq.Term(t => t.Field(f => f.Prices[0].SystemId).Value(item))
                            && nq.Term(t => t.Field(f => f.Prices[0].IsCampaignPrice).Value(true))
                        )
                    );
                }
            }
        }

        private List<Guid> GetPriceListForUser()
        {
            var currencySystemId = _requestModelAccessor.RequestModel.CountryModel.Country.CurrencySystemId;
            var priceCalculatorArgs = new PriceCalculatorArgs
            {
                ChannelSystemId = _requestModelAccessor.RequestModel.ChannelModel.SystemId,
                CurrencySystemId = currencySystemId,
                UserSystemId = _securityContextService.GetIdentityUserSystemId().GetValueOrDefault(),
                DateTimeUtc = _requestModelAccessor.RequestModel.DateTimeUtc,
                CountrySystemId = _requestModelAccessor.RequestModel.CountryModel?.SystemId ?? Guid.Empty
            };
            var result = _priceCalculator.GetPriceLists(priceCalculatorArgs).Select(x => x.SystemId);
            return result.Distinct().ToList();
        }

        public class Container
        {
            private readonly Lazy<IEnumerable<Guid>> _priceListAccessor;
            private readonly Lazy<IEnumerable<Guid>> _campaignsAccessor;

            internal Container(Lazy<IEnumerable<Guid>> priceListAccessor, Lazy<IEnumerable<Guid>> campaignsAccessor)
            {
                _priceListAccessor = priceListAccessor;
                _campaignsAccessor = campaignsAccessor;
            }

            public IEnumerable<Guid> PriceLists => _priceListAccessor.Value;
            public IEnumerable<Guid> Campaigns => _campaignsAccessor.Value;
        }
    }
}
