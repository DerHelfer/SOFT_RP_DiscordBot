using Discord;
using Discord.WebSocket;
using Discord.Interactions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using DiscordBot.Modules;

class Program
{
    static async Task Main()
    {
        try
        {
            var config = LoadConfiguration();
            var token = ValidateToken(config);
            
            var client = new DiscordSocketClient();
            var interactionService = new InteractionService(client);
            var services = ConfigureServices(client, interactionService);
            
            RegisterEventHandlers(client, interactionService, services);
            
            await client.LoginAsync(TokenType.Bot, token);
            await client.StartAsync();
            await Task.Delay(-1);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Fatal error: {ex.Message}");
        }
    }

    private static IConfiguration LoadConfiguration()
    {
        return new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
    }
    
    private static string ValidateToken(IConfiguration config)
    {
        var token = config["DiscordToken"];
        if (string.IsNullOrEmpty(token))
            throw new InvalidOperationException("Discord token not found in appsettings.json");
        return token;
    }

    private static IServiceProvider ConfigureServices(DiscordSocketClient client, InteractionService interactionService)
    {
        return new ServiceCollection()
            .AddSingleton(client)
            .AddSingleton(interactionService)
            .BuildServiceProvider();
    }

    private static void RegisterEventHandlers(DiscordSocketClient client, InteractionService interactionService, IServiceProvider services)
    {
        client.Ready += async () =>
        {
            await interactionService.AddModuleAsync<ProfileModule>(services);
            await interactionService.RegisterCommandsGloballyAsync();
        };

        client.SlashCommandExecuted += async (command) =>
        {
            await interactionService.ExecuteCommandAsync(new SocketInteractionContext(client, command), services);
        };
    }
}