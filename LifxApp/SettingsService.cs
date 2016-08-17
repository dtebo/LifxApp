using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Windows.Storage;
using System.Runtime.Serialization;
using System.IO;

namespace LifxApp
{
    public static class SettingsService
    {
        private const string SETTINGS_FILENAME = "Settings.json";
        private static StorageFolder _settingsFolder = Windows.Storage.ApplicationData.Current.LocalFolder;

        public async static Task<Settings> LoadSettings()
        {
            try
            {
                StorageFile sf = await _settingsFolder.GetFileAsync(SETTINGS_FILENAME);
                if (sf == null) return null;

                string content = await FileIO.ReadTextAsync(sf);
                return JsonConvert.DeserializeObject<Settings>(content);
            }
            catch
            {
                return null;
            }
        }

        public async static Task<bool> SaveSettings(Settings data)
        {
            try
            {
                StorageFile file = await _settingsFolder.CreateFileAsync(SETTINGS_FILENAME, CreationCollisionOption.ReplaceExisting);
                string content = JsonConvert.SerializeObject(data);
                await FileIO.WriteTextAsync(file, content);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }

    [DataContract]
    public class Settings
    {
        [DataMember]
        public string ApiToken { get; set; }
    }
}
