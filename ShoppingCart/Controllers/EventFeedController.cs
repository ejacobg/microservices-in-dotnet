using System.Linq;
using Microsoft.AspNetCore.Mvc;
using ShoppingCart.Models;

namespace ShoppingCart.Controllers
{
    [Route("/events")]
    public class EventFeedController
    {
        private readonly IEventStore _eventStore;

        public EventFeedController(IEventStore eventStore) => _eventStore = eventStore;
    
        // Get provides an endpoint that allows users to indicate a range of sequence values (inclusive), and returns all events within that range.
        [HttpGet("")]
        public Event[] Get([FromQuery] long start, [FromQuery] long end = long.MaxValue) => 
            _eventStore.GetEvents(start, end).ToArray(); // Converting to array is redundant since IEnumerable already gets rendered as a JSON array?
    }
}