using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace QuranCopyCore.Utilities
{
    internal static class Helpers
    {
        private static int? FindClosestSurah(IEnumerable<SurahModel> surahs, string key)
        {
            int min = int.MaxValue;
            int? result = null;
            foreach (var surah in surahs)
            {
                string s = (string)surah.TransliterationName;
                int distance = ComputeLevenshteinDistance(s, key);
                if (distance < min)
                {
                    min = distance;
                    result = surah.SurahId;
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

        public static SurahModel GetSurah(IEnumerable<SurahModel> surahs, string arg)
        {
            if (int.TryParse(arg, out int surahNumber))
            {
                var test = surahs.ToList();
                if (surahNumber > 0 && surahNumber <= 114)
                    return surahs.FirstOrDefault(surah => surah.SurahId == surahNumber);
                else return null;
            }
            else
            {
                surahNumber = FindClosestSurah(surahs, arg).Value;
                return surahs.FirstOrDefault(surah => surah.SurahId == surahNumber);
            }
        }

        public static string RemoveAccents(string input, Dictionary<string, string> replace, bool ignoreAccents)
        {
            if (ignoreAccents)
            {
                var stringBuilder = new StringBuilder();
                var chars = input.Normalize(NormalizationForm.FormD).ToCharArray();
                foreach (char letter in chars)
                {
                    if (CharUnicodeInfo.GetUnicodeCategory(letter) != UnicodeCategory.NonSpacingMark)
                        stringBuilder.Append(letter);
                }
                input = stringBuilder.ToString();
            }
            foreach (var key in replace.Keys)
                input = input.Replace(key, replace[key]);
            return input;
        }

        public static string Peekify(this string text, int index, int peek, bool rtl = false)
        {
            if (peek < text.Length - index)
            {
                if (rtl) return "\"..." + text.Substring(index, peek).Replace("\"", "'") + "\"";
                return "\"" + text.Substring(index, peek).Replace("\"", "'") + "...\"";
            }
            else return "\"" + text.Substring(index, text.Length - index).Replace("\"", "'") + "\"";
        }
    }
}
