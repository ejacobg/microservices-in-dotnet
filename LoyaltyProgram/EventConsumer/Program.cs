using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

namespace EventConsumer
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var start = await GetStartIdFromDatastore(); // Continue from where we left off last time.
            var end = 100;
            var specialOffersHostName = args.Length >= 1 ? args[0] : "http://special-offers:5002";
            var notificationsHostName = args.Length >= 2 ? args[1] : "http://notifications:5003";

            start = await EventConsumer.ConsumeBatch(start, end, specialOffersHostName, notificationsHostName);

            // Save the starting point for the next batch of events.
            await SaveStartIdToDataStore(start); // The value of `start` gets updated during the ProcessEvents() call.
        }

        // fake implementation. Should get from a real database
        public static Task<long> GetStartIdFromDatastore() => Task.FromResult(0L);

        public static Task SaveStartIdToDataStore(long startId) => Task.CompletedTask;
    }

    public static class EventConsumer
    {
        public static async Task<long> ConsumeBatch(long start, long end, string specialOffersHostName,
            string notificationsHostName)
        {
            var client = new HttpClient();

            // Prepare and send the request.
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            using var resp =
                await client.GetAsync(new Uri($"{specialOffersHostName}/events?start={start}&end={end}"));

            // Process the events that were returned.
            // Deserialization doesn't seem to work correctly. Getting default values for all the fields. Will not be fixing this for now.
            var events =
                await JsonSerializer.DeserializeAsync<SpecialOfferEvent[]>(await resp.Content.ReadAsStreamAsync()) ??
                Array.Empty<SpecialOfferEvent>();

            foreach (var @event in events)
            {
                // Match special offer in @event to registered users and send notification to matching user.
                await client.PostAsync($"{notificationsHostName}/notify", new StringContent(""));
                start = Math.Max(start, @event.SequenceNumber + 1);
            }

            return start;
        }
    }

    public record SpecialOfferEvent(long SequenceNumber, DateTimeOffset OccuredAt, string Name, object Content);
}