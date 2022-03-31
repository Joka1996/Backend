﻿using Litium.Events;
using Litium.Search.Indexing;
using Litium.Products.Events;
using Litium.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using Litium.Runtime;
using System.Threading;
using System.Threading.Tasks;

namespace Litium.Accelerator.Search.Indexing.Products
{
    public class ProductEventListener : IIndexQueueHandlerRegistration, IAsyncAutostart
    {
        private readonly EventBroker _eventBroker;
        private readonly IndexQueueService _indexQueueService;
        private readonly VariantService _variantService;
        private readonly CategoryService _categoryService;

        public ProductEventListener(
            EventBroker eventBroker,
            IndexQueueService indexQueueService,
            VariantService variantService,
            CategoryService categoryService)
        {
            _eventBroker = eventBroker;
            _indexQueueService = indexQueueService;
            _variantService = variantService;
            _categoryService = categoryService;
        }

        ValueTask IAsyncAutostart.StartAsync(CancellationToken cancellationToken)
        {
            // product modifications
            _eventBroker.Subscribe<BaseProductCreated>(x => UpdateBaseProduct(x.SystemId));
            _eventBroker.Subscribe<BaseProductUpdated>(x => UpdateBaseProduct(x.SystemId, deleteBeforeUpdate: x.FieldTemplateChanged));
            _eventBroker.Subscribe<BaseProductDeleted>(x => _indexQueueService.Enqueue(new IndexQueueItem<ProductDocument>(x.SystemId) { Action = IndexAction.Delete }));
            _eventBroker.Subscribe<VariantCreated>(x => UpdateBaseProduct(x.BaseProductSystemId));
            _eventBroker.Subscribe<VariantUpdated>(x =>
            {
                if (x.BaseProductSystemId != x.OriginalBaseProductSystemId)
                {
                    UpdateBaseProduct(x.BaseProductSystemId, deleteBeforeUpdate: true);
                    UpdateBaseProduct(x.OriginalBaseProductSystemId);
                }
                else
                {
                    UpdateBaseProduct(x.BaseProductSystemId);
                }
            });
            _eventBroker.Subscribe<VariantDeleted>(x => UpdateBaseProduct(x.BaseProductSystemId, deleteBeforeUpdate: true));

            // product to product list links
            _eventBroker.Subscribe<ProductListItemCreated>(x => UpdateBaseProduct(x.Item.BaseProductSystemId));
            _eventBroker.Subscribe<ProductListItemDeleted>(x => UpdateBaseProduct(x.Item.BaseProductSystemId));
            _eventBroker.Subscribe<ProductListItemUpdated>(x => UpdateBaseProduct(x.Item.BaseProductSystemId));

            // product to category links
            _eventBroker.Subscribe<BaseProductToCategoryLinkAdded>(x => UpdateBaseProduct(x.BaseProductSystemId));
            _eventBroker.Subscribe<BaseProductToCategoryLinkModified>(x => UpdateBaseProduct(x.BaseProductSystemId));
            _eventBroker.Subscribe<BaseProductToCategoryLinkRemoved>(x => UpdateBaseProduct(x.BaseProductSystemId));
            _eventBroker.Subscribe<VariantToCategoryLinkAdded>(x => UpdateBaseProduct(x.BaseProductSystemId));
            _eventBroker.Subscribe<VariantToCategoryLinkRemoved>(x => UpdateBaseProduct(x.BaseProductSystemId));

            //category structure
            _eventBroker.Subscribe<CategoryMoved>(x => UpdateProductsUnderCategory(x.CategorySystemId));
            _eventBroker.Subscribe<CategoryToChannelLinkAdded>(x => UpdateProductsUnderCategory(x.CategorySystemId));
            _eventBroker.Subscribe<CategoryToChannelLinkRemoved>(x => UpdateProductsUnderCategory(x.CategorySystemId));

            // price
            _eventBroker.Subscribe<PriceListItemCreated>(x => UpdateVariant(x.VariantSystemId));
            _eventBroker.Subscribe<PriceListItemUpdated>(x => UpdateVariant(x.VariantSystemId));
            _eventBroker.Subscribe<PriceListItemDeleted>(x => UpdateVariant(x.VariantSystemId));

            // product to channel links
            _eventBroker.Subscribe<VariantToChannelLinkAdded>(x => UpdateBaseProduct(x.BaseProductSystemId));
            _eventBroker.Subscribe<VariantToChannelLinkRemoved>(x => UpdateBaseProduct(x.BaseProductSystemId));

            return ValueTask.CompletedTask;
        }

        private void UpdateBaseProduct(Guid systemId, bool deleteBeforeUpdate = false)
        {
            if (deleteBeforeUpdate)
            {
                _indexQueueService.Enqueue(new IndexQueueItem<ProductDocument>(systemId) { Action = IndexAction.DeleteAndIndex });
                return;
            }
            _indexQueueService.Enqueue(new IndexQueueItem<ProductDocument>(systemId) { Action = IndexAction.Index });
        }

        private void UpdateProductsUnderCategory(Guid categorySystemId)
        {
            //parent heirarchy changes for all children.
            //raise updated for all product links, including all children and grand children.
            var category = _categoryService.Get(categorySystemId);

            if (category is null) return;

            foreach (var categoryToProductLink in category.ProductLinks)
            {
                UpdateBaseProduct(categoryToProductLink.BaseProductSystemId);
            }

            var childCategories = GetChildCategories(category);
            foreach (var categoryToProductLink in childCategories.SelectMany(childCategory => childCategory.ProductLinks))
            {
                UpdateBaseProduct(categoryToProductLink.BaseProductSystemId);
            }
        }

        private void UpdateVariant(Guid systemId)
        {
            var variant = _variantService.Get(systemId);
            if (variant != null)
            {
                UpdateBaseProduct(variant.BaseProductSystemId);
            }
        }

        private IEnumerable<Category> GetChildCategories(Category category)
        {
            var stack = new Stack<Category>();
            stack.Push(category);
            while (stack.Count > 0)
            {
                var next = stack.Pop();
                yield return next;
                foreach (var child in _categoryService.GetChildCategories(next.SystemId))
                {
                    stack.Push(child);
                }
            }
        }
    }
}
