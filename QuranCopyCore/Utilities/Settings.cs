using System.Collections.Generic;

namespace QuranCopyCore.Utilities
{
    internal class Settings
    {
        public int peek = 1;
        public int resultsPerPage = 1;
        public bool ignoreAccents = false;
        public bool useArabize = false;
        public string arabizePath = string.Empty;
        public Dictionary<string, string> replace;
    }

}