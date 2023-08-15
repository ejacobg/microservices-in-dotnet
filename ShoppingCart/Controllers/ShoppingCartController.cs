using Microsoft.AspNetCore.Mvc;

namespace ShoppingCart.Controllers
{
    [Route("/shoppingcart")] 
    public class ShoppingCartController : ControllerBase
    {
        private readonly IShoppingCartStore _shoppingCartStore;

        public ShoppingCartController(IShoppingCartStore shoppingCartStore)
        {
            _shoppingCartStore = shoppingCartStore;
        }

        // Objects (like ShoppingCart) will be serialized to JSON before being returned in the response.
        [HttpGet("{userId:int}")]
        public ShoppingCart Get(int userId) =>
            _shoppingCartStore.Get(userId);
    }
}