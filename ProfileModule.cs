using Discord;
using Discord.Interactions;

public class ProfileModule : InteractionModuleBase<SocketInteractionContext>
{
    static ProfileModule()
    {
        ProfileService.LoadProfiles();
    }

    [SlashCommand("profile", "Показать профиль")]
    public async Task Profile([Summary("user", "Пользователь")] IUser user = null)
    {
        var targetUser = user ?? Context.User;
        var profile = ProfileService.GetProfile(targetUser.Id);
        
        var info = $"**Имя:** {profile?.Name ?? "N/A"}\n" +
                   $"**Steam ID:** {profile?.SteamId ?? "N/A"}\n" +
                   $"**Ранг:** {profile?.Rank ?? "N/A"}\n" +
                   $"**Баллы:** {profile?.Points.ToString() ?? "0"}\n" +
                   $"**Жалобы:** {profile?.Complaints.ToString() ?? "0"}\n" +
                   $"**Отчеты:** {profile?.Reports.ToString() ?? "0"}";

        var embed = new EmbedBuilder()
            .WithTitle($"Профиль {targetUser.Username}")
            .WithColor(0x006400)
            .AddField("Информация", info, false)
            .Build();
        
        await RespondAsync(embed: embed);
    }

    [SlashCommand("addadmin", "Добавить администратора")]
    public async Task AddAdmin(
        [Summary("user", "Пользователь")] IUser user,
        [Summary("name", "Ник")] string name,
        [Summary("steamid", "Steam ID")] string steamId,
        [Summary("rank", "Ранг")] IRole rank)
    {
        var profile = new AdminProfile
        {
            Name = name,
            SteamId = steamId,
            Rank = rank.Name,
            Points = 0,
            Complaints = 0,
            Reports = 0
        };

        ProfileService.AddProfile(user.Id, profile);

        if (user is IGuildUser guildUser)
        {
            await guildUser.AddRoleAsync(rank);
        }

        var embed = new EmbedBuilder()
            .WithTitle("Администратор добавлен")
            .WithColor(0x00FF00)
            .AddField("Пользователь", user.Mention, false)
            .AddField("Ник", name, false)
            .AddField("Steam ID", steamId, false)
            .AddField("Ранг", rank.Name, false)
            .Build();

        await RespondAsync(embed: embed);
    }
}