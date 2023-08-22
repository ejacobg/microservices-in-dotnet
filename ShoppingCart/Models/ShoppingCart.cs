using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Features;

namespace ShoppingCart.Models
{
    // ShoppingCart represents a set of items owned by a particular user.
    public class ShoppingCart
    {
        // _items stores ShoppingCartItems by their ProductCatalogueId.
        private readonly HashSet<ShoppingCartItem> _items = new();

        public int? Id { get; }
        public int UserId { get; }
        public IEnumerable<ShoppingCartItem> Items => _items;

        public ShoppingCart(int userId) => UserId = userId;

        public ShoppingCart(int? id, int userId, IEnumerable<ShoppingCartItem> items)
        {
            Id = id;
            UserId = userId;
            _items = new HashSet<ShoppingCartItem>();
        }

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
                // For some reason, removal events don't seem to be added to the event stream when using EventStoreDB.
                eventStore.Raise("ShoppingCartItemRemoved", new {UserId, item});
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