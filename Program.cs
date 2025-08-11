using Discord;
using Discord.WebSocket;
using Discord.Interactions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

class Program
{
    static async Task Main()
    {
        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();

        var client = new DiscordSocketClient();
        var interactionService = new InteractionService(client);
        var services = new ServiceCollection()
            .AddSingleton(client)
            .AddSingleton(interactionService)
            .BuildServiceProvider();

        client.Ready += async () =>
        {
            await interactionService.AddModuleAsync<ProfileModule>(services);
            await interactionService.RegisterCommandsGloballyAsync();
        };

        client.SlashCommandExecuted += async (command) =>
        {
            try
            {
                await interactionService.ExecuteCommandAsync(new SocketInteractionContext(client, command), services);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                if (!command.HasResponded)
                    await command.RespondAsync("Произошла ошибка при выполнении команды.", ephemeral: true);
            }
        };

        await client.LoginAsync(TokenType.Bot, config["DiscordToken"]);
        await client.StartAsync();
        await Task.Delay(-1);
    }
}