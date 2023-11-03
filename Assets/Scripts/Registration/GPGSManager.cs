using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;

public class GPGSManager : MonoBehaviour
{

#if UNITY_ANDROID
    private PlayGamesClientConfiguration clientConfiguration;


    private void Start()
    {
        ConfigureGPS();
    }

    internal void ConfigureGPS()
    {
        clientConfiguration = new PlayGamesClientConfiguration.Builder().Build();
    }

    internal void SignIntoGPGS(SignInInteractivity interactivity, PlayGamesClientConfiguration configuration)
    {
        configuration = clientConfiguration;
        PlayGamesPlatform.Activate();

        PlayGamesPlatform.Instance.Authenticate(interactivity, (code) =>
        {
            if(code == SignInStatus.Success)
            {
                Globals.loggedInMode = "google";      
                Globals.UserName = Social.localUser.userName; 
                Globals.modeID = Social.localUser.id;
             
                if (Globals.email == null || Globals.email == "")
                    Globals.email = Globals.UserName + "@playgoogle.com";
                APIManager.instance.PostUserLoginDetails
                (Globals.UserName, Globals.email, Globals.loggedInMode,Globals.modeID);               
            }

            else
            {
                LoadingCircle.instance.Loading(false);
                ShowMessage.instance.Show("Failed to Authenticate, Reason " + code);
           
           
            }
        });
    }

    public void SignIn()
    {
        if (Globals.isLoggedIn)
            return;

        LoadingCircle.instance.Loading(true);
        SignIntoGPGS(SignInInteractivity.CanPromptAlways, clientConfiguration);
    }
    
    public void SignOut()
    {
        PlayGamesPlatform.Instance.SignOut();
    }

#endif

}
