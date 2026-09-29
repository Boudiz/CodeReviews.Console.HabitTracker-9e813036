namespace HabitTracker;

using System;
using System.IO;
using System.Text.Json;

public class ConfigSettings
{
    public string Key { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;

    private static readonly string FilePath = Path.Combine(AppContext.BaseDirectory, "config.json");
    public static ConfigSettings LoadConfig()
    {
        if (!File.Exists(FilePath))
        {
            throw new FileNotFoundException($"Configuration file missing at: {FilePath}");
        }

        string jsonContent = File.ReadAllText(FilePath);

        return JsonSerializer.Deserialize<ConfigSettings>(jsonContent)
               ?? throw new InvalidOperationException("Failed to deserialize configuration file.");
    }

    public static bool IsConfigValid()
    {
        if (!File.Exists(FilePath)) return false;
        try
        {
            string jsonContent = File.ReadAllText(FilePath);
            using JsonDocument doc = JsonDocument.Parse(jsonContent);
            JsonElement root = doc.RootElement;
            
            bool hasKey = (root.TryGetProperty("Key", out var key)) && 
                          !string.IsNullOrWhiteSpace(key.GetString());
            
            bool hasRegion = (root.TryGetProperty("Region", out var region)) && 
                             !string.IsNullOrWhiteSpace(region.GetString());

            return hasKey && hasRegion;
        }
        catch (Exception)
        {
            return false;
        }
    }
    
}