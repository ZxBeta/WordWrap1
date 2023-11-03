using AppleAuth;
using AppleAuth.Enums;
using AppleAuth.Extensions;
using AppleAuth.Interfaces;
using AppleAuth.Native;
using TMPro;
using UnityEngine;

public class AppleLoginManager : MonoBehaviour
{
    private IAppleAuthManager _appleAuthManager;
  

    private void Start()
    {
        if (AppleAuthManager.IsCurrentPlatformSupported)
        {
            // Creates a default JSON deserializer, to transform JSON Native responses to C# instances
            var deserializer = new PayloadDeserializer();
            // Creates an Apple Authentication manager with the deserializer
            this._appleAuthManager = new AppleAuthManager(deserializer);
        }

        else
        {
            print("Apple sign in current platform not supported");
        }
    }

    private void Update()
    {
        
    }


    public void SignInWithApple()
    {

        if (Globals.isLoggedIn || !AppleAuthManager.IsCurrentPlatformSupported)
            return;

        LoadingCircle.instance.Loading(true);

        var loginArgs = new AppleAuthLoginArgs(LoginOptions.IncludeEmail | LoginOptions.IncludeFullName);

        this._appleAuthManager.LoginWithAppleId(
            loginArgs,
            credential =>
            {
                Debug.Log("succeded apple login");
             //   logtext.text += "LOGIN SUCCEED ||| ";
                // If a sign in with apple succeeds, we should have obtained the credential with the user id, name, and email, save it
                //PlayerPrefs.SetString(AppleUserIdKey, credential.User);
                SetUpData(credential.User, credential);    
            },

            error =>
            {
                //  logtext.text += "Error on login";
                ShowMessage.instance.Show("Sign in with Apple failed ");
                LoadingCircle.instance.Loading(false);
                var authorizationErrorCode = error.GetAuthorizationErrorCode();
                ShowMessage.instance.Show("Sign in with Apple failed " + authorizationErrorCode.ToString() + " " + error.ToString());
            });

     
    }


    void SetUpData(string appleUserId, ICredential receivedCredential)
    {
        if (receivedCredential == null)
        {
            //  logtext.text += "NO CREDENTIALS RECEIVE";
            ShowMessage.instance.Show("NO CREDENTIALS RECEIVED\nProbably credential status for " + appleUserId +
                "was Authorized");
            LoadingCircle.instance.Loading(false);
            return;
        }

        var appleIdCredential = receivedCredential as IAppleIDCredential;
        var passwordCredential = receivedCredential as IPasswordCredential;
 
        if (appleIdCredential != null)
        {
            if (appleIdCredential.Email != null)
            {
                Globals.email = appleIdCredential.Email;

            }

            if (appleIdCredential.FullName != null)
            {
                Globals.UserName = appleIdCredential.FullName.GivenName;
            }

           // logtext.text += "user details are " + appleIdCredential.Email + " " + appleIdCredential.FullName.GivenName;
            LoadingCircle.instance.Loading(false);

            Globals.loggedInMode = "apple";
            Debug.Log("succeded apple login user name " + Globals.UserName);

            //APIManager.instance.PostUserLoginDetails
            //(Globals.UserName, Globals.email, Globals.loggedInMode);
          

        }

        else if (passwordCredential != null)
        {
           // logtext.text += "password null ";
           LoadingCircle.instance.Loading(false);
        }

        else
        {
          //  logtext.text += "Unknown credentials for user ";
           LoadingCircle.instance.Loading(false);
            ShowMessage.instance.Show("Unknown credentials for user " + receivedCredential.User);
        }
    }
}
