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

    /// <summary>
    /// ShoppingCartStore represents an IShoppingCartStore backed by a SQL Server instance.
    /// </summary>
    public class ShoppingCartStore : IShoppingCartStore
    {
        // Note that we are hard-coding our database credentials here.
        private const string ConnectionString =
            "Data Source=localhost;Initial Catalog=ShoppingCart;User Id=SA;Password=yourStrong(!)Password";

        // Dapper expects you to write raw SQL queries.
        private const string ReadItemsSql =
            @"
select ShoppingCart.ID, ProductCatalogId, ProductName, ProductDescription, Currency, Amount 
from ShoppingCart, ShoppingCartItem
where ShoppingCartItem.ShoppingCartId = ShoppingCart.ID
and ShoppingCart.UserId=@UserId";

        public async Task<ShoppingCart> Get(int userId)
        {
            // Open a connection to our database.
            await using var conn = new SqlConnection(ConnectionString);
            var items = (await
                    conn.QueryAsync( // Dapper extends the IDbConnection interface.
                        ReadItemsSql, // Apply our query.
                        new { UserId = userId }))
                .ToList();
            return new ShoppingCart(
                items.FirstOrDefault()?.ID,
                userId,
                items.Select(x => // Convert our result set into a concrete type.
                    new ShoppingCartItem(
                        (int)x.ProductCatalogId,
                        x.ProductName,
                        x.ProductDescription,
                        new Money(x.Currency, x.Amount))));
        }

        private const string InsertShoppingCartSql =
            @"insert into ShoppingCart (UserId) OUTPUT inserted.ID VALUES (@UserId)";

        private const string DeleteAllForShoppingCartSql =
            @"delete item from ShoppingCartItem item
inner join ShoppingCart cart on item.ShoppingCartId = cart.ID
and cart.UserId=@UserId";

        private const string AddAllForShoppingCartSql =
            @"insert into ShoppingCartItem
(ShoppingCartId, ProductCatalogId, ProductName,
ProductDescription, Amount, Currency)
values
(@ShoppingCartId, @ProductCatalogueId, @ProductName,
@ProductDescription, @Amount, @Currency)";

        public async Task Save(ShoppingCart shoppingCart)
        {
            await using var conn = new SqlConnection(ConnectionString);
            await conn.OpenAsync();
            await using var tx = conn.BeginTransaction(); // This method is thread-safe?

            // If no ID was given, create a new row and return the generated ID.
            var shoppingCartId =
                shoppingCart.Id ??
                await conn.QuerySingleAsync<int>(InsertShoppingCartSql, new { shoppingCart.UserId }, tx);

            // Delete all existing items for this shopping cart.
            await conn.ExecuteAsync(
                DeleteAllForShoppingCartSql,
                new { UserId = shoppingCart.UserId },
                tx);

            // Save all current shopping cart items to the database.
            await conn.ExecuteAsync(
                AddAllForShoppingCartSql,
                shoppingCart.Items.Select(x =>
                    new
                    {
                        shoppingCartId,
                        x.ProductCatalogueId,
                        Productdescription = x.Description,
                        x.ProductName,
                        x.Price.Amount,
                        x.Price.Currency
                    }),
                tx);

            await tx.CommitAsync();
        }
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