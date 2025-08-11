using Discord;
using Discord.Interactions;
using System.Collections.Concurrent;
using System.Text.Json;

public class ProfileModule : InteractionModuleBase<SocketInteractionContext>
{
    private static readonly ConcurrentDictionary<ulong, AdminProfile> AdminProfiles = new();
    private static List<AdminRankType> AdminRanks = LoadRanks();

    private static List<AdminRankType> LoadRanks()
    {
        try
        {
            var json = File.ReadAllText("ranks.json");
            var ranks = JsonSerializer.Deserialize<List<AdminRankType>>(json) ?? new();
            
            for (int i = 0; i < ranks.Count; i++)
            {
                if (ranks[i].Id == 0)
                    ranks[i] = ranks[i] with { Id = i + 1 };
            }
            
            return ranks;
        }
        catch
        {
            return new();
        }
    }

    private static void SaveRanks()
    {
        var json = JsonSerializer.Serialize(AdminRanks, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText("ranks.json", json);
    }

    public record AdminRankType(int Id, string Name, string Type);

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
            .AddField("Ранг", $"{AdminRanks[rank - 1].Name} ({AdminRanks[rank - 1].Type})", true)
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
            embed.AddField("Ранг", $"{AdminRanks[adminProfile.RankId].Name} ({AdminRanks[adminProfile.RankId].Type})", true);
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

        var typeOrder = new[] { "Младшая администрация", "Старшая администрация", "Высшая администрация" };
        
        foreach (var type in typeOrder)
        {
            var ranksOfType = AdminRanks.Where(r => r.Type == type).ToList();
                
            if (!ranksOfType.Any()) continue;
            
            var ranksList = string.Join("\n", ranksOfType.Select(rank => $"{rank.Id}. {rank.Name}"));
            
            var embed = new EmbedBuilder()
                .WithTitle(type)
                // .AddField("\u200b", ranksList, false)
                .AddField(ranksList, "\u200b", false)
                .WithColor(Color.Orange);

            await Context.Channel.SendMessageAsync(embed: embed.Build());
        }
    }

    [SlashCommand("addrank", "Добавить новый ранг")]
    public async Task AddRank(
        [Summary("name", "Название ранга")] string name,
        [Summary("type", "Тип администрации")]
        [Choice("Младшая администрация", "Младшая администрация")]
        [Choice("Старшая администрация", "Старшая администрация")]
        [Choice("Высшая администрация", "Высшая администрация")]
        string type = "Младшая администрация")
    {
        var newId = AdminRanks.Any() ? AdminRanks.Max(r => r.Id) + 1 : 1;
        AdminRanks.Add(new(newId, name, type));
        SaveRanks();
        await RespondAsync($"Ранг '{name}' ({type}) добавлен");
    }

    [SlashCommand("editrank", "Изменить название ранга")]
    public async Task EditRank(
        [Summary("id", "ID ранга")] int id,
        [Summary("name", "Новое название")] string name)
    {
        var rankIndex = AdminRanks.FindIndex(r => r.Id == id);
        if (rankIndex == -1)
        {
            await RespondAsync($"Ранг с ID {id} не найден", ephemeral: true);
            return;
        }
        var oldRank = AdminRanks[rankIndex];
        AdminRanks[rankIndex] = new(oldRank.Id, name, oldRank.Type);
        SaveRanks();
        await RespondAsync($"Ранг '{oldRank.Name}' изменен на '{name}'");
    }

    [SlashCommand("removerank", "Удалить ранг")]
    public async Task RemoveRank([Summary("id", "ID ранга")] int id)
    {
        var rankIndex = AdminRanks.FindIndex(r => r.Id == id);
        if (rankIndex == -1)
        {
            await RespondAsync($"Ранг с ID {id} не найден", ephemeral: true);
            return;
        }
        var removedRank = AdminRanks[rankIndex];
        AdminRanks.RemoveAt(rankIndex);
        
        foreach (var profile in AdminProfiles.Values.Where(p => p.RankId == rankIndex))
            profile.RankId = 0;
        
        SaveRanks();
        await RespondAsync($"Ранг '{removedRank.Name}' удален");
    }
}