using System;

namespace LoyaltyProgram.Models
{
    public record User(int Id, string Name, int LoyaltyPoints, UserSettings Settings);

    public record UserSettings()
    {
        public UserSettings(string[] interests) : this()
        {
            Interests = interests;
        }

        public string[] Interests { get; init; } = Array.Empty<string>();
    }
}