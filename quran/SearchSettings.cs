using Newtonsoft.Json;
using System.IO;

namespace QuranCopy 
{
    class SearchSettings
    {
        public int peek = 25;
        public int resultsPerPage = 4;
        public bool ignoreAccents = true;
        public bool useArabize = false;

        public SearchSettings() { }

        public static SearchSettings Create()
        {
            var json = File.ReadAllText("settings.json");
            var settings = JsonConvert.DeserializeObject<SearchSettings>(json);
            return settings;
        }
    }

}