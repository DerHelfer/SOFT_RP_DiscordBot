using Discord;
using Discord.Interactions;
using System.Collections.Concurrent;

public class ProfileModule : InteractionModuleBase<SocketInteractionContext>
{
    private static readonly ConcurrentDictionary<ulong, AdminProfile> AdminProfiles = new();
    private static readonly List<string> AdminRanks = new() { "Стажер", "Помощник", "Модератор", "Админ" };

    public class AdminProfile
    {
        public string SteamId { get; set; }
        public string Name { get; set; }
        public string ForumUrl { get; set; }
        public int RankId { get; set; } = 0;
    }
    [SlashCommand("addadmin", "Добавить администратора")]
    public async Task AddAdmin(
        [Summary("user", "Пользователь Discord")] IUser user,
        [Summary("sid", "Steam ID")] string sid,
        [Summary("rank", "Ранг (номер из списка)")] int rank = 1,
        [Summary("name", "Никнейм админа на сервере")] string name = null,
        [Summary("url", "Ссылка на профиль на форуме")] string url = null)
    {
        if (rank < 1 || rank > AdminRanks.Count)
        {
            await RespondAsync($"Неверный ранг. Доступные ранги: 1-{AdminRanks.Count}", ephemeral: true);
            return;
        }

        AdminProfiles[user.Id] = new AdminProfile
        {
            SteamId = sid,
            Name = name,
            ForumUrl = url,
            RankId = rank - 1
        };

        var embed = new EmbedBuilder()
            .WithTitle("Администратор добавлен")
            .AddField("Steam ID", sid, true)
            .AddField("Ранг", AdminRanks[rank - 1], true)
            .WithColor(Color.Green);

        if (!string.IsNullOrEmpty(name))
            embed.AddField("Никнейм", name, true);
        if (!string.IsNullOrEmpty(url))
            embed.AddField("Профиль форума", url, true);

        await RespondAsync(embed: embed.Build());
    }

    [SlashCommand("profile", "Показать профиль пользователя")]
    public async Task Profile(
        [Summary("user", "Пользователь Discord")] IUser user = null)
    {
        var targetUser = user ?? Context.User;
        var embed = new EmbedBuilder()
            .WithTitle($"Профиль {targetUser.Username}")
            .WithThumbnailUrl(targetUser.GetAvatarUrl())
            .WithColor(Color.Blue);

        if (AdminProfiles.TryGetValue(targetUser.Id, out var adminProfile))
        {
            embed.AddField("Ранг", AdminRanks[adminProfile.RankId], true);
            embed.AddField("Steam ID", adminProfile.SteamId, true);
            if (!string.IsNullOrEmpty(adminProfile.Name))
                embed.AddField("Никнейм", adminProfile.Name, true);
            if (!string.IsNullOrEmpty(adminProfile.ForumUrl))
                embed.AddField("Профиль форума", adminProfile.ForumUrl, true);
        }
        else
        {
            embed.AddField("Статус", "Обычный пользователь", true);
        }

        await RespondAsync(embed: embed.Build());
    }

    [SlashCommand("ranks", "Показать список рангов")]
    public async Task ShowRanks()
    {
        await RespondAsync("Список админских рангов:");

        for (int i = 0; i < AdminRanks.Count; i++)
        {
            var embed = new EmbedBuilder()
                .WithTitle($"{i + 1}. {AdminRanks[i]}")
                .WithColor(Color.Orange)
                .Build();

            await Context.Channel.SendMessageAsync(embed: embed);
        }
    }

    [SlashCommand("addrank", "Добавить новый ранг")]
    public async Task AddRank([Summary("name", "Название ранга")] string name)
    {
        AdminRanks.Add(name);
        await RespondAsync($"Ранг '{name}' добавлен под номером {AdminRanks.Count}");
    }

    [SlashCommand("editrank", "Изменить название ранга")]
    public async Task EditRank(
        [Summary("number", "Номер ранга")] int number,
        [Summary("name", "Новое название")] string name)
    {
        if (number < 1 || number > AdminRanks.Count)
        {
            await RespondAsync($"Неверный номер. Доступные: 1-{AdminRanks.Count}", ephemeral: true);
            return;
        }
        var oldName = AdminRanks[number - 1];
        AdminRanks[number - 1] = name;
        await RespondAsync($"Ранг '{oldName}' изменен на '{name}'");
    }

    [SlashCommand("removerank", "Удалить ранг")]
    public async Task RemoveRank([Summary("number", "Номер ранга")] int number)
    {
        if (number < 1 || number > AdminRanks.Count)
        {
            await RespondAsync($"Неверный номер. Доступные: 1-{AdminRanks.Count}", ephemeral: true);
            return;
        }
        var removedRank = AdminRanks[number - 1];
        AdminRanks.RemoveAt(number - 1);
        
        foreach (var profile in AdminProfiles.Values.Where(p => p.RankId >= number - 1))
            if (profile.RankId > number - 1) profile.RankId--;
            else profile.RankId = 0;
        
        await RespondAsync($"Ранг '{removedRank}' удален");
    }
}