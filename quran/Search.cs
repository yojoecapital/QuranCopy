using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace QuranCopy
{
    public static class Search 
    {
        private static SearchSettings settings;

        private static SearchSettings Settings
        {
            get
            {
                if (settings == null) settings = SearchSettings.Create();
                return settings;
            }
        }

        public static string RemoveAccents(this string input)
        {
            if (Settings.ignoreAccents)
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
            foreach (var key in settings.replace.Keys)
                input = input.Replace(key, settings.replace[key]);
            return input;
        }


        private static string Peekify(this string text, int index, int peek, bool rtl = false)
        {
            if (peek < text.Length - index)
            {
                if (rtl) return "\"..." + text.Substring(index, peek).Replace("\"", "'") + "\"";
                return "\"" + text.Substring(index, peek).Replace("\"", "'") + "...\"";
            }
            else return "\"" + text.Substring(index, text.Length - index).Replace("\"", "'") + "\"";
        }

        static string RunArabize(string input)
        {
            if (!Settings.useArabize) return input;
            string path = FileManager.ArabizePath;
            if (!string.IsNullOrEmpty(path))
            {
                var startInfo = new ProcessStartInfo
                {
                    FileName = path,
                    Arguments = input,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                using (Process process = new Process())
                {
                    process.StartInfo = startInfo;
                    process.Start();
                    string output = process.StandardOutput.ReadToEnd();
                    process.WaitForExit();
                    return output.Trim('\r', '\n', ' ', '\t').RemoveAccents();
                }
            }
            else return input.RemoveAccents();
        }


        private static IEnumerable<string> Lookup(string arg, int peek, bool searchTranslation)
        {
            var surahs = FileManager.Surahs;
            var iterator = FileManager.Ayat.Join(FileManager.Translation, ayahRow => int.Parse((string)ayahRow.Attribute("AyahId")), translationRow => int.Parse((string)translationRow.Attribute("AyahId")),
                            (ayahRow, translationRow) => new {
                                Ayah = (string)ayahRow.Attribute("Ayah"),
                                Translation = (string)translationRow.Attribute("Translation"),
                                SurahId = (string)ayahRow.Attribute("SurahId"),
                                Number = int.Parse((string)ayahRow.Attribute("Number"))
                            });
            if (!searchTranslation) arg = arg.RemoveAccents();
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
                    if (searchTranslation)
                    {
                        ayah = row.Ayah.RemoveAccents().Peekify(0, peek, true);
                        translation = text.Peekify(index, peek, false);
                    }
                    else
                    {
                        ayah = text.Peekify(index, peek, true);
                        translation = row.Translation.Peekify(0, peek, false);
                    }
                    yield return ayah + "\n" + translation + "\n\u2192 Ayah " + number + " from " + "(" + surahId + ") " + surahName + "\n";
                }
            }
        }


        public static void Run(string[] args, bool searchTranslation)
        {
            if (args.Length > 1)
            {
                var list = args.ToList();
                int peek = Settings.peek, resultsPerPage = Settings.resultsPerPage;
                list.RemoveAt(0);
                if (args.Length > 2 && int.TryParse(args[1], out int result))
                {
                    peek = result;
                    list.RemoveAt(0);
                }
                var prompt = string.Join(" ", list);
                if (!searchTranslation) prompt = RunArabize(prompt);
                var searchResults = Lookup(prompt, peek, searchTranslation);
                int currentPage = 0, total = searchResults.Count();
                while (currentPage * resultsPerPage < total)
                {
                    Console.Clear();
                    Console.WriteLine("Searching for: \"" + prompt + "\"");
                    var currentPageResults = searchResults.Skip(currentPage * resultsPerPage).Take(resultsPerPage);
                    int count = 0;
                    foreach (var searchResult in currentPageResults)
                    {
                        Console.WriteLine(searchResult);
                        count++;
                    }
                    Console.WriteLine((currentPage * resultsPerPage + count) + " / " + total + " result(s)." +
                        "\nUse the arrow keys to display the next " + resultsPerPage + " results or Enter to stop.");
                    while (true)
                    {
                        var keyInfo = Console.ReadKey();
                        if (keyInfo.Key == ConsoleKey.Enter)
                            return;
                        else if (keyInfo.Key == ConsoleKey.RightArrow)
                        {
                            currentPage++;
                            break;
                        }
                        else if (keyInfo.Key == ConsoleKey.LeftArrow)
                        {
                            currentPage--;
                            if (currentPage < 0) currentPage = 0;
                            break;
                        }
                    }
                }
                Console.WriteLine("Search completed.");
            }
            else Console.WriteLine("Usage: quran.exe ar? <ayat search prompt>");
        }
    }
}
