using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace ApiGateway.Controllers
{
    public class ProductListController : Controller
    {
        private readonly HttpClient _productCatalogClient;
        private readonly HttpClient _shoppingCartClient;

        public ProductListController(IHttpClientFactory httpClientFactory)
        {
            _productCatalogClient = httpClientFactory.CreateClient("ProductCatalogClient");
            _shoppingCartClient = httpClientFactory.CreateClient("ShoppingCartClient");
        }

        public async Task<IActionResult> Index([FromQuery] int userId)
        {
            var products = await GetProductsFromCatalog();
            var cartProducts = await GetProductsFromCart(userId);
#if false
      var products = new[]
      {
        new Product(1, "T-shirt", "Really nice t-shirt"),
        new Product(2, "Hoodie", "The coolest hoodie ever"),
        new Product(3, "Jeans", "Perfect jeans"),
      };
#endif
            return View(new ProductListViewModel(
                products,
                cartProducts
            ));
        }

        private async Task<Product[]> GetProductsFromCart(int userId)
        {
            var response = await _shoppingCartClient.GetAsync($"/shoppingcart/{userId}");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStreamAsync();
            var cart =
                await JsonSerializer.DeserializeAsync<ShoppingCart>(content,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return cart.Items;
        }

        private async Task<Product[]> GetProductsFromCatalog()
        {
            var response = await _productCatalogClient.GetAsync("/products?productIds=1,2,3,4");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStreamAsync();
            var products =
                await JsonSerializer.DeserializeAsync<Product[]>(content,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return products;
        }
    }

    public record Product(int ProductCatalogueId, string ProductName, string Description);

    public record ShoppingCart(int UserId, Product[] Items);

    public record ProductListViewModel(Product[] Products, Product[] CartProducts);
}