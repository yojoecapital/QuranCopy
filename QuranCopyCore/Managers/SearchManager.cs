using QuranCopyCore.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Transactions;
using System.Xml.Linq;

namespace QuranCopyCore.Managers
{
    internal class SearchManager
    {
        public readonly QuranCopyCoreFileManager fileManager;

        public SearchManager(QuranCopyCoreFileManager fileManager) =>
            this.fileManager = fileManager;

        public IEnumerable<string> TextLookup(Settings settings, string arg, bool searchTranslation)
        {
            var iterator = fileManager.AyatTranslation;
            var surahs = fileManager.Surahs;
            if (iterator != null && surahs != null)
            {
                foreach (var row in iterator)
                {
                    var number = row.Number;
                    var text = searchTranslation ? row.Translation.ToLower() : Helpers.RemoveAccents(row.Ayah, settings.replace, settings.ignoreAccents);
                    var index = text.IndexOf(arg);
                    if (index != -1)
                    {
                        var surahId = row.SurahId;
                        var surahName = surahs.FirstOrDefault(surah => surah.SurahId == surahId)?.TransliterationName;
                        string ayahText, translationText;
                        if (searchTranslation)
                        {
                            ayahText = Helpers.RemoveAccents(row.Ayah, settings.replace, settings.ignoreAccents).Peekify(0, settings.peek, true);
                            translationText = text.Peekify(index, settings.peek, false);
                        }
                        else
                        {
                            ayahText = text.Peekify(index, settings.peek, true);
                            translationText = row.Translation.Peekify(0, settings.peek, false);
                        }
                        yield return ayahText + "\n" + translationText + "\n\u2192 Ayah " + number + " from " + "(" + surahId + ") " + surahName + "\n";
                    }
                }
            } 
            else yield break;
        }
    }
}
