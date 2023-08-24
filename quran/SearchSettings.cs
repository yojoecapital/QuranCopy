using Newtonsoft.Json;
using System.IO;

namespace QuranCopy 
{
    public class SearchSettings
    {
        public int peek;
        public int resultsPerPage;
        public bool ignoreAccents;
        public bool useArabize;
        public string arabizePath;

        public SearchSettings() { }

        public static SearchSettings Create()
        {
            var json = File.ReadAllText(FileManager.SettingsFilePath);
            var settings = JsonConvert.DeserializeObject<SearchSettings>(json);
            FileManager.ArabizePath = settings.arabizePath;
            return settings;
        }
    }

}