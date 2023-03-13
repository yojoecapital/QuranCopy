#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <ctype.h>
#include <limits.h>

#define MAX_WORD_LENGTH 50
#define MAX_NUM_WORDS 10000

int levenshtein_distance(char* s1, char* s2) {
    int m = strlen(s1);
    int n = strlen(s2);

    int d[m + 1][n + 1];
    int i, j;

    for (i = 0; i <= m; i++) {
        d[i][0] = i;
    }
    for (j = 0; j <= n; j++) {
        d[0][j] = j;
    }

    for (j = 1; j <= n; j++) {
        for (i = 1; i <= m; i++) {
            if (s1[i - 1] == s2[j - 1]) {
                d[i][j] = d[i - 1][j - 1];
            }
            else {
                int deletion = d[i - 1][j] + 1;
                int insertion = d[i][j - 1] + 1;
                int substitution = d[i - 1][j - 1] + 1;
                int min = (deletion < insertion) ? deletion : insertion;
                min = (min < substitution) ? min : substitution;
                d[i][j] = min;
            }
        }
    }

    return d[m][n];
}

int main(int argc, char *argv[]) {
    if (argc < 2)
    {
        return 1;
    }

    // Open the word list file
    FILE* wordListFile = fopen("surah-names.txt", "r");
    if (!wordListFile) {
        return 1;
    }

    // Load the words from the file into an array
    char words[MAX_NUM_WORDS][MAX_WORD_LENGTH];
    int numWords = 0;
    while (fgets(words[numWords], MAX_WORD_LENGTH, wordListFile) != NULL) {
        // Remove newline character at end of word
        words[numWords][strlen(words[numWords]) - 1] = '\0';
        numWords++;
    }

    // Close the word list file
    fclose(wordListFile);

    char* inputWord = argv[1];

    // Check each word in the word list against the input word
    int i;
    int minDistance = INT_MAX;
    char* closestWord = NULL;
    for (i = 0; i < numWords; i++) {
        int distance = levenshtein_distance(inputWord, words[i]);
        if (distance < minDistance) {
            minDistance = distance;
            closestWord = words[i];
        }
    }

    // Print the closest word in the word list
    printf(closestWord);

    return 0;
}
