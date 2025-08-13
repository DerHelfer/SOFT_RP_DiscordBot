public class AdminProfile
{
    public string? Name { get; set; }
    public string? SteamId { get; set; }
    public string? Rank { get; set; }
    public int Points { get; set; } = 0;
    public int Complaints { get; set; } = 0;
    public int Reports { get; set; } = 0;
}