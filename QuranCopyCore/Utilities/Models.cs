using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

namespace QuranCopyCore.Utilities
{
    public class SurahModel
    {
        public int SurahId;
        public int AyahCount;
        public int AyahId;
        public string Name;
        public string TransliterationName;
        public string EnglishName;

        public override string ToString() => JsonConvert.SerializeObject(this, Formatting.Indented);
    }

    public class AyahModel
    {
        public int AyahId;
        public int Number;
        public int SurahId;
        public string Ayah;

        public override string ToString() => JsonConvert.SerializeObject(this, Formatting.Indented);
    }

    public class AyahSurahModel : AyahModel
    {
        public string SurahName;

        public AyahSurahModel(AyahModel model, IEnumerable<SurahModel> surahs)
        {
            Ayah = model.Ayah;
            AyahId = model.AyahId;
            Number = model.Number;
            SurahId = model.SurahId;
            SurahName = surahs.Where(surah => surah.SurahId == SurahId).FirstOrDefault()?.TransliterationName;
        }
    }


    public class AyahTranslationModel
    {
        public int AyahId;
        public int Number;
        public int SurahId;
        public string Ayah;
        public string Translation;

        public override string ToString() => JsonConvert.SerializeObject(this, Formatting.Indented);
    }

    public class AyahTranslationSurahModel : AyahTranslationModel
    {
        public string SurahName;

        public AyahTranslationSurahModel(AyahTranslationModel model, IEnumerable<SurahModel> surahs)
        {
            Ayah = model.Ayah;
            AyahId = model.AyahId;
            Number = model.Number;
            SurahId = model.SurahId;
            Translation = model.Translation;
            SurahName = surahs.Where(surah => surah.SurahId == SurahId).FirstOrDefault()?.TransliterationName;
        }
    }

    public class TranslationModel
    {
        public int AyahId;
        public string Translation;

        public override string ToString() => JsonConvert.SerializeObject(this, Formatting.Indented);
    }
}