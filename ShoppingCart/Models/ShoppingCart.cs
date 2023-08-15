using System.Collections.Generic;
using System.Linq;

namespace ShoppingCart.Models
{
    // ShoppingCart represents a set of items owned by a particular user.
    public class ShoppingCart
    {
        private readonly HashSet<ShoppingCartItem> _items = new();

        public int UserId { get; }
        public IEnumerable<ShoppingCartItem> Items => _items;

        public ShoppingCart(int userId) => UserId = userId;

        public void AddItems(IEnumerable<ShoppingCartItem> shoppingCartItems, IEventStore eventStore)
        {
            foreach (var item in shoppingCartItems)
                if (_items.Add(item))
                    eventStore.Raise("ShoppingCartItemAdded", new { UserId, item });
        }

        public void RemoveItems(int[] productCatalogueIds, IEventStore eventStore) =>
            _items.RemoveWhere(i => productCatalogueIds.Contains(i.ProductCatalogueId));
    }

    public record ShoppingCartItem(
        int ProductCatalogueId,
        string ProductName,
        string Description,
        Money Price)
    {
        public virtual bool Equals(ShoppingCartItem? obj) =>
            obj != null && ProductCatalogueId.Equals(obj.ProductCatalogueId);

        public override int GetHashCode() =>
            ProductCatalogueId.GetHashCode();
    }

    public record Money(string Currency, decimal Amount);
}