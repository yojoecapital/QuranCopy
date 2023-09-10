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
            Repl repl = new()
            {
                pagifyHelp = fileManager.Settings.resultsPerPage
            };
            repl.AddCommand(
                args => args.Length > 1 && args[0].Equals("ar?"),
                commandManager.ArabicLookup
            );
            repl.AddCommand(
                args => args.Length > 1 && args[0].Equals("en?"),
                commandManager.EnglishLookup
            );
            repl.AddCommand(
                args => args.Length == 1 && (args[0].Equals("open") || args[0].Equals("o")),
                commandManager.OpenSettings
            );
            repl.AddCommand(
                args => args.Length == 1 && (args[0].Equals("reload") || args[0].Equals("r")),
                commandManager.Reload
            );
            repl.AddCommand(
                args => args.Length == 1,
                commandManager.CopySurah
            );
            repl.AddCommand(
                args => args.Length > 1,
                commandManager.CopyAyah
            );
            repl.Run(args, true);
        }
    }
}