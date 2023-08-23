using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

namespace QuranCopy
{
    public static class FileManager
    {
        static readonly string surahsFileName = "surahs.xml";

        static readonly string ayatFileName = "ayat.xml";

        static readonly string translationFileName = "translation.xml";

        static string arabizePath = null;

        public static string SurahsFilePath
        {
            get
            {
                return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, surahsFileName);
            }
        }

        public static string AyatFilePath
        {
            get
            {
                return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ayatFileName);
            }
        }

        public static string TranslationFilePath
        {
            get
            {
                return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, translationFileName);
            }
        }

        public static IEnumerable<XElement> Surahs
        {
            get
            {
                return XDocument.Load(SurahsFilePath).Root.Elements();
            }
        }

        public static IEnumerable<XElement> Ayat
        {
            get
            {
                return XDocument.Load(AyatFilePath).Root.Elements();
            }
        }

        public static IEnumerable<XElement> Translation
        {
            get
            {
                return XDocument.Load(TranslationFilePath).Root.Elements();
            }
        }

        public static string ArabizePath
        {
            get
            {
                if (arabizePath == null) arabizePath = FindArabizePath();
                return arabizePath;
            }
            set
            {
                arabizePath = value;
            }
        }

        private static string FindArabizePath()
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

    }
}
