using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

namespace ShoppingCart.Models
{
    // IProductCatalogClient provides methods for working with an external service that holds detailed product information.
    public interface IProductCatalogClient
    {
        // GetShoppingCartItems takes an array of product IDs and returns the detailed information for each one.
        Task<IEnumerable<ShoppingCartItem>> GetShoppingCartItems(int[] productCatalogIds);
    }

    public class ProductCatalogClient : IProductCatalogClient
    {
        private readonly HttpClient _client;

        private static readonly string
            ProductCatalogBaseUrl =
                @"https://git.io/JeHiE"; // Not a real service, just points to a hardcoded JSON file.

        private static readonly string GetProductPathTemplate = "?productIds=[{0}]"; // eg. ?productIds=[1,2]

        public ProductCatalogClient(HttpClient client)
        {
            client.BaseAddress =
                new Uri(ProductCatalogBaseUrl); // Configure the client to make requests to the catalog service.
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json")); // Accept JSON responses.
            _client = client;
        }

        public async Task<IEnumerable<ShoppingCartItem>> GetShoppingCartItems(int[] productCatalogIds)
        {
            using var response = await RequestProductFromProductCatalog(productCatalogIds);
            return await ConvertToShoppingCartItems(response);
        }

        private async Task<HttpResponseMessage> RequestProductFromProductCatalog(int[] productCatalogIds)
        {
            var productsResource = string.Format(GetProductPathTemplate, string.Join(",", productCatalogIds));
            return await _client.GetAsync(productsResource);
        }

        private static async Task<IEnumerable<ShoppingCartItem>> ConvertToShoppingCartItems(
            HttpResponseMessage response)
        {
            response.EnsureSuccessStatusCode();

            // Deserialize the response into a list of product catalog objects.
            var products = await JsonSerializer.DeserializeAsync<List<ProductCatalogProduct>>(
                               await response.Content.ReadAsStreamAsync(),
                               new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                           ?? new();

            // Convert the product catalog objects into shopping cart items.
            return products
                .Select(p =>
                    new ShoppingCartItem(
                        p.ProductId,
                        p.ProductName,
                        p.ProductDescription,
                        p.Price
                    ));
        }

        // Too avoid too much tight coupling, only the ProductCatalogClient knows the shape of the response returned by the product catalog.
        // The product catalog actually returns more data than what is described here, however these are the fields that this service will actually use.
        private record ProductCatalogProduct(int ProductId, string ProductName, string ProductDescription, Money Price);
    }
}