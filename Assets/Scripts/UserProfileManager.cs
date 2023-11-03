using System.Collections;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using System.Text;

public class UserProfileManager : MonoBehaviour
{
    [SerializeField]
    TMP_Text userName;

    [SerializeField]
    Image userProfilePicture;

    [SerializeField]
    Image userPicInEditProfile;

    [SerializeField]
    Image userLogInWith;

    [SerializeField]
    TMP_Text userNameInEditProfile;

    [SerializeField]
    TMP_Text userWinningPercentage;

    [SerializeField]
    TMP_Text userRank;

    public GameObject guestLoginObj;
    public GameObject loginWithObj;

    public Sprite[] loginWithSprite;

    public static Action<string, Sprite> eventUpdateDetails;
    public static Action<Sprite> eventUpdateDP;
    public static Action<LeaderBoardRoot> eventUpdateProgress;
    public static Action loginButtonEvent;

    public InGameAvatarPanel avatarPanel;

    private void Awake()
    {
        eventUpdateDetails = UpdateDetails;
        eventUpdateDP = UpdateDP;
        eventUpdateProgress = UpdateProgress;
        loginButtonEvent = LogInBtn;
    }



    void UpdateDetails(string name, Sprite dp)
    {
        userName.text = name;
        userNameInEditProfile.text = "Hi " + name + "!";

        if (dp != null)
        {
            userProfilePicture.sprite = dp;
            userPicInEditProfile.sprite = dp;
        }


        avatarPanel.APIChangeAvtar(int.Parse(Globals.avatar));

        UpdateLoginWith();

    }

    void UpdateDP(Sprite dp)
    {
        if (dp != null)
        {
            userProfilePicture.sprite = dp;
            userPicInEditProfile.sprite = dp;
        }
           
    }

    void UpdateProgress(LeaderBoardRoot l)
    {
        var winningPerecentage = (l.wins *100) / (l.wins + l.loss);
        userWinningPercentage.text = winningPerecentage + "%";
        userRank.text = Globals.rank.ToString();
    }

    void LogInBtn()
    {
        guestLoginObj.SetActive(Globals.isGuestLoggedIn);
        loginWithObj.SetActive(Globals.isLoggedIn);
    }

    void UpdateLoginWith()
    {
        if (Globals.loggedInMode == "facebook")
            userLogInWith.sprite = loginWithSprite[0];
        else if (Globals.loggedInMode == "google")
            userLogInWith.sprite = loginWithSprite[1];
        else if (Globals.loggedInMode == "apple")
            userLogInWith.sprite = loginWithSprite[2];
    }

}


