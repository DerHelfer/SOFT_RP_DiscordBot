using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using DiscordBot.Modules;

namespace DiscordBot.Services;

public class DiscordService
{
    private readonly DiscordSocketClient _client;
    private readonly InteractionService _interactionService;
    private readonly IServiceProvider _services;

    public DiscordService(IServiceProvider services)
    {
        _services = services;
        _client = new DiscordSocketClient();
        _interactionService = new InteractionService(_client);
        RegisterEventHandlers();
    }

    public async Task StartAsync(string token)
    {
        await _client.LoginAsync(TokenType.Bot, token);
        await _client.StartAsync();
    }

    private void RegisterEventHandlers()
    {
        _client.Ready += async () =>
        {
            await _interactionService.AddModuleAsync<ProfileModule>(_services);
            await _interactionService.RegisterCommandsGloballyAsync();
        };

        _client.SlashCommandExecuted += async command =>
        {
            await _interactionService.ExecuteCommandAsync(
                new SocketInteractionContext(_client, command), _services);
        };
    }
}