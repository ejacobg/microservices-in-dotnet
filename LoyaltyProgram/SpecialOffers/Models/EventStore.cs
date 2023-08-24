using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SpecialOffers.Models
{
    public record Event(long SequenceNumber, DateTimeOffset OccuredAt, string Name, object Content);

    public interface IEventStore
    {
        Task RaiseEvent(string name, object content);
        Task<IEnumerable<Event>> GetEvents(int start, int end);
    }

    public class EventStore : IEventStore
    {
        private static long _currentSequenceNumber;
        private static readonly IList<Event> Database = new List<Event>();

        public Task RaiseEvent(string name, object content)
        {
            var seqNumber = Interlocked.Increment(ref _currentSequenceNumber);
            Database.Add(new Event(seqNumber, DateTimeOffset.UtcNow, name, content));
            return Task.CompletedTask;
        }

        public Task<IEnumerable<Event>> GetEvents(int start, int end)
            => Task.FromResult<IEnumerable<Event>>( // Using the type parameter is probably better than using AsEnumerable() like in ShoppingCart.Models.InmemEventStore.
                Database
                .Where(e => start <= e.SequenceNumber && e.SequenceNumber < end)
                .OrderBy(e => e.SequenceNumber));
    }
}