using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;

static class Program
{
    static readonly string surahsFileName = "surahs.xml";

    static readonly string ayatFileName = "ayat.xml";

    private static string SurahsFilePath {
        get {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, surahsFileName);
        }
    } 

    private static string AyatFilePath {
        get{
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ayatFileName);
        }
    }

    private static IEnumerable<XElement> Surahs {
        get{
            return XDocument.Load(SurahsFilePath).Root.Elements();
        }
    }

    private static IEnumerable<XElement> Ayat {
        get {
            return XDocument.Load(AyatFilePath).Root.Elements();
        }
    }

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

    private static XElement GetSurah(string arg)
    {
        int surahNumber;
        if (int.TryParse(arg, out surahNumber))
        {
            if (surahNumber > 0 && surahNumber <= 114)
            {
                return Surahs.FirstOrDefault((XElement row) => int.Parse((string)row.Attribute("SurahId")) == surahNumber);
            }
            return null;
        }
        IEnumerable<XElement> surahs = Surahs;
        surahNumber = FindClosestSurah(surahs, arg).Value;
        return surahs.FirstOrDefault((XElement row) => int.Parse((string)row.Attribute("SurahId")) == surahNumber);
    }

    [STAThread]
    private static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;
        if (args.Length == 1)
        {
            XElement surah = GetSurah(args[0]);
            if (surah == null)
            {
                Console.WriteLine("Error: surah not found");
                return;
            }
            Clipboard.SetText((string)surah.Attribute("Name"));
            Console.WriteLine((string)surah.Attribute("TransliterationName"));
            Console.WriteLine("Surah Number: " + (string)surah.Attribute("SurahId"));
            Console.WriteLine("Ayat Count: " + (string)surah.Attribute("AyahCount"));
        }
        else if (args.Length > 1)
        {
            XElement surah = GetSurah(args[0]);
            if (surah == null)
            {
                Console.WriteLine("Error: surah not found");
                return;
            }
            string surahNumber = (string)surah.Attribute("SurahId");
            int ayahCount = int.Parse((string)surah.Attribute("AyahCount")), ayahNumber;
            if (int.TryParse(args[1], out ayahNumber))
            {
                int endAyahNumber;
                ayahNumber = Math.Min(Math.Max(ayahNumber, 1), ayahCount);
                if (args.Length > 2 && int.TryParse(args[2], out endAyahNumber) && ayahNumber <= endAyahNumber)
                {
                    endAyahNumber = Math.Min(endAyahNumber, ayahCount);
                    List<XElement> list = Ayat.Where((XElement row) => surahNumber.Equals((string)row.Attribute("SurahId")) && ayahNumber <= int.Parse((string)row.Attribute("Number")) && endAyahNumber >= int.Parse((string)row.Attribute("Number"))).ToList();
                    if (list == null || list.Count == 0)
                    {
                        Console.WriteLine("Error: ayat not found");
                        return;
                    }
                    Clipboard.SetText(string.Join("\n", list.Select((XElement ayah) => (string)ayah.Attribute("Ayah") + " \u06DD ")));
                    Console.WriteLine("Copied ayat " + ayahNumber + " through " + endAyahNumber + " of surah " + (string)surah.Attribute("TransliterationName"));
                }
                else
                {
                    XElement xElement = Ayat.FirstOrDefault((XElement row) => surahNumber.Equals((string)row.Attribute("SurahId")) && ayahNumber == int.Parse((string)row.Attribute("Number")));
                    if (xElement == null)
                    {
                        Console.WriteLine("Error: ayah not found");
                        return;
                    }
                    Clipboard.SetText((string)xElement.Attribute("Ayah"));
                    Console.WriteLine("Copied ayah " + ayahNumber + " of surah " + (string)surah.Attribute("TransliterationName"));
                }
            }
            else
            {
                Clipboard.SetText((string)surah.Attribute("Name"));
                Console.WriteLine((string)surah.Attribute("TransliterationName"));
                Console.WriteLine("Surah Number: " + (string)surah.Attribute("SurahId"));
                Console.WriteLine("Ayat Count: " + (string)surah.Attribute("AyahCount"));
            }
        }
        else
        {
            Console.WriteLine("Usage: quran.exe <surah number> <ayah number>");
        }
    }
}
