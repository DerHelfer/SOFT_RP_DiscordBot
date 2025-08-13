namespace DiscordBot.Models;

public class AdminProfile
{
    public string? Name { get; set; }
    public string? SteamId { get; set; }
    public string? Rank { get; set; }
    public string? Url { get; set; }
    public int Points { get; set; }
    public int Complaints { get; set; }
    public int Reports { get; set; }
    public int Recruitments { get; set; }
    public int CheatChecks { get; set; }
}