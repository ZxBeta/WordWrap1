using UnityEngine;
using Facebook.Unity;
using System.Collections.Generic;
using Facebook.MiniJSON;
using TMPro;
using System.Threading.Tasks;
using System;

public class FacebookManager : MonoBehaviour
{


    private void Awake()
    {
       

        if (!FB.IsInitialized)
        {
            // Initialize the Facebook SDK
            FB.Init(InitCallback, OnHideUnity);
        }
        else
        {
            // Already initialized, signal an app activation App Event
            FB.ActivateApp();
        }
    }


    private void InitCallback()
    {
        if (FB.IsInitialized)
        {
            // Signal an app activation App Event
            FB.ActivateApp();
            // Continue with Facebook SDK
            // ...
      
        }
        else
        {
            Debug.Log("Failed to Initialize the Facebook SDK");
        }
    }

    private void OnHideUnity(bool isGameShown)
    {
        if (!isGameShown)
            Time.timeScale = 0;
        else
            Time.timeScale = 1;
    }

    public void FacebookLogin()
    {
        if(!Globals.isLoggedIn)
        {
            var permissions = new List<string>() { "public_profile", "email", "user_friends" };
            FB.LogInWithReadPermissions(permissions, AuthCallback);
        }
     
    }

    private void AuthCallback(ILoginResult result)
    {
        if (FB.IsLoggedIn)
        {
            var aToken = AccessToken.CurrentAccessToken;
            Debug.Log(aToken.UserId);
            foreach (string perm in aToken.Permissions)
                Debug.Log(perm);
            GetUserInfo();
           
   
        }
        else
        {
            Debug.Log("User cancelled login");
            ShowMessage.instance.Show("User cancelled login");
            LoadingCircle.instance.Loading(false);
        }
    }
    public void FacebookLogout()
    {
        FB.LogOut();
    }

    private void GetUserInfo()
    {
        string query = "/me?fields=id,name,email";

        try
        {
            LoadingCircle.instance.Loading(true);
            FB.API(query, HttpMethod.GET, delegate (IGraphResult result)
            {
                var details = (Dictionary<string, object>)result.ResultDictionary;

                if (result.ResultDictionary.ContainsKey("name"))
                    Globals.UserName = (string)result.ResultDictionary["name"];

                if (result.ResultDictionary.ContainsKey("id"))
                    Globals.modeID = (string)result.ResultDictionary["id"];

                if (result.ResultDictionary.ContainsKey("email"))
                    Globals.email = (string)result.ResultDictionary["email"];

                if (Globals.email == null || Globals.email == "")
                    Globals.email = Globals.UserName + "@facebook.com";

                Globals.loggedInMode = "facebook";

                APIManager.instance.PostUserLoginDetails
                 (Globals.UserName, Globals.email, Globals.loggedInMode, Globals.modeID);


            });
        }

        catch(Exception e)
        {
            LoadingCircle.instance.Loading(false);
            ShowMessage.instance.Show(e.ToString());
        }
    }

    public void GetFriendsPlayingThisGame()
    {
        string query = "/me/friends";
        FB.API(query, HttpMethod.GET, result =>
        {
            var dictionary = (Dictionary<string, object>)Json.Deserialize(result.RawResult);
            var friendsList = (List<object>)dictionary["data"];
            //foreach (var dict in friendsList)
            //    FriendsText.text += ((Dictionary<string, object>)dict)["name"];
        });
    }

    private void GetProfilePicture()
    {
        var get = HttpMethod.GET;

        FB.API("me/picture?type=circle", get, delegate (IGraphResult result)
        {
            if (result.Texture != null)
            {
              //  print(result.Texture);
               // Globals.avatar = ImageUtility.Texture2DToBase64(result.Texture);
             

                return;
            }
        });

       
    }


}