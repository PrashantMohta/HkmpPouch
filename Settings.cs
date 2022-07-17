using System.IO;
using Newtonsoft.Json;

namespace HkmpPouch{
    public class SettingsFile{
        public bool enableLogging = true;
        public bool enableDebugLogging = false;
    }
    public static class Settings{
        public static SettingsFile currentSettings = new();

        public static void LoadSettings(){
            var settingsFilePath = Path.Combine(Platform.getCurrentDirectory(),"HkmpPouch.json");
            if(!File.Exists(settingsFilePath)){
                SaveSettings();
            }
            currentSettings = JsonConvert.DeserializeObject<SettingsFile>(File.ReadAllText(settingsFilePath));
        }

        public static void SaveSettings(){
            var settingsFilePath = Path.Combine(Platform.getCurrentDirectory(),"HkmpPouch.json");
            File.WriteAllText(settingsFilePath, JsonConvert.SerializeObject(currentSettings,Formatting.Indented));
        }
    }
}