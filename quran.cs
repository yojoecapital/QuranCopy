using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Windows.Forms;
using System.Xml.Linq;

static class Program
{
#region File
    static readonly string surahsFileName = "surahs.xml";

    static readonly string ayatFileName = "ayat.xml";

    static readonly string translationFileName = "translation.xml";

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

    private static string TranslationFilePath {
        get{
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, translationFileName);
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

    private static IEnumerable<XElement> Translation {
        get {
            return XDocument.Load(TranslationFilePath).Root.Elements();
        }
    }
#endregion

#region ClosestSurah
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
        var surahs = Surahs;
        surahNumber = FindClosestSurah(surahs, arg).Value;
        return surahs.FirstOrDefault((XElement row) => int.Parse((string)row.Attribute("SurahId")) == surahNumber);
    }
#endregion

#region Search
    public static string RemoveAccents(this string input)
    {     
        var stringBuilder = new StringBuilder();     
        var chars = input.Normalize(NormalizationForm.FormD).ToCharArray();  
        foreach (char letter in chars)
        {     
            if (CharUnicodeInfo.GetUnicodeCategory(letter) != UnicodeCategory.NonSpacingMark)  
                stringBuilder.Append(letter);     
        }     
        return stringBuilder.ToString();     
    } 

    
    private static string Peekify(this string text, int index, int peek, bool rtl = false)
    {
        if (peek < text.Length - index) {
            if (rtl) return "\"..." + text.Substring(index, peek).Replace("\"", "'") + "\"";
            return "\"" + text.Substring(index, peek).Replace("\"", "'") + "...\"";
        } else return "\"" + text.Substring(index, text.Length - index).Replace("\"", "'") + "\"";
    }

    private static IEnumerable<string> Search(string arg, int peek, bool searchTranslation)
    {
        var surahs = Surahs;
        var iterator = Ayat.Join(Translation, ayahRow => int.Parse((string)ayahRow.Attribute("AyahId")), translationRow => int.Parse((string)translationRow.Attribute("AyahId")), 
                        (ayahRow, translationRow) => new { 
                        Ayah = (string)ayahRow.Attribute("Ayah"), 
                        Translation = (string)translationRow.Attribute("Translation"),
                        SurahId = (string)ayahRow.Attribute("SurahId"),
                        Number = int.Parse((string)ayahRow.Attribute("Number")) });
        foreach (var row in iterator)
        {
            var number = row.Number;
            var text = searchTranslation ? row.Translation.ToLower() : row.Ayah.RemoveAccents();
            var index = text.IndexOf(arg);
            if (index != -1)
            {
                var surahId = row.SurahId;
                var surahName = (string)surahs.FirstOrDefault((XElement surahRow) => ((string)surahRow.Attribute("SurahId")).Equals(surahId)).Attribute("TransliterationName");
                string ayah, translation;
                if (searchTranslation){
                    ayah = row.Ayah.RemoveAccents().Peekify(0, peek, true);
                    translation = text.Peekify(index, peek, false);
                } else {
                    ayah = text.Peekify(index, peek, true);
                    translation = row.Translation.Peekify(0, peek, false);
                }
                yield return ayah + "\n" + translation + "\n\u2192 Ayah " + number + " from " + "(" + surahId + ") " + surahName + "\n";
            }
        }
    }
#endregion

