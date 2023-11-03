
using System;

public class UserDetailsBase
{
    public class UserDetails
    {
        public string username;
        public string email;
        public string mode;
        public string mode_id;
        public string avatar;
    }

    public class GetUserInfo
    {
        public int id;
        public string username;
        public string email;
        public string password;
        public object access_token;
        public string mode;
        public string mode_id;
        public string role_type;
        public string avatar;
        public DateTime createdAt;
        public DateTime updatedAt;
        public bool emailVerified;
    }

    public class AvatarUpdate
    {
        public int id;
        public string avatar;

    }
    public class DailyPuzzleScoreDetails
    {
        public int user_id;
        public int wins;
        public int loss;
        public int score;
        public int todayScore;
        public string avatar;
        public DateTime puzzlePlayedAt;
    }

    public class MultiplayerScoreDetails
    {
        public int user_id;
        public int wins;
        public int loss;
        public int score;
        public string avatar;
    }

    public class ChallengeRequestDetails
    {
        public int minplayersToMatch;
        public int noOfLevels;
        public int totalTime;
        public bool hasTimer;
        public string senderUserName;
        public int senderdbId;
        public string reciverUserName;
        public int recieverdbId;
    }

    public class InAppMessageBase
    {
        public int id;
        public string message;
        public DateTime createdAt;
        public DateTime updatedAt;
    }


}





