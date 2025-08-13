using System.Text.RegularExpressions;

namespace DiscordBot.Validators;

public static class UrlValidator
{
    private static readonly Regex ForumUrlPattern = new(@"^https://softrp\.ru/forum/members/[\w\-\.]+/?$", RegexOptions.Compiled);

    public static bool IsValidForumUrl(string url) => ForumUrlPattern.IsMatch(url);
}