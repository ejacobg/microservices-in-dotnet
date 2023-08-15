using System;

namespace ShoppingCart.Models
{
    public record Event(long SequenceNumber, DateTimeOffset OccuredAt, string Name, object Content);
}