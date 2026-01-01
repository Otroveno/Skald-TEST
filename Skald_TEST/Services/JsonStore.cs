
using System;
using System.IO;
using System.Text;
using System.Text.Json;

namespace AIInfluence.Services
{
    public static class JsonStore
    {
        private static readonly string BasePath =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "Mount & Blade II Bannerlord", "AIInfluence");

        public static void Save(string filename, object data)
        {
            try
            {
                Directory.CreateDirectory(BasePath);
                var json = JsonSerializer.Serialize(data, new JsonSerializerOptions
                {
                    WriteIndented = true
                });
                File.WriteAllText(Path.Combine(BasePath, filename), json, Encoding.UTF8);
            }
            catch (Exception)
            {
                // fail silently to avoid crashing the game
            }
        }
    }
}
