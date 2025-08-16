using Microsoft.Extensions.DependencyInjection;
using DiscordBot.Services;

class Program
{
    static async Task Main()
    {
        try
        {
            var services = ConfigureServices();
            var configService = services.GetRequiredService<ConfigurationService>();
            var discordService = services.GetRequiredService<DiscordService>();
            
            ProfileService.LoadProfiles();
            
            var token = configService.GetDiscordToken();
            await discordService.StartAsync(token);
            await Task.Delay(-1);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Fatal error: {ex.Message}");
        }
    }

    private static IServiceProvider ConfigureServices()
    {
        return new ServiceCollection()
            .AddSingleton<ConfigurationService>()
            .AddSingleton<DiscordService>()
            .BuildServiceProvider();
    }
}