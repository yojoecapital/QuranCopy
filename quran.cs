using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Windows.Forms;

namespace QuranCopy
{
    static class Program
    {
        static readonly string surahsFileName = "surahs.xml";
        static readonly string ayatFileName = "ayat.xml";

        static string SurahsFilePath{
            get{
                return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, surahsFileName);
            }
        }

        static string AyatFilePath{
            get{
                return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ayatFileName);
            }
        }

        static IEnumerable<XElement> Surahs{
            get{
                return XDocument.Load(SurahsFilePath).Root.Elements();
            }
        }

        static IEnumerable<XElement> Ayat{
            get{
                return XDocument.Load(AyatFilePath).Root.Elements();
            }
        }

        public static int? FindClosestSurah(IEnumerable<XElement> surahs, string key)
        {
            int minDistance = int.MaxValue;
            int? closest = null;

            foreach (var e in surahs)
            {
                var s = (string)e.Attribute("TransliterationName");
                int distance = ComputeLevenshteinDistance(s, key);

                if (distance < minDistance)
                {
                    minDistance = distance;
                    closest = int.Parse((string)e.Attribute("SurahId"));
                }
            }

            return closest;
        }

        public static int ComputeLevenshteinDistance(string s, string t)
        {
            int m = s.Length;
            int n = t.Length;
            int[,] d = new int[m + 1, n + 1];

            for (int i = 0; i <= m; i++)
            {
                d[i, 0] = i;
            }

            for (int j = 0; j <= n; j++)
            {
                d[0, j] = j;
            }

            for (int j = 1; j <= n; j++)
            {
                for (int i = 1; i <= m; i++)
                {
                    int cost = (s[i - 1] == t[j - 1]) ? 0 : 1;

                    d[i, j] = Math.Min(Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1), d[i - 1, j - 1] + cost);
                }
            }

            return d[m, n];
        }

        static XElement GetSurah(string arg)
        {
            int surahNumber;
            XElement surah;
            if (int.TryParse(arg, out surahNumber))
            {
                if (surahNumber > 0 && surahNumber <= 114)
                    surah = Surahs.FirstOrDefault(row => int.Parse((string)row.Attribute("SurahId")) == surahNumber);
                else surah = null;
            }
            else 
            {
                var surahs = Surahs;
                surahNumber = FindClosestSurah(surahs, arg).Value;
                surah = surahs.FirstOrDefault(row => int.Parse((string)row.Attribute("SurahId")) == surahNumber);
            }
            return surah;
        }

        [STAThreadAttribute]
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            if (args.Length == 1)
            {
                var surah = GetSurah(args[0]);
                if (surah == null) Console.WriteLine("Error: surah not found");
                else {
                    Clipboard.SetText((string)surah.Attribute("Name"));
                    Console.WriteLine((string)surah.Attribute("TransliterationName"));
                    Console.WriteLine("Surah Number: " + (string)surah.Attribute("SurahId"));
                    Console.WriteLine("Ayat Count: " +(string)surah.Attribute("AyahCount"));
                }
            }
            else if ((args.Length > 1))
            {
                var surah = GetSurah(args[0]);
                if (surah == null) Console.WriteLine("Error: surah not found");
                else {
                    var surahNumber = (string)surah.Attribute("SurahId");
                    int ayahNumber, endAyahNumber, ayahCount = int.Parse((string)surah.Attribute("AyahCount"));
                    if (int.TryParse(args[1], out ayahNumber))
                    {
                        ayahNumber = Math.Min(Math.Max(ayahNumber, 1), ayahCount);
                        if (args.Length > 2 && int.TryParse(args[2], out endAyahNumber) && ayahNumber <= endAyahNumber)
                        {
                            endAyahNumber = Math.Min(endAyahNumber, ayahCount);
                            var ayat = Ayat.Where(row => surahNumber.Equals((string)row.Attribute("SurahId"))
                                                                && ayahNumber <= int.Parse((string)row.Attribute("Number"))
                                                                && endAyahNumber >= int.Parse((string)row.Attribute("Number"))).ToList();
                            if (ayat == null || ayat.Count == 0) Console.WriteLine("Error: ayat not found");
                            else {
                                Clipboard.SetText((string)string.Join("\n ", ayat.Select(ayah => (string)ayah.Attribute("Ayah"))));
                                Console.WriteLine("Copied ayat " + ayahNumber + " through " + endAyahNumber + " of surah " + (string)surah.Attribute("TransliterationName"));
                            }
                        }
                        else {
                            var ayah = Ayat.FirstOrDefault(row => surahNumber.Equals((string)row.Attribute("SurahId"))
                                                                && ayahNumber == int.Parse((string)row.Attribute("Number")));
                            if (ayah == null) Console.WriteLine("Error: ayah not found");
                            else {
                                Clipboard.SetText((string)ayah.Attribute("Ayah"));
                                Console.WriteLine("Copied ayah " + ayahNumber + " of surah " + (string)surah.Attribute("TransliterationName"));
                            }
                        }
                    }
                    else
                    {
                        Clipboard.SetText((string)surah.Attribute("Name"));
                        Console.WriteLine((string)surah.Attribute("TransliterationName"));
                        Console.WriteLine("Surah Number: " + (string)surah.Attribute("SurahId"));
                        Console.WriteLine("Ayat Count: " +(string)surah.Attribute("AyahCount"));
                    }
                }
            }
            else Console.WriteLine("Usage: quran.exe <surah number> <ayah number>");
        }
    }
}
