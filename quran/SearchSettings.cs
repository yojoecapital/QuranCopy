using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
        public Dictionary<string, string> replace;

        public SearchSettings() { }

        public static SearchSettings Create()
        {
            var json = File.ReadAllText(FileManager.SettingsFilePath);
            var settings = JsonConvert.DeserializeObject<SearchSettings>(json);
            FileManager.ArabizePath = settings.arabizePath;
            if (settings.replace == null) 
                settings.replace = new Dictionary<string, string>;
            return settings;
        }
    }

}