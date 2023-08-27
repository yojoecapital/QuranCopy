# Quran Copy

- This is a command for Windows Systems copy Quranic Ayat in Arabic Unicode to clipboard. 
- To access it easier, place the path of the `QuranCopy` directory in user's `Path` Environment Variable.
- Quran Copy can either be used as an interactive Interpreter (REPL) or a command-line program. To begin the REPL, just execute `quran.exe` without any arguments.

## Usage

Pass a Surah number and an Ayah number to have the Ayah's Arabic Unicode text copied to clipboard.

```
> 18 1
Copied ayah 1 of (18) Al-Kahf
```

This copies Ayah 1 of Al-Kahf:

> بِسْمِ ٱللَّهِ ٱلرَّحْمَـٰنِ ٱلرَّحِيمِ ٱلْحَمْدُ لِلَّهِ ٱلَّذِىٓ أَنزَلَ عَلَىٰ عَبْدِهِ ٱلْكِتَـٰبَ وَلَمْ يَجْعَل لَّهُۥ عِوَجَا ۜ

Pass a Surah name to get its metadata. If the spelling is not correct, the closest Surah name (by Levenshtein distance) is used.

```
> al-mlk
(67) Al-Mulk
Ayat Count: 30
```

Pass a Surah name and an Ayah number to have the Ayah's Arabic Unicode text copied to clipboard. 

```
> al-mlk 3
Copied ayah 3 of (67) Al-Mulk
```

This copies Ayah 3 of Al-Mulk:

> ٱلَّذِى خَلَقَ سَبْعَ سَمَـٰوَٰتٍ طِبَاقًا ۖ مَّا تَرَىٰ فِى خَلْقِ ٱلرَّحْمَـٰنِ مِن تَفَـٰوُتٍ ۖ فَٱرْجِعِ ٱلْبَصَرَ هَلْ تَرَىٰ مِن فُطُورٍ

Pass a Surah name (or number) as well as a start Ayah number and an end Ayah Number to have the range of Ayat copied to clipboard.

```
> al-mlk 1 3
Copied ayat 1 through 3 of (67) Al-Mulk
```

> This copies Ayat 1 through 10 of Al-Mulk:
>
> بِسْمِ ٱللَّهِ ٱلرَّحْمَـٰنِ ٱلرَّحِيمِ تَبَـٰرَكَ ٱلَّذِى بِيَدِهِ ٱلْمُلْكُ وَهُوَ عَلَىٰ كُلِّ شَىْءٍ قَدِيرٌ ۝ 
>
> ٱلَّذِى خَلَقَ ٱلْمَوْتَ وَٱلْحَيَوٰةَ لِيَبْلُوَكُمْ أَيُّكُمْ أَحْسَنُ عَمَلًا ۚ وَهُوَ ٱلْعَزِيزُ ٱلْغَفُورُ ۝ 
>
> ٱلَّذِى خَلَقَ سَبْعَ سَمَـٰوَٰتٍ طِبَاقًا ۖ مَّا تَرَىٰ فِى خَلْقِ ٱلرَّحْمَـٰنِ مِن تَفَـٰوُتٍ ۖ فَٱرْجِعِ ٱلْبَصَرَ هَلْ تَرَىٰ مِن فُطُورٍ ۝ 
>

Add the last argument `t` to also copy the translation.

```
$ quran an-naas 1 2 t
Copied ayat 1 through 2 of (114) An-Naas with translation
```

> This copies Ayat 1 through 6 of An-Naas with its translation:
>
> بِسْمِ ٱللَّهِ ٱلرَّحْمَـٰنِ ٱلرَّحِيمِ قُلْ أَعُوذُ بِرَبِّ ٱلنَّاسِ ۝
>
> Say, "I seek refuge in the Lord of people,
>
> مَلِكِ ٱلنَّاسِ ۝
>
> the King of people,
>

## How to Search

Use the argument `ar?` followed by Arabic Unicode to search for that sequence in the Quran.

```
Searching for: "يُوسُف"
"...يوسف وموسى وهارون  وكذلك "
"We gave him Isaac and Jac..."
→ Ayah 84 from (6) Al-Anaam

"...يوسف لابيه ياابت انى رايت"
"When Joseph told his fath..."
→ Ayah 4 from (12) Yusuf

"...يوسف واخوتهۦ ءايات للسايل"
"Surely, in Joseph and his..."
→ Ayah 7 from (12) Yusuf

"...يوسف واخوه احب الى ابينا "
"They said [to each other]..."
→ Ayah 8 from (12) Yusuf

4 / 26 result(s).
Use the arrow keys to display the next 4 results or Enter to stop.
```

You can use the arrow keys to cycle through the result pages or press Enter to exit the result display.

Use the argument `en?` followed by English text to search for that sequence in the Quran's translation.

### Search Settings

```json
{
  "peek": 25,
  "resultsPerPage": 4,
  "ignoreAccents": true,
  "useArabize": false,
  "arabizePath": null,
  "replace": {
    "ـ": "ا",
    "هۥ": "ه"
  }
}
```

- `peek`: Defines how many characters to print in each search result
- `resultsPerPage`: Defines how many results to show on each page of search results
- `ignoreAccents`: Whether the diacritics should be stemmed from the Ayat (and search query) while searching
- `useArabize`: Whether the [Arabize](https://github.com/yojoecapital/Arabize) CLI program should be used to transform the search query for the `ar?` prompt. For example `ar? ya_waw_seen_fa`
- `arabizePath`: This is the path to the *directory* containing `arabize.exe`
  - If `useArabize` is `true`,  this will be the path used to execute `arabize.exe`
  - If `useArabize` is `true` and `arabizePath` is `null` the user's `Path` Environment Variable will be searched to try to execute `arabize.exe`
- `replace`: Defines key-value pairs for transformations where the key gets replaced with the value for the `ar?` prompt

## Arguments

- `help` or `h`: Display arguments
- `open` or `o`: Open the settings JSON
- `reload` or `r`: Reload the settings JSON
- `ar? [text]`: Searches for Arabic `[text]`
- `en? [text]`: Searches for translation `[text]`
- `clear` or `cls`: Clear the console screen
- `quit` or `q`: Exit the program

## Building

1. Clone the repository: `git clone https://github.com/yojoecapital/QuranCopy.git`
2. Restore the NuGet Packages using the NuGet CLI: `nuget restore`
3. Build the application using the .NET CLI: `dotnet msbuild`
4. Run the executable located in `Quran/bin`

### Releasing

```
dotnet msbuild --property:Configuration=Release && cd QuranCopy/bin/Release && 7z a QuranCopy.zip * && gh release create v1.0.0 ./QuranCopy.zip -t "v1.0.0" --target main -F ./RELEASE.md && cd ../../..
```

## Contact

For any inquiries or feedback, contact me at [yousefsuleiman10@gmail.com](mailto:yousefsuleiman10@gmail.com).
