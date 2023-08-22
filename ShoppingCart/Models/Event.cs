using System;

namespace ShoppingCart.Models
{
    // Event represents a significant action taken on a shopping cart, like when items are added or removed.
    public record Event(long SequenceNumber, DateTimeOffset OccuredAt, string Name, object Content);

    // When using the SQL Server event feed, you need to be able to convert the returned string into type object, or else you'll get this exception:
    // System.InvalidOperationException: A parameterless default constructor or one matching signature (System.Int32 ID, System.String Name, System.DateTimeOffset OccurredAt, System.String Content) is required for ShoppingCart.Models.Event materialization
    // Won't be fixed for now.
}