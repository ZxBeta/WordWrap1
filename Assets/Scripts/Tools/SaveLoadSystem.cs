using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SaveLoadSystem 
{
   
    public static void SaveUserDetails()
    {
        PlayerPrefs.SetString("modeID", Globals.modeID);
        PlayerPrefs.SetString("userId", Globals.modeID);
    }

    public static void UnSaveUserID()
    {
        PlayerPrefs.SetString("userId", "");
        PlayerPrefs.SetString("modeID", "");
    }

    public static SaveDetails LoadUserDetails()
    {
        if (PlayerPrefs.GetString("modeID") != "" || PlayerPrefs.GetString("modeID") == null)
        {
            SaveDetails s = new SaveDetails();
            s.userId = PlayerPrefs.GetString("userId");
            s.modeID = PlayerPrefs.GetString("modeID");
            return s;
        }

        return null;
    }


    public static void SaveGuestDetails()
    {
        PlayerPrefs.SetString("guestName", Globals.UserName);
        PlayerPrefs.SetString("guestAvatar", Globals.avatar);
    }

    public static Dictionary<string, string> LoadGuestDetails()
    {
        Dictionary<string, string> guestDetails = new Dictionary<string, string>();

        if (!PlayerPrefs.HasKey("guestName"))
            return null;

        if (PlayerPrefs.GetString("guestName") != "")
        {
            guestDetails.Add("guestName", PlayerPrefs.GetString("guestName"));
            guestDetails.Add("guestAvatar", PlayerPrefs.GetString("guestAvatar"));
            return guestDetails;
        }
        else
            return null;
    }

    public static void SaveInAppMsgCount(int count)
    {
        PlayerPrefs.SetInt("MsgCount", count);
    }

    public static int LoadInAppMsgCount()
    {
        if(PlayerPrefs.HasKey("MsgCount"))
        {
            return PlayerPrefs.GetInt("MsgCount");
        }

        return -1;
    }

    public static bool WatchedDailyPuzzle()
    {
        if (PlayerPrefs.HasKey("watchedDaily"))
        {
            if (PlayerPrefs.GetInt("watchedDaily") == 1)
                return true;
            else return false;

        }
        else
            return false;       
    }

    public static bool WatchedMultiPuzzle()
    {
        if (PlayerPrefs.HasKey("watchedMulti"))
        {
            if (PlayerPrefs.GetInt("watchedMulti") == 1)
                return true;
            else return false;

        }
        else
            return false;
    }

 

}

public class SaveDetails
{
    public string userId;
    public string modeID;

}
