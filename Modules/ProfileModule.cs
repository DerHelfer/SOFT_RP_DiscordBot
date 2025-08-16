using Discord;
using Discord.Interactions;
using DiscordBot.Validators;
using DiscordBot.Builders;
using DiscordBot.Services;
using DiscordBot.Models;
using DiscordBot.Constants;

namespace DiscordBot.Modules;

public class ProfileModule : InteractionModuleBase<SocketInteractionContext>
{

    [SlashCommand("profile", "Показать профиль")]
    public async Task Profile([Summary("user", "Пользователь")] IUser user = null)
    {
        var targetUser = user ?? Context.User;
        var profile = ProfileService.GetProfile(targetUser.Id);
        var embed = EmbedBuilders.CreateProfileEmbed(targetUser, profile);
        
        await RespondAsync(embed: embed);
    }

    [SlashCommand("addadmin", "Добавить администратора")]
    public async Task AddAdmin(
        [Summary("user", "Пользователь")] IUser user,
        [Summary("steamid", "Steam ID")] string steamId,
        [Summary("rank", "Ранг")] IRole rank,
        [Summary("name", "Ник")] string name = null,
        [Summary("url", "Форум")] string url = null)
    {
        if (!SteamIdValidator.IsValid(steamId))
        {
            await RespondAsync(Messages.InvalidSteamId, ephemeral: true);
            return;
        }

        if (url != null && !UrlValidator.IsValidForumUrl(url))
        {
            await RespondAsync(Messages.InvalidUrl, ephemeral: true);
            return;
        }

        var profile = new AdminProfile
        {
            Name = name ?? user.Username,
            SteamId = steamId,
            Rank = rank.Name,
            Url = url
        };

        ProfileService.AddProfile(user.Id, profile);

        if (user is IGuildUser guildUser)
        {
            await guildUser.AddRoleAsync(rank);
        }

        var embed = EmbedBuilders.CreateAdminAddedEmbed(user, name ?? user.Username, steamId, rank.Name, url);
        await RespondAsync(embed: embed);
    }
}