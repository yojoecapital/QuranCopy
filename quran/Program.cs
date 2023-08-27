using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Windows.Forms;
using System;
using System.IO;

namespace QuranCopy
{
    static class Program
    {
        private static void ProcessArgs(string[] args)
        {
            bool searchAyah = false, searchTranslation = false;
            if (args.Length > 1 && (searchAyah = args[0].Equals("ar?")) || (searchTranslation = args[0].Equals("en?")))
            {
                Search.Run(args, searchTranslation);
            }
            else if (args.Length == 1 && (args[0].Equals("open") || args[0].Equals("o")))
            {
                System.Diagnostics.Process.Start(FileManager.SettingsFilePath);
                Console.WriteLine(FileManager.SettingsFilePath);
            }
            else if (args.Length == 1 && (args[0].Equals("reload") || args[0].Equals("r")))
            {
                Search.ReloadSettings();
                Console.WriteLine("Reloaded settings");
            }
            else if (args.Length == 1 && (args[0].Equals("help") || args[0].Equals("h")))
            {
                Console.WriteLine("Usage:");
                Console.WriteLine("  open (o)                     - Open the settings JSON");
                Console.WriteLine("  reload (r)                   - Reload the settings JSON");
                Console.WriteLine("  [surah]                      - Get information about [surah]");
                Console.WriteLine("  [surah] [ayah]               - Copy [ayah] from [surah]");
                Console.WriteLine("                                 (optionally add 't' at the end to copy translation too)");
                Console.WriteLine("  [surah] [start] [end]        - Copy ayat [start] to [end] from [surah]");
                Console.WriteLine("                                 (optionally add 't' at the end to copy translation too)");
                Console.WriteLine("  ar? [text]                   - Searches for Arabic [text] (will use arabize.exe if found)");
                Console.WriteLine("  en? [text]                   - Searches for translation [text]");
                Console.WriteLine("  clear (cls)                  - Clear the console screen");
                Console.WriteLine("  quit (q)                     - Exit the program");

            }
            else if (args.Length == 1)
            {
                XElement surah = ClosestSurah.GetSurah(args[0]);
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
                XElement surah = ClosestSurah.GetSurah(args[0]);
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
                        if (args.Length > 3 && args[3].Equals("t"))
                        {
                            var list = FileManager.Ayat.Join(FileManager.Translation, ayahRow => int.Parse((string)ayahRow.Attribute("AyahId")), translationRow => int.Parse((string)translationRow.Attribute("AyahId")),
                            (ayahRow, translationRow) => new {
                                Ayah = (string)ayahRow.Attribute("Ayah"),
                                Translation = (string)translationRow.Attribute("Translation"),
                                SurahId = (string)ayahRow.Attribute("SurahId"),
                                Number = int.Parse((string)ayahRow.Attribute("Number"))
                            }).Where(row => surahNumber.Equals(row.SurahId) && ayahNumber <= row.Number && endAyahNumber >= row.Number).ToList();
                            if (list == null || list.Count == 0)
                            {
                                Console.WriteLine("Error: ayat not found");
                                return;
                            }
                            withTranslation = " with translation";
                            copy = string.Join("\n", list.Select(row => row.Ayah + " \u06DD\n" + row.Translation));
                        }
                        else
                        {
                            var list = FileManager.Ayat.Where((XElement row) => surahNumber.Equals((string)row.Attribute("SurahId")) && ayahNumber <= int.Parse((string)row.Attribute("Number")) && endAyahNumber >= int.Parse((string)row.Attribute("Number"))).ToList();
                            if (list == null || list.Count == 0)
                            {
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
                        XElement xElement = FileManager.Ayat.FirstOrDefault((XElement row) => surahNumber.Equals((string)row.Attribute("SurahId")) && ayahNumber == int.Parse((string)row.Attribute("Number")));
                        if (xElement == null)
                        {
                            Console.WriteLine("Error: ayah not found");
                            return;
                        }
                        if (args.Length > 2 && args[2].Equals("t"))
                        {
                            var ayahId = (string)xElement.Attribute("AyahId");
                            var translation = (string)FileManager.Translation.FirstOrDefault((XElement row) => ayahId.Equals((string)row.Attribute("AyahId"))).Attribute("Translation");
                            Clipboard.SetText((string)xElement.Attribute("Ayah") + "\n" + translation);
                            Console.WriteLine("Copied ayah " + ayahNumber + " of (" + (string)surah.Attribute("SurahId") + ") " + (string)surah.Attribute("TransliterationName") + " with translation");
                        }
                        else
                        {
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
        }

        [STAThread]
        private static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.Unicode;
            Console.InputEncoding = Encoding.Unicode;
            if (args.Length > 0)
                ProcessArgs(args);
            else
            {
                string input;
                while (true)
                {
                    Console.Write("> ");
                    input = Console.ReadLine().Trim().ToLower();
                    if (string.IsNullOrEmpty(input)) continue;
                    else if (input.Equals("cls") || input.Equals("clear"))
                    {
                        Console.Clear();
                        continue;
                    }
                    else if (input.Equals("q") || input.Equals("quit")) return;
                    var argsArray = input.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    ProcessArgs(argsArray);
                }
            }
        }
    }

}