    private static void ProcessArgs(string[] args)
    {
        bool searchAyah = false, searchTranslation = false;
        if ((searchAyah = args[0].Equals("ar?")) || (searchTranslation = args[0].Equals("en?")))
        {
            if (args.Length > 1)
            {
                var list = args.ToList();
                int peek = 20, result, count = 0;
                list.RemoveAt(0);
                if (args.Length > 2 && int.TryParse(args[1], out result)) {
                    peek = result;
                    list.RemoveAt(0);
                }
                var prompt = string.Join(" ", list);
                if (searchAyah) prompt = prompt.RemoveAccents();
                Console.WriteLine("Searching for: \"" + prompt + "\"");
                foreach (var searchResult in Search(prompt, peek, searchTranslation))
                {
                    Console.WriteLine(searchResult);
                    count++;
                }
                Console.WriteLine(count + " result(s)");
            }
            else Console.WriteLine("Usage: quran.exe ar? <ayat search prompt>");
        }
        else if (args.Length == 1)
        {
            XElement surah = GetSurah(args[0]);
            if (surah == null)
            {
                Console.WriteLine("Error: surah not found");
                return;
            }
            Clipboard.SetText((string)surah.Attribute("Name"));
            Console.WriteLine("(" + (string)surah.Attribute("SurahId") + ") " + (string)surah.Attribute("TransliterationName"));
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
                if (args.Length > 2 && int.TryParse(args[2], out endAyahNumber) && ayahNumber <= endAyahNumber) // multi-line copy
                {
                    endAyahNumber = Math.Min(endAyahNumber, ayahCount);
                    string copy, withTranslation = string.Empty;
                    if (args.Length > 3 && args[3].Equals("t")) {
                        var list = Ayat.Join(Translation, ayahRow => int.Parse((string)ayahRow.Attribute("AyahId")), translationRow => int.Parse((string)translationRow.Attribute("AyahId")), 
                        (ayahRow, translationRow) => new { 
                            Ayah = (string)ayahRow.Attribute("Ayah"), 
                            Translation = (string)translationRow.Attribute("Translation"),
                            SurahId = (string)ayahRow.Attribute("SurahId"),
                            Number = int.Parse((string)ayahRow.Attribute("Number")) }).Where(row => surahNumber.Equals(row.SurahId) && ayahNumber <= row.Number && endAyahNumber >= row.Number).ToList();
                        if (list == null || list.Count == 0) {
                            Console.WriteLine("Error: ayat not found");
                            return;
                        }
                        withTranslation = " with translation";
                        copy = string.Join("\n", list.Select(row => row.Ayah + " \u06DD\n" + row.Translation));
                    } else {
                        var list = Ayat.Where((XElement row) => surahNumber.Equals((string)row.Attribute("SurahId")) && ayahNumber <= int.Parse((string)row.Attribute("Number")) && endAyahNumber >= int.Parse((string)row.Attribute("Number"))).ToList();
                        if (list == null || list.Count == 0) {
                            Console.WriteLine("Error: ayat not found");
                            return;
                        }
                        copy = string.Join("\n", list.Select((XElement ayah) => (string)ayah.Attribute("Ayah") + " \u06DD "));
                    }
                    Clipboard.SetText(copy);
                    Console.WriteLine("Copied ayat " + ayahNumber + " through " + endAyahNumber + " of (" + (string)surah.Attribute("SurahId") + ") " + (string)surah.Attribute("TransliterationName") + withTranslation);
                }
                else // single-line copy
                {
                    XElement xElement = Ayat.FirstOrDefault((XElement row) => surahNumber.Equals((string)row.Attribute("SurahId")) && ayahNumber == int.Parse((string)row.Attribute("Number")));
                    if (xElement == null)
                    {
                        Console.WriteLine("Error: ayah not found");
                        return;
                    }
                    if (args.Length > 2 && args[2].Equals("t")) {
                        var ayahId = (string)xElement.Attribute("AyahId");
                        var translation = (string)Translation.FirstOrDefault((XElement row) => ayahId.Equals((string)row.Attribute("AyahId"))).Attribute("Translation");
                        Clipboard.SetText((string)xElement.Attribute("Ayah") + "\n" + translation);
                        Console.WriteLine("Copied ayah " + ayahNumber + " of (" + (string)surah.Attribute("SurahId") + ") " + (string)surah.Attribute("TransliterationName") + " with translation");
                    } else {
                        Clipboard.SetText((string)xElement.Attribute("Ayah"));
                        Console.WriteLine("Copied ayah " + ayahNumber + " of (" + (string)surah.Attribute("SurahId") + ") " + (string)surah.Attribute("TransliterationName"));
                    }
                }
            }
            else
            {
                Clipboard.SetText((string)surah.Attribute("Name"));
                Console.WriteLine("(" + (string)surah.Attribute("SurahId") + ") " + (string)surah.Attribute("TransliterationName"));
                Console.WriteLine("Ayat Count: " + (string)surah.Attribute("AyahCount"));
            }
        }
        else
        {
            Console.WriteLine("Usage: quran.exe <surah number> <ayah number>");
        }
    }

    [STAThread]
    private static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;
        if (args.Length > 0)
            ProcessArgs(args);
        else
        {
            string input;
            while (true)
            {
                Console.Write("> ");
                input = Console.ReadLine();
                if (input.Equals("cls")) 
                {
                    Console.Clear();
                    continue;
                }
                else if (input.Equals("q") || input.Equals("quit")) return;
                var argsArray = input.Split(new char[]{' '}, StringSplitOptions.RemoveEmptyEntries);
                ProcessArgs(argsArray);
            }
        }
    }
}
