using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ApiGateway
{
    public class LoyaltyProgramClient
    {
        private readonly HttpClient _httpClient;

        public LoyaltyProgramClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<HttpResponseMessage> RegisterUser(string name)
        {
            var user = new {name, Settings = new { }};
            return await _httpClient.PostAsync("/users/",
                CreateBody(user));
        }

        private static StringContent CreateBody(object user) =>
            new StringContent(
                JsonSerializer.Serialize(user),
                Encoding.UTF8,
                "application/json");

        public async Task<HttpResponseMessage> QueryUser(string arg) => await _httpClient.GetAsync($"/users/{int.Parse(arg)}");

        public async Task<HttpResponseMessage> UpdateUser(LoyaltyProgramUser user) =>
            await _httpClient.PutAsync($"/users/{user.Id}", CreateBody(user));
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