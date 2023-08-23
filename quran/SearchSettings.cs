using Newtonsoft.Json;
using System.IO;

namespace QuranCopy 
{
    class SearchSettings
    {
        public int peek;
        public int resultsPerPage;
        public bool ignoreAccents;
        public bool useArabize;

        public SearchSettings() { }

        public static SearchSettings Create()
        {
            var json = File.ReadAllText("settings.json");
            var settings = JsonConvert.DeserializeObject<SearchSettings>(json);
            return settings;
        }
    }

}