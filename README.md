# Quran Copy

This is a command to copy Ayat from the Quran. 

To use it, place the path of the `QuranCopy` directory in `Path` for System Variables.

To recompile, use `gcc -o surah-checker surah-checker.c`.

## How to use:

Pass a Surah number and an Ayah number to have the Ayah's Arabic Unicode text copied to clipboard.

```
C:\> quran 18 1
```

This copies Ayah 1 of Al-Kahf:

> بِسْمِ ٱللَّهِ ٱلرَّحْمَـٰنِ ٱلرَّحِيمِ ٱلْحَمْدُ لِلَّهِ ٱلَّذِىٓ أَنزَلَ عَلَىٰ عَبْدِهِ ٱلْكِتَـٰبَ وَلَمْ يَجْعَل لَّهُۥ عِوَجَا ۜ

Pass a Surah name to get the Surah number. If the spelling is not correct, the closest Surah name (by Levenshtein distance) is used.

```
C:\> quran al-mlk
Al-Mulk
67
```

Pass a Surah name and an Ayah number to have the Ayah's Arabic Unicode text copied to clipboard. Again, spelling is checked.

```
C:\> quran al-mlk 3
Al-Mulk
```

This copies Ayah 3 of Al-Mulk:

> ٱلَّذِى خَلَقَ سَبْعَ سَمَـٰوَٰتٍ طِبَاقًا ۖ مَّا تَرَىٰ فِى خَلْقِ ٱلرَّحْمَـٰنِ مِن تَفَـٰوُتٍ ۖ فَٱرْجِعِ ٱلْبَصَرَ هَلْ تَرَىٰ مِن فُطُورٍ
