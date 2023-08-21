using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Linq;
using Dapper;

namespace ShoppingCart.Models
{
    // IShoppingCartStore provides methods for working with shopping carts from a data store.
    public interface IShoppingCartStore
    {
        // Get returns the shopping cart for the given user, creating one if it doesn't exist.
        Task<ShoppingCart> Get(int userId);

        // Save saves the given shopping cart to the data store.
        Task Save(ShoppingCart shoppingCart);
    }

    // InmemShoppingCartStore provides an in-memory data store for shopping carts.
    public class InmemShoppingCartStore : IShoppingCartStore
    {
        private static readonly Dictionary<int, ShoppingCart> Database = new Dictionary<int, ShoppingCart>();

        public Task<ShoppingCart> Get(int userId) =>
            Task.FromResult(Database.ContainsKey(userId) ? Database[userId] : new ShoppingCart(userId));

        public Task Save(ShoppingCart shoppingCart)
        {
            Database[shoppingCart.UserId] = shoppingCart;
            return Task.CompletedTask;
        }
    }
}