using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using EventStore.ClientAPI;

namespace ShoppingCart.Models
{
    /// <summary>
    /// EsEventStore represents an IEventStore backed by an EventStoreDB instance.
    /// </summary>
    public class EsEventStore : IEventStore
    {
        private const string ConnectionString =
            "tcp://admin:changeit@localhost:1113";

        public async Task<IEnumerable<Event>> GetEvents(long firstEventSequenceNumber, long lastEventSequenceNumber)
        {
            // Same boilerplate as in Raise().
            using var connection = EventStoreConnection.Create(
                ConnectionSettings.Create().DisableTls().Build(),
                new Uri(ConnectionString));

            await connection.ConnectAsync();
            var result = await connection.ReadStreamEventsForwardAsync( // Read events from EventStoreDB.
                "ShoppingCart",
                start: firstEventSequenceNumber,
                count: (int)(lastEventSequenceNumber - firstEventSequenceNumber), // Bounds go from [first, last)?
                resolveLinkTos: false);

            return result.Events
                .Select(e =>
                    new
                    {
                        Content = Encoding.UTF8.GetString(e.Event.Data),
                        Metadata = JsonSerializer.Deserialize<EventMetadata>(Encoding.UTF8.GetString(e.Event.Metadata),
                            new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!
                    })
                .Select((e, i) =>
                    new Event(
                        i + firstEventSequenceNumber, // Events will be in order, so this is safe to do.
                        e.Metadata.OccuredAt,
                        e.Metadata.EventName,
                        e.Content));
        }

        public async Task Raise(string eventName, object content)
        {
            using var connection = EventStoreConnection.Create( // Create a connection to our EventStoreDB instance.
                ConnectionSettings.Create().DisableTls().Build(), // In production, TLS should be enabled.
                new Uri(ConnectionString));
            await connection.ConnectAsync(); // Actually open the connection to our instance.
            var res =
                await
                    connection
                        .AppendToStreamAsync( // Write the event to the store; we don't have to mess with any SQL queries.
                            "ShoppingCart",
                            ExpectedVersion.Any,
                            new EventData( // EventStoreDB stores events as EventData objects.
                                Guid.NewGuid(),
                                "ShoppingCartEvent",
                                isJson: true,
                                data: Encoding.UTF8.GetBytes(JsonSerializer.Serialize(content)),
                                metadata: Encoding.UTF8.GetBytes(JsonSerializer.Serialize(new EventMetadata
                                (
                                    OccuredAt: DateTimeOffset.UtcNow,
                                    EventName: eventName
                                )))));
        }

        public record EventMetadata(DateTimeOffset OccuredAt, string EventName);
    }
}