using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text.Json;

namespace RXInstanceManager
{
    public static class AppInitializer
    {
        internal static readonly string FileName = "rxman.config";

        public static string FilePath { get; }
        public static AppConfig Config { get; set; }

        public static void Initialize()
        {
            if (!File.Exists(FilePath))
            {
                var config = new AppConfig();
                config.LogViewerPath = "";

                var newJson = Serialize(config);
                File.WriteAllText(FilePath, newJson);
            }

            var json = File.ReadAllText(FilePath);
            Config = Deserialize(json);
        }

        public static string Serialize(AppConfig config)
        {
            return JsonSerializer.Serialize<AppConfig>(config);
        }

        public static AppConfig Deserialize(string json)
        {
            return JsonSerializer.Deserialize<AppConfig>(json);
        }

        static AppInitializer()
        {
            FilePath = $"{AppContext.BaseDirectory}{FileName}";
        }
    }
}
