using System;

namespace ShoppingCart.Models
{
    // Event represents a significant action taken on a shopping cart, like when items are added or removed.
    public record Event(long SequenceNumber, DateTimeOffset OccuredAt, string Name, object Content);
}