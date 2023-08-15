using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ShoppingCart.Models;

namespace ShoppingCart.Controllers
{
    [Route("/shoppingcart")]
    public class ShoppingCartController : ControllerBase
    {
        private readonly IShoppingCartStore _shoppingCartStore;
        private readonly IProductCatalogClient _productCatalogClient;
        private readonly IEventStore _eventStore;

        public ShoppingCartController(IShoppingCartStore shoppingCartStore, IProductCatalogClient productCatalogClient,
            IEventStore eventStore)
        {
            _shoppingCartStore = shoppingCartStore;
            _productCatalogClient = productCatalogClient;
            _eventStore = eventStore;
        }

        // Objects (like ShoppingCart) will be serialized to JSON before being returned in the response.
        [HttpGet("{userId:int}")]
        public Models.ShoppingCart Get(int userId) =>
            _shoppingCartStore.Get(userId);

        [HttpPost("{userId:int}/items")]
        public async Task<Models.ShoppingCart> Post(
            int userId,
            [FromBody] int[] productIds) // Automatically deserialize the request body into an array.
        {
            var shoppingCart = _shoppingCartStore.Get(userId);
            var shoppingCartItems =
                await _productCatalogClient
                    .GetShoppingCartItems(productIds);
            shoppingCart.AddItems(shoppingCartItems, _eventStore);
            _shoppingCartStore.Save(shoppingCart);
            return shoppingCart;
        }
        
        [HttpDelete("{userid:int}/items")]
        public Models.ShoppingCart Delete(int userId, [FromBody] int[] productIds)
        {
            var shoppingCart = _shoppingCartStore.Get(userId);
            shoppingCart.RemoveItems(productIds, _eventStore);
            _shoppingCartStore.Save(shoppingCart);
            return shoppingCart;
        }
    }
}