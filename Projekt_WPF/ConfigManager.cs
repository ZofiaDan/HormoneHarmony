using System;
using System.IO;
using System.Text.Json;

namespace Projekt_WPF
{
    // Klasa przechowująca dane użytkownika (konfiguracja)
    public class UserSettings
    {
        public string UserName { get; set; } = "Girlie";
    }

    public static class ConfigManager
    {
        private static readonly string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");

        public static UserSettings LoadConfig()
        {
            if (!File.Exists(filePath)) return new UserSettings();
            try
            {
                string json = File.ReadAllText(filePath);
                return JsonSerializer.Deserialize<UserSettings>(json);
            }
            catch { return new UserSettings(); }
        }

        public static void SaveConfig(string name)
        {
            var settings = new UserSettings { UserName = name };
            string json = JsonSerializer.Serialize(settings);
            File.WriteAllText(filePath, json);
        }
    }
}