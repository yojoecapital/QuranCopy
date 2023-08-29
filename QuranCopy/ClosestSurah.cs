using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace QuranCopy
{
    public static class ClosestSurah
    {
        private static int? FindClosestSurah(IEnumerable<XElement> surahs, string key)
        {
            int min = int.MaxValue;
            int? result = null;
            foreach (XElement surah in surahs)
            {
                string s = (string)surah.Attribute("TransliterationName");
                int distance = ComputeLevenshteinDistance(s, key);
                if (distance < min)
                {
                    min = distance;
                    result = int.Parse((string)surah.Attribute("SurahId"));
                }
            }
            return result;
        }

        private static int ComputeLevenshteinDistance(string s, string t)
        {
            int length = s.Length;
            int length2 = t.Length;
            int[,] array = new int[length + 1, length2 + 1];
            for (int i = 0; i <= length; i++)
            {
                array[i, 0] = i;
            }
            for (int j = 0; j <= length2; j++)
            {
                array[0, j] = j;
            }
            for (int j = 1; j <= length2; j++)
            {
                for (int i = 1; i <= length; i++)
                {
                    int min = ((s[i - 1] != t[j - 1]) ? 1 : 0);
                    array[i, j] = Math.Min(Math.Min(array[i - 1, j] + 1, array[i, j - 1] + 1), array[i - 1, j - 1] + min);
                }
            }
            return array[length, length2];
        }

        public static XElement GetSurah(string arg)
        {
            int surahNumber;
            if (int.TryParse(arg, out surahNumber))
            {
                if (surahNumber > 0 && surahNumber <= 114)
                {
                    return FileManager.Surahs.FirstOrDefault((XElement row) => int.Parse((string)row.Attribute("SurahId")) == surahNumber);
                }
                return null;
            }
            var surahs = FileManager.Surahs;
            surahNumber = FindClosestSurah(surahs, arg).Value;
            return surahs.FirstOrDefault((XElement row) => int.Parse((string)row.Attribute("SurahId")) == surahNumber);
        }
    }
}
