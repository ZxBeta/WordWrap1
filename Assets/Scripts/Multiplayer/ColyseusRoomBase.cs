using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColyseusRoomBase
{
    public class LobbyRoomPlayerInfo
    {
        public bool oppJoinedLobby;
        public string oppJoinedName;
        public string oppJoinedAvatar;
        public Player[] gamePlayerList;
    }

    public class GameRoomPlayerInfo
    {
        public string roomId;
        public string team;
        public int userIndex;
        public string type;
        public bool hasTimer;
        public int noOfLevels;
        public int totalTime;
        public int minplayersToMatch;
    }

    public class PlayerSumbitData
    {
        public bool addedWord;
        public string wordMade;
        public List<AddedLetter> addedLettersList;
       // public List<string> letterFrequency;
        public int score;
        public int dbID;
        public int userIndex;
    }

    public class RoundWinner
    {
        public string avatar;
        public int dbId;
        public string userName;
        public int score;
     
    }

    public class GameWinner
    {
        public string avatar;
        public int dbId;
        public string userName;
        public int roundWon;
    }

    public class AddedLetter
    {
        public string letter;
        public Vector2 posInGrid;
    }

    public class PlayerRoundDetails
    {
        public int dbID;
        public int userIndex;
        public int score;
        public int currentRound;
        public int totalWinCount;

    }

    public class ChallengeRecived
    {
        public string roomId;
        public int userIndex;
        public string senderUserName;
        public int senderdbId;
        public int noOfLevels;
        public int totalTime;
        public int minplayersToMatch;
        public bool hasTimer;
        public string type;
    }

    public class PlayerLeftLobby
    {
        public string avatar;
        public string userName;
        public int index;
        public int dbId;
        public bool consented;
    }

    public class Letters
    {
        public Object Object;
    }


}
