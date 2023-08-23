using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Polly;
using Polly.Extensions.Http;

namespace ApiGateway
{
    /// <summary>
    /// LoyaltyProgramClient provides methods for working with the Loyalty Program microservice.
    /// </summary>
    public class LoyaltyProgramClient
    {
        private readonly HttpClient _httpClient;

        // If you want any retrying policies to be applied, they must be set on the injected HttpClient.
        public LoyaltyProgramClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<HttpResponseMessage> RegisterUser(string name)
        {
            var user = new { name, Settings = new { } };
            return await _httpClient.PostAsync("/users/",
                CreateBody(user));
        }

        private static StringContent CreateBody(object user) =>
            new(
                JsonSerializer.Serialize(user),
                Encoding.UTF8,
                "application/json");

        public async Task<HttpResponseMessage> QueryUser(string arg) =>
            await _httpClient.GetAsync($"/users/{int.Parse(arg)}");

        public async Task<HttpResponseMessage> UpdateUser(LoyaltyProgramUser user) =>
            await _httpClient.PutAsync($"/users/{user.Id}", CreateBody(user));
    }

    /// <summary>
    /// RetryingClient functions similarly to LoyaltyProgramClient, but configures its own retrying policies rather than relying on a preconfigured HttpClient.
    /// Note that the URI for the remote endpoint still needs to be configured in the injected client.
    /// </summary>
    public class RetryingClient
    {
        // A "fast retry" policy using exponential backoff. This policy should be used when sending commands to the Loyalty Program microservice.
        // Alternatively, this may be moved into Startup.cs (in this case used within Program.cs) and applied to the HttpClient injected into his class.
        private static readonly IAsyncPolicy<HttpResponseMessage>
            ExponentialRetryPolicy =
                Policy<HttpResponseMessage> // Code executed under this policy should return an HttpResponseMessage.
                    .Handle<HttpRequestException>()
                    .OrTransientHttpStatusCode() // Handles timeouts and server (5XX) errors.
                    .WaitAndRetryAsync( // Use an async policy since this will be used with async code.
                        3, // Retry 3 times.
                        attempt =>
                            TimeSpan.FromMilliseconds(100 * Math.Pow(2, attempt)));

        // This circuit breaker policy should be used when sending queries to the Loyalty Program microservice.
        private static readonly IAsyncPolicy<HttpResponseMessage>
            CircuitBreakerPolicy =
                Policy<HttpResponseMessage> // Configured similarly to the exponential retry policy.
                    .Handle<HttpRequestException>()
                    .OrTransientHttpStatusCode()
                    .CircuitBreakerAsync(
                        5,
                        TimeSpan.FromMinutes(1));

        private readonly HttpClient _httpClient;

        public RetryingClient(HttpClient httpClient) =>
            _httpClient = httpClient;

        public async Task<HttpResponseMessage> RegisterUser(string name)
        {
            var user = new { name, Settings = new { } };
            return await ExponentialRetryPolicy.ExecuteAsync(
                () => // Execute this request using our exponential backoff policy.
                    _httpClient.PostAsync("/users/", CreateBody(user)));
        }

        private static StringContent CreateBody(object user) =>
            new(
                JsonSerializer.Serialize(user),
                Encoding.UTF8,
                "application/json");

        public async Task<HttpResponseMessage> QueryUser(string arg) =>
            await CircuitBreakerPolicy.ExecuteAsync(() => _httpClient.GetAsync($"/users/{int.Parse(arg)}"));

        public async Task<HttpResponseMessage> UpdateUser(LoyaltyProgramUser user) =>
            await ExponentialRetryPolicy.ExecuteAsync(() =>
                _httpClient.PutAsync($"/users/{user.Id}", CreateBody(user)));
    }

    public record LoyaltyProgramUser(int Id, string Name, int LoyaltyPoints, LoyaltyProgramSettings Settings);

    public record LoyaltyProgramSettings()
    {
        public LoyaltyProgramSettings(string[] interests) : this()
        {
            Interests = interests;
        }

        public string[] Interests { get; init; } = Array.Empty<string>();
    }
}