# Quran Copy

This is a command to copy Ayat from the Quran. 

To access it easier, place the path of the `QuranCopy` directory in `Path` for Environment Variables.

To recompile, use `csc quran.cs`.

## How to Copy:

Pass a Surah number and an Ayah number to have the Ayah's Arabic Unicode text copied to clipboard.

```
$ quran 18 1
Copied ayah 1 of (18) Al-Kahf
```

This copies Ayah 1 of Al-Kahf:

> بِسْمِ ٱللَّهِ ٱلرَّحْمَـٰنِ ٱلرَّحِيمِ ٱلْحَمْدُ لِلَّهِ ٱلَّذِىٓ أَنزَلَ عَلَىٰ عَبْدِهِ ٱلْكِتَـٰبَ وَلَمْ يَجْعَل لَّهُۥ عِوَجَا ۜ

Pass a Surah name to get its metadata. If the spelling is not correct, the closest Surah name (by Levenshtein distance) is used.

```
$ quran al-mlk
(67) Al-Mulk
Ayat Count: 30
```

Pass a Surah name and an Ayah number to have the Ayah's Arabic Unicode text copied to clipboard. 

```
$ quran al-mlk 3
Copied ayah 3 of (67) Al-Mulk
```

This copies Ayah 3 of Al-Mulk:

> ٱلَّذِى خَلَقَ سَبْعَ سَمَـٰوَٰتٍ طِبَاقًا ۖ مَّا تَرَىٰ فِى خَلْقِ ٱلرَّحْمَـٰنِ مِن تَفَـٰوُتٍ ۖ فَٱرْجِعِ ٱلْبَصَرَ هَلْ تَرَىٰ مِن فُطُورٍ

Pass a Surah name (or number) as well as a start Ayah number and an end Ayah Number to have the range of Ayat copied to clipboard.

```
$ quran al-mlk 1 10
Copied ayat 1 through 10 of (67) Al-Mulk
```

> This copies Ayat 1 through 10 of Al-Mulk:
>
> بِسْمِ ٱللَّهِ ٱلرَّحْمَـٰنِ ٱلرَّحِيمِ تَبَـٰرَكَ ٱلَّذِى بِيَدِهِ ٱلْمُلْكُ وَهُوَ عَلَىٰ كُلِّ شَىْءٍ قَدِيرٌ ۝ 
>
> ٱلَّذِى خَلَقَ ٱلْمَوْتَ وَٱلْحَيَوٰةَ لِيَبْلُوَكُمْ أَيُّكُمْ أَحْسَنُ عَمَلًا ۚ وَهُوَ ٱلْعَزِيزُ ٱلْغَفُورُ ۝ 
>
> ٱلَّذِى خَلَقَ سَبْعَ سَمَـٰوَٰتٍ طِبَاقًا ۖ مَّا تَرَىٰ فِى خَلْقِ ٱلرَّحْمَـٰنِ مِن تَفَـٰوُتٍ ۖ فَٱرْجِعِ ٱلْبَصَرَ هَلْ تَرَىٰ مِن فُطُورٍ ۝ 
>
> ثُمَّ ٱرْجِعِ ٱلْبَصَرَ كَرَّتَيْنِ يَنقَلِبْ إِلَيْكَ ٱلْبَصَرُ خَاسِئًا وَهُوَ حَسِيرٌ ۝ 
>
> وَلَقَدْ زَيَّنَّا ٱلسَّمَآءَ ٱلدُّنْيَا بِمَصَـٰبِيحَ وَجَعَلْنَـٰهَا رُجُومًا لِّلشَّيَـٰطِينِ ۖ وَأَعْتَدْنَا لَهُمْ عَذَابَ ٱلسَّعِيرِ ۝ 
>
> وَلِلَّذِينَ كَفَرُوا۟ بِرَبِّهِمْ عَذَابُ جَهَنَّمَ ۖ وَبِئْسَ ٱلْمَصِيرُ ۝ 
>
> إِذَآ أُلْقُوا۟ فِيهَا سَمِعُوا۟ لَهَا شَهِيقًا وَهِىَ تَفُورُ ۝ 
>
> تَكَادُ تَمَيَّزُ مِنَ ٱلْغَيْظِ ۖ كُلَّمَآ أُلْقِىَ فِيهَا فَوْجٌ سَأَلَهُمْ خَزَنَتُهَآ أَلَمْ يَأْتِكُمْ نَذِيرٌ ۝ 
>
> قَالُوا۟ بَلَىٰ قَدْ جَآءَنَا نَذِيرٌ فَكَذَّبْنَا وَقُلْنَا مَا نَزَّلَ ٱللَّهُ مِن شَىْءٍ إِنْ أَنتُمْ إِلَّا فِى ضَلَـٰلٍ كَبِيرٍ ۝ 
>
> وَقَالُوا۟ لَوْ كُنَّا نَسْمَعُ أَوْ نَعْقِلُ مَا كُنَّا فِىٓ أَصْحَـٰبِ ٱلسَّعِيرِ ۝ 

Add the last argument `t` to also copy the translation.

```
$ quran an-naas 1 6 t
Copied ayat 1 through 6 of (114) An-Naas with translation
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
> إِلَـٰهِ ٱلنَّاسِ ۝
>
> the God of people,
>
> مِن شَرِّ ٱلْوَسْوَاسِ ٱلْخَنَّاسِ ۝
>
> from the mischief of every sneaking whis-perer,
>
> ٱلَّذِى يُوَسْوِسُ فِى صُدُورِ ٱلنَّاسِ ۝
>
> who whispers into the hearts of people,
>
> مِنَ ٱلْجِنَّةِ وَٱلنَّاسِ ۝
>
> from jinn and men."

## How to Search:

Use the argument `ar?` followed by Arabic Unicode to search for that sequence in the Quran.

```
$ quran ar? يُوسُف
Searching for: "يوسف"
"...يوسف وموسى وهـرون  و"
→ Ayah 84 from (6) Al-Anaam

"...يوسف لابيه يـابت انى"
→ Ayah 4 from (12) Yusuf
...

26 result(s)
```

Use the argument `en?` followed by English text to search for that sequence in the Quran's translation.

```
$ quran en? calf
Searching for: "calf"
"calf, and thus becam..."
"...واذ وعدنا موسى اربعي"
→ Ayah 51 from (2) Al-Baqara

"calf; turn in repent..."
"...واذ قال موسى لقومهۦ "
→ Ayah 54 from (2) Al-Baqara
...

11 result(s)
```

Add a numerical argument after the `en?` or `ar?` to specify the maximum characters that should be displayed on the console in each search result.
