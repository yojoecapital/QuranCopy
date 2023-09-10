using CliFramework;
using QuranCopyCore.Managers;
using System;

namespace QuranCopyCore
{
    internal class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            QuranCopyCoreFileManager fileManager = new();
            SearchManager searchManager = new(fileManager);
            CommandManager commandManager = new(searchManager);
            Repl repl = new();
            repl.AddCommand(
                args => args.Length > 1 && args[0].Equals("ar?"),
                commandManager.ArabicLookup,
                "ar? [text]",
                "Search for Arabic [text]."
            );
            repl.AddCommand(
                args => args.Length > 1 && args[0].Equals("en?"),
                commandManager.EnglishLookup,
                "en? [text]",
                "Search for translation [text]."
            );
            repl.AddCommand(
                args => args.Length == 1 && (args[0].Equals("open") || args[0].Equals("o")),
                commandManager.OpenSettings,
                "open (o)",
                "Open the settings JSON file."
            );
            repl.AddCommand(
                args => args.Length == 1 && (args[0].Equals("reload") || args[0].Equals("r")),
                commandManager.Reload,
                "reload (r)",
                "Reload the settings JSON file."
            );
            repl.AddCommand(
                args => args.Length == 1,
                commandManager.CopySurah,
                "[surah-number | surah-name]",
                "Display metadata for the surah."
            );
            repl.AddCommand(
                args => args.Length > 1,
                commandManager.CopyAyah,
                "[surah-number | surah-name] [ayah-number] [t?]",
                "Copy the ayah.\nThe optional [t?] will include the translation."
            );
            repl.AddDescription(
                "[surah-number | surah-name] [ayah-number] [ayah-number] [t?]",
                "Copy the range of ayat.\nThe optional [t?] will include the translation."
            );
            repl.Run(args, true);
        }
    }
}