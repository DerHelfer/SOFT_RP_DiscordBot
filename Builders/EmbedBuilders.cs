using Discord;
using DiscordBot.Models;

namespace DiscordBot.Builders;

public static class EmbedBuilders
{
    public static Embed CreateProfileEmbed(IUser user, AdminProfile? profile)
    {
        var builder = new EmbedBuilder()
            .WithTitle($"Профиль {user.Username}")
            .WithColor(0x006400)
            .WithThumbnailUrl(user.GetAvatarUrl() ?? user.GetDefaultAvatarUrl());

        builder.AddField("Имя", profile?.Name ?? "N/A", false);
        builder.AddField("Steam ID", profile?.SteamId ?? "N/A", false);
        builder.AddField("Ранг", profile?.Rank ?? "N/A", false);
        
        if (!string.IsNullOrEmpty(profile?.Url))
            builder.AddField("Форум", profile.Url, false);

        if (profile?.Points > 0)
            builder.AddField("Баллы", profile.Points.ToString(), true);
        if (profile?.Complaints > 0)
            builder.AddField("Жалобы", profile.Complaints.ToString(), true);
        
        if (profile?.Reports > 0)
            builder.AddField("Ивенты", profile.Reports.ToString(), true);
        if (profile?.Recruitments > 0)
            builder.AddField("Наборы", profile.Recruitments.ToString(), true);
        if (profile?.CheatChecks > 0)
            builder.AddField("Проверки на читы", profile.CheatChecks.ToString(), true);

        return builder.WithFooter("Система администрации").Build();
    }

    public static Embed CreateAdminAddedEmbed(IUser user, string name, string steamId, string rank, string? url)
    {
        return new EmbedBuilder()
            .WithTitle("Администратор добавлен")
            .WithColor(0x00FF00)
            .WithThumbnailUrl(user.GetAvatarUrl() ?? user.GetDefaultAvatarUrl())
            .AddField("Пользователь", user.Mention, false)
            .AddField("Ник", name, false)
            .AddField("Steam ID", steamId, false)
            .AddField("Ранг", rank, false)
            .AddField("Форум", url ?? "N/A", false)
            .WithFooter("Система администрации")
            .Build();
    }
}