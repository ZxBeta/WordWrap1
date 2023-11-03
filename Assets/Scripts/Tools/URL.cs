using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class URL 
{
    public static string dailyPuzzle = "http://52.70.100.138:3000/api/dailypuzzles";
    public static string dailyPuzzleLeaderBoard = "http://52.70.100.138:3000/api/dailyleaderboards";
    public static string todayDailyPuzzleLeaderBoard = "http://52.70.100.138:3000/api/dailyleaderboards?filter[where][puzzlePlayedAt][gt]=" + GetDateTime.GetDate();
    public static string multiplayerLeaderBoard = "http://52.70.100.138:3000/api/multiplayerleaderboards";
    public static string postUserInfo = "http://52.70.100.138:3000/api/modelogin";
    public static string registerUserInfo = "http://52.70.100.138:3000/api/registeruserinfo";
    public static string getUserInfo = "http://52.70.100.138:3000/api/registeruserinfo?filter[where][mode_id]=";
    public static string updateAvatar = "http://52.70.100.138:3000/api/updateAvatar";
    public static string inAppMsg = "http://52.70.100.138:3000/api/inappmessages";
    public static string deleteUserDetails = "http://52.70.100.138:3000/api/users/";

}
