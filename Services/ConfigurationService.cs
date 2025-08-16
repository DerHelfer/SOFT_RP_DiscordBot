using Microsoft.Extensions.Configuration;
using DiscordBot.Constants;

namespace DiscordBot.Services;

public class ConfigurationService
{
    private readonly IConfiguration _configuration;

    public ConfigurationService()
    {
        _configuration = new ConfigurationBuilder()
            .AddJsonFile(FileNames.Token)
            .Build();
    }

    public string GetDiscordToken()
    {
        var token = _configuration["DiscordToken"];
        if (string.IsNullOrEmpty(token) || token == "YOUR_DISCORD_BOT_TOKEN_HERE")
            throw new InvalidOperationException($"Discord token not configured in {FileNames.Token}");
        return token;
    }
}