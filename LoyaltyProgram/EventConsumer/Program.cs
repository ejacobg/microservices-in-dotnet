using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

var start = await GetStartIdFromDatastore(); // Continue from where we left off last time.
const long numEvents = 100L;
var client = new HttpClient();

// Prepare and send the request.
client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
using var resp =
    await client.GetAsync(new Uri($"http://special-offers:5002/events?start={start}&end={start + numEvents}"));

// Process the events that were returned.
await ProcessEvents(await resp.Content.ReadAsStreamAsync());

// Save the starting point for the next batch of events.
await SaveStartIdToDataStore(start); // The value of `start` gets updated during the ProcessEvents() call.

// fake implementation. Should get from a real database
Task<long> GetStartIdFromDatastore() => Task.FromResult(0L);

// fake implementation. Should apply business rules to events
async Task ProcessEvents(Stream content)
{
    var events = await JsonSerializer.DeserializeAsync<SpecialOfferEvent[]>(content) ?? Array.Empty<SpecialOfferEvent>();
    foreach (var @event in events)
    {
        Console.WriteLine(@event);
        start = Math.Max(start, @event.SequenceNumber + 1);
    }
}

Task SaveStartIdToDataStore(long startId) => Task.CompletedTask;

public record SpecialOfferEvent(long SequenceNumber, DateTimeOffset OccuredAt, string Name, object Content);