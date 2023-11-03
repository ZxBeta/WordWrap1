using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuPanel : MonoBehaviour
{
    public GameObject SettingsPanel;
    public GameObject multiPlayerPanel;
    public GameObject loginPopup;
    public GameObject bgSoundOffIcon;
    public GameObject effectSoundOffIcon;
    public GameObject dailyTutorialObj;
    public GameObject multiTutorialObj;
    

    private void Start()
    {
        AudioSource a = GameObject.Find("BG").GetComponent<AudioSource>();
        // PlayerPrefs.SetInt("watchedDaily", 0);
        Globals.gameEnded = false;

        if (PlayerPrefs.HasKey("BgSound"))
        {
         
            if (PlayerPrefs.GetInt("BgSound") == 1)
            {
                a.volume = 0;

            }
               
            else if(PlayerPrefs.GetInt("BgSound") == 2)
            {

                a.volume = 1;
            }
               

        }

        else
        {

            a.volume = 1;
            PlayerPrefs.SetInt("BgSound", 2);
        }

        if (PlayerPrefs.HasKey("effectSound"))
        {
            if (PlayerPrefs.GetInt("effectSound") == 1)
            {
                AudioHandler.instance.oneShotAudioSrc.volume = 0;

            }
            else if(PlayerPrefs.GetInt("effectSound") == 2)
            {   
                AudioHandler.instance.oneShotAudioSrc.volume = 1;
            }

        }
        else
        {
            AudioHandler.instance.oneShotAudioSrc.volume = 1;
            effectSoundOffIcon.SetActive(false);
            PlayerPrefs.SetInt("effectSound", 2);
        }
    }

    private void Update()
    {
        if (!SettingsPanel.activeInHierarchy)
            return;

        AudioSource a = GameObject.Find("BG").GetComponent<AudioSource>();

        if (a.volume == 0)
        {
          
            bgSoundOffIcon.SetActive(true);
        }

        else if(a.volume  == 1)
        {
            bgSoundOffIcon.SetActive(false);
        }

        if (AudioHandler.instance.oneShotAudioSrc.volume == 0)
        {
            effectSoundOffIcon.SetActive(true);
        }

        else if (AudioHandler.instance.oneShotAudioSrc.volume == 1)
        {
        
            effectSoundOffIcon.SetActive(false);
        }

    }

    public void OnClickSettingsButton()
    {
        SettingsPanel.SetActive(true);
    }

    public void StartPracticePuzzle()
    {
        if (!SaveLoadSystem.WatchedDailyPuzzle())
        {
            dailyTutorialObj.SetActive(true);
   
            return;
        }

        Globals.isPractisePuzzle = true;
        SceneLoader.LoadScene(SceneLoader.Scene.GameSceneSingleplayer);
    }

    public void StartMakeWord()
    {
        if (!Globals.isLoggedIn )
        { loginPopup.SetActive(true); return; }

        if (!SaveLoadSystem.WatchedMultiPuzzle())
        {
            multiTutorialObj.SetActive(true);
        
            return;
        }

        Globals.isPractisePuzzle = false;

        multiPlayerPanel.SetActive(true);
    }

    public void StartDailyWordPuzzle()
    {
        if (!Globals.isLoggedIn )
        { loginPopup.SetActive(true); return; }

        if (!SaveLoadSystem.WatchedDailyPuzzle())
        {
            dailyTutorialObj.SetActive(true);

            return;
        }
          

        if(APIDataContainer.instance.dailyPuzzle.dailyPuzzleAPIRootList.Count < 1)
        { ShowMessage.instance.Show("No daily puzzle found for today!"); return; }    

        Globals.isPractisePuzzle = false;
        
        SceneLoader.LoadScene(SceneLoader.Scene.GameSceneSingleplayer);
    }

    public void WatchTutorial(string value)
    {
        if (value == "daily")
            PlayerPrefs.SetInt("watchedDaily", 1);
        if (value == "multi")
            PlayerPrefs.SetInt("watchedMulti", 1);

        BGSoundMuteTutorial();
    }


    public void AppleScene()
    {
        SceneLoader.LoadScene(SceneLoader.Scene.SampleScene);
    }

    public void GuestLogin(GameObject enterName)
    {
        enterName.SetActive(true);
    }

    public void SetGuestName(TMP_InputField nname)
    {
        Globals.UserName = nname.text;
        SaveLoadSystem.SaveGuestDetails();
        APIManager.instance.GuestLogin();
    }

    public void LogOut()
    {
        APIManager.instance.LogOut();
    }

    public void DeleteAccount()
    {
        APIManager.instance.DeleteAccount();
    }

    public void ButtonSoundMute()
    {
  
        if (AudioHandler.instance.oneShotAudioSrc.volume == 0)
        {
            PlayerPrefs.SetInt("effectSound", 2);
            AudioHandler.instance.oneShotAudioSrc.volume = 1;

        }
          
        else
        {
            PlayerPrefs.SetInt("effectSound", 1);
            AudioHandler.instance.oneShotAudioSrc.volume = 0;
    
        }
         

    }

    public void BGSoundMute()
    {
        AudioSource a = GameObject.Find("BG").GetComponent<AudioSource>();

        if (a.volume == 0)
        {
            PlayerPrefs.SetInt("BgSound", 2);
            a.volume = 1;
           

        }

        else
        {
            PlayerPrefs.SetInt("BgSound", 1);
            a.volume = 0;
  
        }

    }

    public void BGSoundMuteTutorial()
    {
        AudioSource a = GameObject.Find("BG").GetComponent<AudioSource>();
        a.volume = 0;

    }

    public void BGSoundOnTutorial()
    {
        AudioSource a = GameObject.Find("BG").GetComponent<AudioSource>();
        if (PlayerPrefs.GetInt("BgSound") == 2)
            a.volume = 1;

    }
}
