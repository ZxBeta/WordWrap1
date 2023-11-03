using System;

[Serializable]
public class LeaderBoardRoot
{
    public int id;
    public int user_id;
    public string username;
    public int wins;
    public int loss;
    public int score;
    public int todayScore = 0;
    public int rank;
    public string profile_pic;
    public DateTime createdAt;
    public DateTime updatedAt;
    //public DateTime puzzlePlayedAt;
}
