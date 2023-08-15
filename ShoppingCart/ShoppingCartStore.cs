using System.Collections.Generic;

namespace ShoppingCart
{
    // IShoppingCartStore provides methods for working with shopping carts from a data store.
    public interface IShoppingCartStore
    {
        // Get returns the shopping cart for the given user, creating one if it doesn't exist.
        ShoppingCart Get(int userId);
        
        // Save saves the given shopping cart to the data store.
        void Save(ShoppingCart shoppingCart);
    }

    // ShoppingCartStore provides an in-memory data store for shopping carts.
    public class ShoppingCartStore : IShoppingCartStore
    {
        private static readonly Dictionary<int, ShoppingCart> Database = new Dictionary<int, ShoppingCart>();

        public ShoppingCart Get(int userId) =>
            Database.ContainsKey(userId) ? Database[userId] : new ShoppingCart(userId);

        public void Save(ShoppingCart shoppingCart) =>
            Database[shoppingCart.UserId] = shoppingCart;
    }
}