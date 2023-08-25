using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using LoyaltyProgram;
using LoyaltyProgram.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;
using ServiceTests.Mocks;
using Xunit;

namespace ServiceTests.Scenarios
{
    public class RegisterUserAndGetNotification : IDisposable
    {
        private static int _mocksPort = 5050;
        private readonly MocksHost _serviceMock;
        private readonly IHost _loyaltyProgramHost;
        private readonly HttpClient _sut;

        public RegisterUserAndGetNotification()
        {
            _serviceMock = new MocksHost(_mocksPort); // Launch our mocks.
            _loyaltyProgramHost = new HostBuilder() // Run our loyalty program service.
                .ConfigureWebHost(x => x
                    .UseStartup<Startup>()
                    .UseTestServer())
                .Start();
            _sut = _loyaltyProgramHost.GetTestClient();
        }

        [Fact]
        public async Task Scenario()
        {
            await RegisterNewUser();
            await RunConsumer();
            AssertNotificationWasSent();
        }

        private async Task RegisterNewUser()
        {
            var actual = await _sut.PostAsync( // Register a new user.
                "/users",
                new StringContent(
                    JsonSerializer.Serialize(new User(0, "Chr", 0, new UserSettings())),
                    Encoding.UTF8,
                    "application/json"));

            Assert.Equal(HttpStatusCode.Created, actual.StatusCode);
        }

        private Task RunConsumer()
        {
            return EventConsumer.EventConsumer.ConsumeBatch(
                0,
                100,
                $"http://localhost:{_mocksPort}/specialoffers",
                $"http://localhost:{_mocksPort}"
            );
        }

        private void AssertNotificationWasSent() => Assert.True(NotificationsMock.ReceivedNotification);

        // Apparently the test still hangs after completion. Not sure why this is.
        public void Dispose()
        {
            _serviceMock?.Dispose();
            _loyaltyProgramHost?.Dispose();
            _sut?.Dispose();
        }
    }
}