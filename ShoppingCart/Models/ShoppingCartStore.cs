using System.Collections.Generic;

namespace ShoppingCart.Models
{
    // IShoppingCartStore provides methods for working with shopping carts from a data store.
    public interface IShoppingCartStore
    {
        // Get returns the shopping cart for the given user, creating one if it doesn't exist.
        Models.ShoppingCart Get(int userId);
        
        // Save saves the given shopping cart to the data store.
        void Save(Models.ShoppingCart shoppingCart);
    }

    // ShoppingCartStore provides an in-memory data store for shopping carts.
    public class ShoppingCartStore : IShoppingCartStore
    {
        private static readonly Dictionary<int, Models.ShoppingCart> Database = new Dictionary<int, Models.ShoppingCart>();

        public Models.ShoppingCart Get(int userId) =>
            Database.ContainsKey(userId) ? Database[userId] : new Models.ShoppingCart(userId);

        public void Save(Models.ShoppingCart shoppingCart) =>
            Database[shoppingCart.UserId] = shoppingCart;
    }
}