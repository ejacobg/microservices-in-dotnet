using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace ApiGateway.Controllers
{
    public class ShoppingCartController : Controller
    {
        private readonly HttpClient _shoppingCartClient;

        public ShoppingCartController(IHttpClientFactory httpClientFactory)
        {
            _shoppingCartClient = httpClientFactory.CreateClient("ShoppingCartClient");
        }

        [HttpPost("/shoppingcart/{userId}")]
        public async Task<OkResult> AddToCart(int userId, [FromBody] int productId)
        {
            var response =
                await _shoppingCartClient.PostAsJsonAsync($"/shoppingcart/{userId}/items", new[] { productId });
            response.EnsureSuccessStatusCode();
            return Ok();
        }

        [HttpDelete("/shoppingcart/{userId}")]
        public async Task<OkResult> RemoveFromCart(int userId, [FromBody] int productId)
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, $"/shoppingcart/{userId}/items");
            request.Content = new StringContent(JsonSerializer.Serialize(new[] { productId }), Encoding.UTF8,
                "application/json");
            var response = await _shoppingCartClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            return Ok();
        }
    }
}