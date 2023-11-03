using System.Text;
using UnityEngine.UI;
using UnityEngine;
using AppleAuth;
using AppleAuth.Native;
using AppleAuth.Interfaces;
using AppleAuth.Enums;
using AppleAuth.Extensions;
using Newtonsoft.Json.Linq;
using TMPro;

public class AppleManager : MonoBehaviour
{
    private IAppleAuthManager appleAuthManager;


    void Start()
    {

        // If the current platform is supported
        if (AppleAuthManager.IsCurrentPlatformSupported)
        {
            // Creates a default JSON deserializer, to transform JSON Native responses to C# instances
            var deserializer = new PayloadDeserializer();
            // Creates an Apple Authentication manager with the deserializer
            this.appleAuthManager = new AppleAuthManager(deserializer);
        }
    }

    void Update()
    {
        // Updates the AppleAuthManager instance to execute
        // pending callbacks inside Unity's execution loop
        if (this.appleAuthManager != null)
        {
            this.appleAuthManager.Update();
        }
    }

    public void QuickAppleSignIn()
    {
        Debug.Log("Here Signin With Apple");
        if (AppleAuthManager.IsCurrentPlatformSupported && !Globals.isLoggedIn)
        {
            LoadingCircle.instance.Loading(true);
            var quickLoginArgs = new AppleAuthQuickLoginArgs();
            this.appleAuthManager.QuickLogin(
            quickLoginArgs,
            credential =>
            {
                var appleIdCredential = credential as IAppleIDCredential;
                if (appleIdCredential != null)
                {
                    var userId = appleIdCredential.User;
                    Globals.modeID = userId;
                    Globals.loggedInMode = "apple";
                    SaveLoadSystem.SaveUserDetails();
                   APIManager.instance.GetUserprofile(SaveLoadSystem.LoadUserDetails(),true);
                 

                }
            },
            error =>
            {
                var authorizationErrorCode = error.GetAuthorizationErrorCode();
                Debug.Log(authorizationErrorCode);
                FirstAppleSignIn();
                // Quick login failed. The user has never used Sign in With Apple on your app. Go to login screen
            });
        }
        else
        {
            Debug.Log("Platform Not Supported");
        }
    }

    public void FirstAppleSignIn()
    {

        if (AppleAuthManager.IsCurrentPlatformSupported)
        {
            LoadingCircle.instance.Loading(true);
            var loginArgs = new AppleAuthLoginArgs(LoginOptions.IncludeEmail | LoginOptions.IncludeFullName);
            this.appleAuthManager.LoginWithAppleId(
                loginArgs,
                credential =>
                {
                    // Obtained credential, cast it to IAppleIDCredential
                    var appleIdCredential = credential as IAppleIDCredential;
                    if (appleIdCredential != null)
                    {
                        // Apple User ID
                        // You should save the user ID somewhere in the device
                        var userId = appleIdCredential.User;
                        // Email (Received ONLY in the first login)
                        if (appleIdCredential.Email != null)
                        {
                            Globals.email = appleIdCredential.Email;

                        }
                        
                        // Full name (Received ONLY in the first login)
                        var fullName = appleIdCredential.FullName;

                        // Identity token
                        var identityToken = Encoding.UTF8.GetString( 
                            appleIdCredential.IdentityToken,
                            0,
                            appleIdCredential.IdentityToken.Length);

                        // Authorization code
                        var authorizationCode = Encoding.UTF8.GetString(
                            appleIdCredential.AuthorizationCode,
                            0,
                            appleIdCredential.AuthorizationCode.Length);

                        Globals.UserName = fullName.GivenName;
                        Globals.loggedInMode = "apple";
                        Globals.modeID = userId;


                        if (Globals.email == null || Globals.email == "")
                            Globals.email = Globals.UserName + "@apple.com";

                        APIManager.instance.PostUserLoginDetails
                         (Globals.UserName, Globals.email, Globals.loggedInMode, Globals.modeID);

                    }
                },

                error =>
                {
                    // Something went wrong;
                    LoadingCircle.instance.Loading(false);
                    var authorizationErrorCode = error.GetAuthorizationErrorCode();
                    Debug.Log(authorizationErrorCode);
                });
        }
        else
        {
            Debug.Log("Platform Not Supported");
        }
    }





}
