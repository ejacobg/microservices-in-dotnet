using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http.Features;

namespace ShoppingCart.Models
{
    // ShoppingCart represents a set of items owned by a particular user.
    public class ShoppingCart
    {
        // _items stores ShoppingCartItems by their ProductCatalogueId.
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

        public void RemoveItems(int[] productCatalogueIds, IEventStore eventStore)
        {
            foreach (var item in _items.Where(i => productCatalogueIds.Contains(i.ProductCatalogueId)))
            {
                eventStore.Raise("ShoppingCartItemRemoved", new {UserId, shoppingCartItem = item});
                _items.Remove(item);
            }
        }
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