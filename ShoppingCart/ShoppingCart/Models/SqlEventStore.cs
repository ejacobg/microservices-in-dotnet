using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text.Json;
using System.Threading.Tasks;
using Dapper;

namespace ShoppingCart.Models
{
    /// <summary>
    /// SqlEventStore represents an IEventStore backed by a SQL Server database. This implementation IS NOT thread-safe.
    /// </summary>
    public class SqlEventStore : IEventStore
    {
        private const string ConnectionString = @"Data Source=localhost;Initial Catalog=ShoppingCart;
User Id=SA; Password=yourStrong(!)Password";

        private const string WriteEventSql =
            @"insert into EventStore(Name, OccurredAt, Content) values (@Name, @OccurredAt, @Content)";

        public async Task Raise(string eventName, object content)
        {
            // Context objects can take any shape. We will store them as JSON in our database.
            var jsonContent = JsonSerializer.Serialize(content);

            await using var conn = new SqlConnection(ConnectionString);

            // Add this event to the database.
            await conn.ExecuteAsync(
                WriteEventSql,
                new
                {
                    Name = eventName,
                    OccurredAt = DateTimeOffset.Now,
                    Content = jsonContent
                });
        }

        // There is no guarantee that events will be ordered?
        private const string ReadEventsSql =
            @"select * from EventStore where ID >= @Start and ID <= @End";

        public async Task<IEnumerable<Event>> GetEvents(
            long firstEventSequenceNumber,
            long lastEventSequenceNumber)
        {
            await using var conn = new SqlConnection(ConnectionString);
            return await conn.QueryAsync<Event>(
                ReadEventsSql,
                new
                {
                    Start = firstEventSequenceNumber,
                    End = lastEventSequenceNumber
                });
        }
    }
}