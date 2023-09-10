using CliFramework;
using Newtonsoft.Json;
using QuranCopyCore.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace QuranCopyCore.Managers
{
    internal class QuranCopyCoreFileManager : FileManager
    {
        public static string SettingsFilePath
        {
            get => GetDictionaryFilePath("settings.json");
        }

        private Settings settings;
        public Settings Settings
        {
            get
            {
                settings ??= GetObject<Settings>(SettingsFilePath);
                if (settings == null)
                {
                    PrettyConsole.PrintError("Could not parse settings JSON file.");
                    return null;
                }
                else
                {
                    settings.replace ??= new Dictionary<string, string>();
                    if (settings.useArabize && string.IsNullOrEmpty(settings.arabizePath))
                        settings.arabizePath = GetArabizePath();
                    return settings;
                }
            }
            set
            {
                settings = value;
                SetObject(SettingsFilePath, settings, Formatting.Indented);
            }
        }

        private static string GetArabizePath()
        {
            string[] pathDirs = Environment.GetEnvironmentVariable("PATH").Split(';');
            string fooExecutableName = "arabize.exe";
            foreach (string pathDir in pathDirs)
            {
                string fullPath = Path.Combine(pathDir, fooExecutableName);
                if (File.Exists(fullPath))
                    return fullPath;
            }
            return null;
        }

        public static string SurahsFilePath
        {
            get => GetFilePath("surahs.xml");
        }

        private IEnumerable<SurahModel> surahs;
        public IEnumerable<SurahModel> Surahs
        {
            get
            {
                surahs ??= GetObjectXml<SurahModel>(SurahsFilePath);
                if (surahs == null) PrettyConsole.PrintError("Could not parse surahs XML file.");
                return surahs;
            }
        }

        public static string AyatFilePath
        {
            get => GetFilePath("ayat.xml");
        }

        private IEnumerable<AyahModel> ayat;
        public IEnumerable<AyahModel> Ayat
        {
            get
            {
                ayat ??= GetObjectXml<AyahModel>(AyatFilePath);
                if (ayat == null) PrettyConsole.PrintError("Could not parse ayat XML file.");
                return ayat;
            }
        }

        public static string TranslationFilePath
        {
            get => GetFilePath("translation.xml");
        }

        private IEnumerable<TranslationModel> translation;
        public IEnumerable<TranslationModel> Translation
        {
            get
            {
                translation ??= GetObjectXml<TranslationModel>(TranslationFilePath);
                if (translation == null) PrettyConsole.PrintError("Could not parse translation XML file.");
                return translation;
            }
        }

        public IEnumerable<AyahTranslationModel> AyatTranslation
        {
            get 
            {
                var ayat = Ayat;
                var translation = Translation;
                if (ayat != null && translation != null)
                {
                    return ayat.Join(translation, ayah => ayah.AyahId, translation => translation.AyahId,
                                            (ayah, translation) => new AyahTranslationModel()
                                            {
                                                Ayah = ayah.Ayah,
                                                Translation = translation.Translation,
                                                SurahId = ayah.SurahId,
                                                Number = ayah.Number
                                            });
                }
                else return null;
            }
        }

        public void Reload() => settings = null;
    }
}
