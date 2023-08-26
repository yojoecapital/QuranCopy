using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace QuranCopy 
{
    class SearchSettings
    {
        public int peek;
        public int resultsPerPage;
        public bool ignoreAccents;
        public bool useArabize;
        public Dictionary<string, string> replace;

        public SearchSettings() { }

        private static readonly string settingsFileName = "settings.json";

        public static SearchSettings Create()
        {
            var json = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, settingsFileName));
            var settings = JsonConvert.DeserializeObject<SearchSettings>(json);
            return settings;
        }
    }

}