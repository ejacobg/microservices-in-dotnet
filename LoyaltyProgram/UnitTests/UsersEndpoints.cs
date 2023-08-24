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
using Xunit;

namespace UnitTests
{
    public class UsersEndpoints : IDisposable
    {
        private readonly IHost _host;
        private readonly HttpClient _sut; // "System Under Test"

        public UsersEndpoints()
        {
            _host = new HostBuilder()
                .ConfigureWebHost(x => x
                    .UseStartup<Startup>() // Use the real loyalty program startup code to initialize your host.
                    .UseTestServer())
                .Start();
            _sut = _host.GetTestClient();
        }

        [Fact]
        public async Task TestNotFoundUser()
        {
            var actual = await _sut.GetAsync("/users/1000");
            Assert.Equal(HttpStatusCode.NotFound, actual.StatusCode);
        }

        [Fact]
        public async Task TestRegisterUser()
        {
            var expected = new User(0, "Christian", 0, new UserSettings());

            // Send a POST request to the server..
            var registrationResponse = await _sut.PostAsync("/users",
                new StringContent(JsonSerializer.Serialize(expected), Encoding.UTF8, "application/json"));

            // Deserialize the returned user.
            var newUser =
                await JsonSerializer.DeserializeAsync<User>(
                    await registrationResponse.Content.ReadAsStreamAsync(),
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            // Get the user data from the GET endpoint.
            var actual = await _sut.GetAsync($"/users/{newUser?.Id}");
            var actualUser = JsonSerializer.Deserialize<User>(await actual.Content.ReadAsStringAsync(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            // Compare the responses from the POST and GET responses.
            Assert.Equal(HttpStatusCode.OK, actual.StatusCode);
            Assert.Equal(expected.Name, actualUser?.Name);
        }

        [Fact]
        public async Task TestUpdateUser()
        {
            // Create the new user and deserialize the response.
            var user = new User(0, "Christian", 0, new UserSettings());
            var registrationResponse = await _sut.PostAsync(
                "/users",
                new StringContent(JsonSerializer.Serialize(user), Encoding.UTF8, "application/json"));
            var newUser = await
                JsonSerializer.DeserializeAsync<User>(
                    await registrationResponse.Content.ReadAsStreamAsync(),
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;

            // Update the user.
            var expected = "jane";
            var updatedUser = newUser! with { Name = expected };
            var actual = await _sut.PutAsync($"/users/{newUser.Id}",
                new StringContent(JsonSerializer.Serialize(updatedUser), Encoding.UTF8, "application/json"));
            var actualUser = await JsonSerializer.DeserializeAsync<User>(
                await actual.Content.ReadAsStreamAsync(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            // Confirm that the name change was applied.
            Assert.Equal(HttpStatusCode.OK, actual.StatusCode);
            Assert.Equal(expected, actualUser?.Name);
        }

        public void Dispose()
        {
            _host.Dispose();
            _sut.Dispose();
        }
    }
}