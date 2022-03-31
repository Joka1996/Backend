GraphQL Support
===============

This package contains GraphQL support for common operations. 

All code regarding GraphQL is experimental and may be changed or removed without notice.

## Endpoint

The `StorefrontSchema` that is the base definition of the schema that is exposed to the clients will
be reachable on the url `/storefront.graphql`.

## GraphQL playground

In the `Litium.Accelerator.GraphQL`-project the GraphQL playground is added to the application, 
this is done in the file `Runtime/EndpointRouteBuilder.cs` and should be removed or as conditional
used depend on the environment where the playground should be deployed into.

The url for the playground is `/ui/playground` and is pre-configured to fetch the `StorefrontSchema`.

## CORS

Using GraphQL cross domain require CORS, the cross access domain from where the frontend site is running 
need to be added as a domain name in globalization settings, 
domain validations is done without port number.

# Cart and checkout

## Cart-Context-Id http header

To handle the connection to the cart, the `Cart-Context-Id` should be included as http header, example
```json
{
  "Cart-Context-Id": "CfDJ8FMNnb7UndRFp7aihL1h-RtFVpUSuD03H4UfP4rxOzK4ObJZGyktzFidxW4-UcPfo5yMWgR7E6IFDtmW5C-ok9aCdybH_Px-9PmU6ZWlz-h-0HzAg8r0hq1uzkwxKt-UdVWfCPNG9QKrI-PFX6TGGZw"
}
```

If any request returning the `extensions["Cart-Context-Id"]` the value should be stored and reused as the header value

## To create a cart context

```gql
mutation CreateCartContext($url: String!) {
 cartCreateContext (url: $url) {
  	result {
      message
      success
    }
  }
}
```

Query variables
```json
{
  "url": "https://litium-vnext.localtest.me:5001/"  
}
```

## Add item to cart

```gql
mutation AddItemToCart($item: AddItemToCartType!) {
 cartAddItem (item: $item) {
  	result {
      message
      success
    }
  	data {
    	... cart
  	}
	}
}

fragment price on PriceInterfaceType {
  formattedTotalPrice
  formattedVatAmount
  totalPrice
  vatAmount
  vatRate
}

fragment cart on CartType {
    ...price
    rows {
      id
      articleNumber
      quantity
      formattedUnitPrice
      unitPrice
      ...price
    }
}
```

Query Variables
```json
{
  "item": {
    "articleNumber": "courtdress-l-001-637731002962774606"
  }
}
```

HTTP Headers
```json
{
  "Cart-Context-Id": "CfDJ8FMNnb7UndRFp7aihL1h-RtFVpUSuD03H4UfP4rxOzK4ObJZGyktzFidxW4-UcPfo5yMWgR7E6IFDtmW5C-ok9aCdybH_Px-9PmU6ZWlz-h-0HzAg8r0hq1uzkwxKt-UdVWfCPNG9QKrI-PFX6TGGZw"
}
```

## Remove item from cart

```gql
mutation RemoveItemFromCart($item: RemoveItemFromCartType!) {
 cartRemoveItem (item: $item) {
  	result {
      message
  		success
    }
  	data {
    	... cart
  	}
	}
}

fragment price on PriceInterfaceType {
  formattedTotalPrice
  formattedVatAmount
  totalPrice
  vatAmount
  vatRate
}

fragment cart on CartType {
    ...price
    items {
      id
      articleNumber
      description
      quantity
      formattedUnitPrice
      unitPrice
      ...price
    }
}
```

Query Variables
```json
{
  "item": {
    "id": "1"
  }
}
```

HTTP Headers
```json
{
  "Cart-Context-Id": "CfDJ8FMNnb7UndRFp7aihL1h-RtFVpUSuD03H4UfP4rxOzK4ObJZGyktzFidxW4-UcPfo5yMWgR7E6IFDtmW5C-ok9aCdybH_Px-9PmU6ZWlz-h-0HzAg8r0hq1uzkwxKt-UdVWfCPNG9QKrI-PFX6TGGZw"
}
```

## Get cart

``` gql
query GetCart {
  __typename
  cart {
    ...cart
    shippingOptionId
    paymentOptionId
    supportedPaymentOptions {
      ...cartOption
    }
    supportedShippingOptions {
      ...cartOption
    }
    paymentWidget {
      redirectUrl
      responseString
    }
  }
}

fragment price on PriceInterfaceType {
  formattedTotalPrice
  formattedVatAmount
  totalPrice
  vatAmount
  vatRate
}

fragment cart on CartType {
  ...price
	items {
    id
    articleNumber
    quantity
    description
    formattedUnitPrice
    unitPrice
    ...price
	}
}

fragment cartOption on CartOptionType {
  id
  description
  name
}
```

HTTP Headers
```json
{
  "Cart-Context-Id": "CfDJ8FMNnb7UndRFp7aihL1h-RtFVpUSuD03H4UfP4rxOzK4ObJZGyktzFidxW4-UcPfo5yMWgR7E6IFDtmW5C-ok9aCdybH_Px-9PmU6ZWlz-h-0HzAg8r0hq1uzkwxKt-UdVWfCPNG9QKrI-PFX6TGGZw"
}
```

## Set checkout options to the cart

```gql
mutation SetCheckoutOptions($item: CheckoutOptionsType!) {
 cartCheckoutOption (item: $item) {
  	result {
      message
  		success
    }
  	data {
      ...cart
      shippingOptionId
      paymentOptionId
      supportedPaymentOptions {
        ...cartOption
      }
      supportedShippingOptions {
        ...cartOption
      }
      paymentWidget {
        redirectUrl
        responseString
      }
    }
	}
}

fragment price on PriceInterfaceType {
  formattedTotalPrice
  totalPrice
  vatAmount
  vatRate
}

fragment cart on CartType {
    ...price
    items {
      id
      articleNumber
      quantity
      formattedUnitPrice
      unitPrice
      ...price
    }
}

fragment cartOption on CartOptionType {
  id
  description
  name
}
```

