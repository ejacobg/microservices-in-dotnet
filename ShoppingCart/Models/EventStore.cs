using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using EventStore.ClientAPI;

namespace ShoppingCart.Models
{
    /// <summary>
    /// IEventStore represents a service for managing events on an event feed.
    /// </summary>
    public interface IEventStore
    {
        // GetEvents returns all events whose sequence number falls within the given bounds (inclusivity depends on implementation). These events may or may not be ordered sequentially.
        Task<IEnumerable<Event>> GetEvents(long firstEventSequenceNumber, long lastEventSequenceNumber);
        
        // Raise adds the given event to the store.
        Task Raise(string eventName, object content);
    }

    public class InmemEventStore : IEventStore
    {
        private static long _currentSequenceNumber;
        private static readonly IList<Event> Database = new List<Event>();

        public Task<IEnumerable<Event>> GetEvents(
            long firstEventSequenceNumber,
            long lastEventSequenceNumber) 
            => Task.FromResult(Database
                .Where(e =>
                    e.SequenceNumber >= firstEventSequenceNumber &&
                    e.SequenceNumber <= lastEventSequenceNumber)
                .OrderBy(e => e.SequenceNumber).AsEnumerable());

        public Task Raise(string eventName, object content)
        {
            var seqNumber = Interlocked.Increment(ref _currentSequenceNumber);
            Database.Add(
                new Event(
                    seqNumber,
                    DateTimeOffset.UtcNow,
                    eventName,
                    content));
            return Task.CompletedTask;
        }
    }
}