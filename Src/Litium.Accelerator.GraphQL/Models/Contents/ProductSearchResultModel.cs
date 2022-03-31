using System.Collections.Generic;
using System.Text.Json.Serialization;
using Litium.Accelerator.ViewModels.Search;

namespace Litium.Accelerator.GraphQL.Models.Contents
{
    public class ProductSearchResultModel
    {
        public IEnumerable<ProductItemModel> Items { get; set; }
        public long TotalProducts { get; set; }

        [JsonIgnore]
        public IEnumerable<ProductSearchResult> ItemsSource { get; set; }
    }
}
