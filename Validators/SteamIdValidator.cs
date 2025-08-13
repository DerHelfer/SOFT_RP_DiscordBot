using System.Text.RegularExpressions;

namespace DiscordBot.Validators;

public static class SteamIdValidator
{
    private static readonly Regex SteamIdPattern = new(@"^STEAM_0:[01]:\d+$", RegexOptions.Compiled);

    public static bool IsValid(string steamId) => SteamIdPattern.IsMatch(steamId);
}