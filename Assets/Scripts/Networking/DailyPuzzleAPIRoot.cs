using System;
using System.Collections.Generic;

[Serializable]
public class DailyPuzzleAPIRoot
{
    public int id;
    public string dailypuzzle;
    public int target_score;
    public string list_of_words;
    public DateTime date_of_assigning;
    public DateTime createdAt;
    public DateTime updatedAt;
    public List<DailPuzzleItems> dailPuzzlesLettersList = new List<DailPuzzleItems>();
    public List<string> words = new List<string>();

}

[Serializable]
public class DailPuzzleItems
{
    public List<string> dailyPuzzleLetter = new List<string>();

}