Query Variables
```json
{
  "item": {
    "paymentOptionId": "DirectPayment:DirectPay",
    "shippingOptionId": "DirectShipment:standardPackage"
  }
}
```

HTTP Headers
```json
{
  "Cart-Context-Id": "CfDJ8FMNnb7UndRFp7aihL1h-RtFVpUSuD03H4UfP4rxOzK4ObJZGyktzFidxW4-UcPfo5yMWgR7E6IFDtmW5C-ok9aCdybH_Px-9PmU6ZWlz-h-0HzAg8r0hq1uzkwxKt-UdVWfCPNG9QKrI-PFX6TGGZw"
}
```

## Place cart as an order

```gql
mutation PlaceOrder(
  $customer: CustomerDetailsType!
  $address: AddressType!
  $alternativeAddress: AddressType
  $notes: String) {
 cartPlaceOrder (
  customer: $customerDetails
  address: $address
  alternativeDeliveryAddress: $alternativeAddress
  notes: $notes) {
    result {
      message
      success
    }
    data
  }
}
```

Query variables
```json
{
  "customer": {
    "email": "customer@localtest.me",
    "firstName": "Customer",
    "lastName": "Name"
  },
  "address": {
    "address1": "Street 33",
    "city": "City",
    "country": "Sverige",
    "zipCode": "12311",
    "phoneNumber": "070-000 00 00"
  }  
}
```

Http Headers
```json
{
    "Cart-Context-Id": "CfDJ8FMNnb7UndRFp7aihL1h-RsgoGkxVNyM-PmtaEODRbBKWZqNCWCqkK6sy3378DCOyXGhR-lsJlQJBZcWMdTQJlug9qSGIFPfojDi_Kgm9HSSKdeeEDyXMyBvNxrcyq-ypICDLW2gxoYe_M3fxVl3Uu8"
  }
```

# Get content

Content can be anything from pages, category pages and products. 
For each content type an resolver need to be added, they are implemented by the interface `IPageTemplateResolver`, `ICategoryTemplateResolver` or `IProductTemplateResolver` in namespace `Litium.Accelerator.GraphQL.Queries.Contents`. 
All of them are registered as named services and the name is from the following locations:
  - categories and products, the display template id is used
  - pages and blocks, the field template id is used

To return the correct calculated prices the `Cart-Context-Id`-header should be included in the request.

```gql
query GetContent($url: String!) {
  content(url: $url) {
    __typename

    ... on RedirectContentType {
      redirect
    }
    
    ... on TemplateAwareContentType {
      id
      template
      metaTitle
      metaDescription
      framework {
        footerNavigation
        headerNavigation
        primaryNavigation
        favIcons
      }
    }
    
    ... on CategoryContentType {
      metaTitle
      metaDescription
      products {
        totalProducts
        items {
          ...productListItem
        }
      }
      navigation {
        ...subNavigation
      }
      facetFilters {
        ...facetFilterItems
      }
      sort {
        enabled
        name
        url
      }
    }
    ...productContent
  }
}

fragment facetFilterItems on FacetGroupFilterType {
  id
  label
  selectedOptions
  singleSelect
  options {
    id
    label
    quantity
  }
}

fragment subNavigationItems on SubNavigationLinkType {
  isSelected
  name
  url
}

fragment subNavigation on SubNavigationLinkType {
  ...subNavigationItems
  links {
    ...subNavigationItems
    links {
      ...subNavigationItems
      links {
        ...subNavigationItems
        links {
          ...subNavigationItems
          links {
            ...subNavigationItems
            links {
              ...subNavigationItems
            }
          }
        }
      }
    }
  }
}

fragment image on ImageModelType {
  dimension { 
    height
    width
  }
  url
}

fragment filterItem on ProductFilterType {
  isActive
  value
  enabled
  name
  url
}

fragment productListItem on ProductItemType {
  name
  id
  brand
  color
  description
  isInStock
  showBuyButton
  showQuantityField
  stockStatusDescription
  url
  price {
    currency
    formattedCampaignPrice
    campaignPriceIncludingVat
    campaignPriceExcludingVat
    formattedPrice
    unitPriceIncludingVat
    unitPriceExcludingVat
    vatRate
  }
  images(max: {height: 200, width: 200}) {
    ...image
  }
}

fragment productContent on ContentInterfaceType {
  ... on ProductContentInterfaceType {
    popularProducts { 
      ...productListItem 
    }
    productItem {
      ...productListItem
      large_image:images {
        ...image
      }
    }
    similarProducts {
      ...productListItem
    }
  }

  ... on ProductWithVariantsContentType {
    filter1Items {
      ...filterItem
    }
    filter1Text
    filter2Items {
      ...filterItem
    }
    filter2Text
  }

  ... on ProductWithVariantListContentType {
    variants {
      ...productListItem
    }
  }
}
```

Query variables
```json
{
  "url": "https://litium-vnext.localtest.me:5001/"  
}
```

Http Headers
```json
{
    "Cart-Context-Id": "CfDJ8FMNnb7UndRFp7aihL1h-RsgoGkxVNyM-PmtaEODRbBKWZqNCWCqkK6sy3378DCOyXGhR-lsJlQJBZcWMdTQJlug9qSGIFPfojDi_Kgm9HSSKdeeEDyXMyBvNxrcyq-ypICDLW2gxoYe_M3fxVl3Uu8"
  }
```