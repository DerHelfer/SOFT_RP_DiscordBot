using System.Collections.Concurrent;
using System.Text.Json;
using DiscordBot.Models;

namespace DiscordBot.Services;

public static class ProfileService
{
    private static readonly ConcurrentDictionary<ulong, AdminProfile> _profiles = new();
    private const string ProfilesFile = "profiles.json";

    public static void LoadProfiles()
    {
        try
        {
            if (!File.Exists(ProfilesFile)) return;
            
            var json = File.ReadAllText(ProfilesFile);
            var profiles = JsonSerializer.Deserialize<Dictionary<ulong, AdminProfile>>(json);
            
            if (profiles != null)
            {
                foreach (var kvp in profiles)
                    _profiles[kvp.Key] = kvp.Value;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading profiles: {ex.Message}");
        }
    }

    public static void SaveProfiles()
    {
        try
        {
            var json = JsonSerializer.Serialize(_profiles.ToDictionary(kvp => kvp.Key, kvp => kvp.Value), 
                new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(ProfilesFile, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving profiles: {ex.Message}");
        }
    }

    public static AdminProfile? GetProfile(ulong userId)
    {
        _profiles.TryGetValue(userId, out var profile);
        return profile;
    }

    public static void AddProfile(ulong userId, AdminProfile profile)
    {
        _profiles[userId] = profile;
        SaveProfiles();
    }
}