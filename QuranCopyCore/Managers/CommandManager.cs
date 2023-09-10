using CliFramework;
using Newtonsoft.Json;
using QuranCopyCore.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace QuranCopyCore.Managers
{
    internal class CommandManager
    {
        private readonly SearchManager searchManager;
        private readonly QuranCopyCoreFileManager fileManager;

        public CommandManager(SearchManager searchManager)
        {
            this.searchManager = searchManager;
            fileManager = searchManager.fileManager;
        }

        public string RunArabize(string input)
        {
            var settings = fileManager.Settings;
            if (settings != null && settings.useArabize)
            {
                string path = fileManager.Settings.arabizePath;
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
                    using Process process = new();
                    process.StartInfo = startInfo;
                    process.Start();
                    string output = process.StandardOutput.ReadToEnd();
                    process.WaitForExit();
                    return Helpers.RemoveAccents(output.Trim('\r', '\n', ' ', '\t'), settings.replace, settings.ignoreAccents).Replace("\r", string.Empty).Replace("\n", string.Empty).Replace("\t", string.Empty);
                }
                else return Helpers.RemoveAccents(input, settings.replace, settings.ignoreAccents);
            }
            else return input;
        }

        public void ArabicLookup(string[] args)
        {
            var settings = fileManager.Settings;
            if (settings != null) 
            {
                var arg = string.Join(" ", args.Skip(1));
                arg = Helpers.RemoveAccents(RunArabize(arg), settings.replace, settings.ignoreAccents);
                var lookup = searchManager.TextLookup(settings, arg, false);
                PrettyConsole.PrintPagedList(lookup, settings.resultsPerPage, "Searching for: \"" + arg + "\"");
            }
        }

        public void EnglishLookup(string[] args)
        {
            var settings = fileManager.Settings;
            if (settings != null)
            {
                var arg = string.Join(" ", args.Skip(1));
                var lookup = searchManager.TextLookup(settings, arg, true);
                PrettyConsole.PrintPagedList(lookup, settings.resultsPerPage);
                PrettyConsole.PrintPagedList(lookup, settings.resultsPerPage, "Searching for: \"" + arg + "\"");
            }
        }

        public void Reload(string[] _)
        {
            fileManager.Reload();
            Console.WriteLine("Reloaded settings.");
        }

        public void OpenSettings(string[] _)
        {
            var path = QuranCopyCoreFileManager.SettingsFilePath;
            ProcessStartInfo psi = new()
            {
                FileName = path,
                UseShellExecute = true
            };
            Process.Start(psi);
            Console.WriteLine(path);
        }

        public void CopySurah(string[] args)
        {
            var surahs = fileManager.Surahs;
            if (surahs != null)
            {
                var surah = Helpers.GetSurah(surahs, args[0]);
                if (surah != null)
                {
                    Clipboard.SetText(surah.Name);
                    PrettyConsole.PrintColor(surah, ConsoleColor.Yellow);
                }
                else PrettyConsole.PrintError("Surah not found.");
            }
        }

        public void CopyAyah(string[] args)
        {
            var surahs = fileManager.Surahs;
            var ayat = fileManager.Ayat;
            var translation = fileManager.Translation;
            var settings = fileManager.Settings;
            if (settings != null && ayat != null && surahs != null && translation != null)
            {
                var surah = Helpers.GetSurah(surahs, args[0]);
                if (surah != null)
                {
                    int surahNumber = surah.SurahId;
                    int ayahCount = surah.AyahCount;
                    if (int.TryParse(args[1], out int ayahNumber))
                    {
                        ayahNumber = Math.Min(Math.Max(ayahNumber, 1), ayahCount);
                        if (args.Length > 2 && int.TryParse(args[2], out int endAyahNumber) && ayahNumber <= endAyahNumber) // multi-line copy
                        {
                            endAyahNumber = Math.Min(endAyahNumber, ayahCount);
                            string copy, json;
                            if (args.Length > 3 && args[3].Equals("t"))
                            {
                                var list = fileManager.AyatTranslation?.Where(row => surahNumber.Equals(row.SurahId) && ayahNumber <= row.Number && endAyahNumber >= row.Number).ToList();
                                if (list == null || list.Count == 0)
                                {
                                    PrettyConsole.PrintError("Ayat not found.");
                                    return;
                                }
                                copy = string.Join("\n", list.Select(row => row.Ayah + " \u06DD\n" + row.Translation));
                                json = JsonConvert.SerializeObject(new List<AyahTranslationSurahModel> { new AyahTranslationSurahModel(list.FirstOrDefault(), surahs), new AyahTranslationSurahModel(list.LastOrDefault(), surahs) }, Formatting.Indented);
                            }
                            else
                            {
                                var list = ayat.Where(ayah => ayah.SurahId == surahNumber && ayahNumber <= ayah.Number && endAyahNumber >= ayah.Number).ToList();
                                if (list == null || list.Count == 0)
                                {
                                    PrettyConsole.PrintError("Ayat not found.");
                                    return;
                                }
                                copy = string.Join("\n", list.Select(ayah => ayah.Ayah + " \u06DD "));
                                json = JsonConvert.SerializeObject(new List<AyahModel> { new AyahSurahModel(list.FirstOrDefault(), surahs), new AyahSurahModel(list.LastOrDefault(), surahs) }, Formatting.Indented);
                            }
                            Clipboard.SetText(copy);
                            PrettyConsole.PrintColor(json, ConsoleColor.Yellow);
                        }
                        else // single-line copy
                        {
                            var ayahModel = ayat.FirstOrDefault(ayah => ayah.SurahId == surahNumber && ayah.Number == ayahNumber);
                            if (ayahModel == null)
                            {
                                PrettyConsole.PrintError("Ayah not found.");
                                return;
                            }
                            if (args.Length > 2 && args[2].Equals("t"))
                            {
                                var copy = ayahModel.Ayah;
                                Clipboard.SetText(copy);
                                PrettyConsole.PrintColor(new AyahSurahModel(ayahModel, surahs), ConsoleColor.Yellow);
                            }
                            else
                            {
                                var copy = ayahModel.Ayah + "\n" + translation;
                                Clipboard.SetText(copy);
                                PrettyConsole.PrintColor(new AyahSurahModel(ayahModel, surahs), ConsoleColor.Yellow);
                            }
                        }
                    }
                    else CopySurah(args);
                }
                else PrettyConsole.PrintError("Surah not found.");
            }
        }
    }
}