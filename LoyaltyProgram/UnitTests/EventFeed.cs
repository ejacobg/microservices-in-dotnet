using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SpecialOffers.Controllers;
using SpecialOffers.Models;
using Xunit;

namespace UnitTests
{
    public class EventFeed : IDisposable
    {
        private readonly IHost _host;
        private readonly HttpClient _sut;

        public EventFeed()
        {
            _host = new HostBuilder()
                .ConfigureWebHost(host =>
                    host
                        .ConfigureServices(x => x
                            .AddScoped<IEventStore, FakeEventStore>() // Use the fake event store as our IEventStore implementation.
                            .AddControllersByType(typeof(EventFeedController)) // Only use endpoints from EventFeedController.
                            .AddApplicationPart(typeof(EventFeedController).Assembly))
                        .Configure(x => x.UseRouting().UseEndpoints(opt => opt.MapControllers()))
                        .UseTestServer())
                .Start();
            _sut = _host.GetTestClient();
        }

        [Fact]
        public async Task TestEventRange()
        {
            var actual = await _sut.GetAsync("/events?start=0&end=100");

            Assert.Equal(HttpStatusCode.OK, actual.StatusCode);
            var eventFeedEvents =
                await JsonSerializer.DeserializeAsync<IEnumerable<Event>>(await actual.Content.ReadAsStreamAsync())
                ?? Enumerable.Empty<Event>();
            Assert.Equal(100, eventFeedEvents.Count());
        }

        [Fact]
        public async Task TestEmptyResponse()
        {
            var actual = await _sut.GetAsync("/events?start=200&end=300");

            var eventFeedEvents =
                await JsonSerializer.DeserializeAsync<IEnumerable<Event>>(await actual.Content.ReadAsStreamAsync());
            Assert.Empty(eventFeedEvents);
        }

        public void Dispose()
        {
            _sut?.Dispose();
            _host?.Dispose();
        }
    }

    public class FakeEventStore : IEventStore
    {
        public Task RaiseEvent(string name, object content) =>
            throw new NotImplementedException();

        public Task<IEnumerable<Event>> GetEvents(int start, int end)
        {
            // Requests for events after the 100th event will be empty.
            if (start > 100)
                return Task.FromResult(Enumerable.Empty<Event>());

            // Otherwise, generate fake events and return them.
            return Task.FromResult(Enumerable
                .Range(start, end - start)
                .Select(i => new Event(i, DateTimeOffset.UtcNow, "some event", new object())));
        }
    }
}