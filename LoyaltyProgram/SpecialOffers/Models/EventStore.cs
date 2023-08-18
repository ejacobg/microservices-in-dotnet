using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace SpecialOffers.Models
{
    public record Event(long SequenceNumber, DateTimeOffset OccuredAt, string Name, object Content);

    public interface IEventStore
    {
        void RaiseEvent(string name, object content);
        IEnumerable<Event> GetEvents(int start, int end);
    }

    public class EventStore : IEventStore
    {
        private static long _currentSequenceNumber;
        private static readonly IList<Event> Database = new List<Event>();

        public void RaiseEvent(string name, object content)
        {
            var seqNumber = Interlocked.Increment(ref _currentSequenceNumber);
            Database.Add(new Event(seqNumber, DateTimeOffset.UtcNow, name, content));
        }

        public IEnumerable<Event> GetEvents(int start, int end)
            => Database
                .Where(e => start <= e.SequenceNumber && e.SequenceNumber < end)
                .OrderBy(e => e.SequenceNumber);
    }
}