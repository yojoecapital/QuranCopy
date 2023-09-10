using System.Collections.Generic;

namespace QuranCopyCore.Utilities
{
    internal class Settings
    {
        public int peek;
        public int resultsPerPage = 1;
        public bool ignoreAccents;
        public bool useArabize;
        public string arabizePath;
        public Dictionary<string, string> replace;
    }

}