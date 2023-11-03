
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UserDetailsBase;

public class APIManager : MonoBehaviour
{
    public static APIManager instance;
    

    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(gameObject);
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        InitLogin();
    }

    void InitLogin()
    {
        UIManager.instance.loadingPanel.SetActive(true);

        if (Application.platform == RuntimePlatform.WindowsPlayer)
        {
            SaveDetails s = new SaveDetails();
            s.modeID = "107229095512218";
            GetUserprofile(s, false);
            return;
        }


        if (SaveLoadSystem.LoadUserDetails() != null)
        {
            GetUserprofile(SaveLoadSystem.LoadUserDetails(), false);
        }

        else
        {
            if (SaveLoadSystem.LoadGuestDetails() == null)
            {
                UIManager.instance.mainLoginPanel.SetActive(true);
            }              
            else
                GuestLogin();

            UIManager.instance.loadingPanel.SetActive(false);
        }
    

    }
   

    private void Start()
    {
        GetDailyPuzzle();      
    }

    public async void GetDailyPuzzle()
    {
        string result = await GetRequest.Get<DailyPuzzleAPIRoot>(URL.dailyPuzzle);
        JArray jsonArray = JArray.Parse(result);

        AddDailyPuzzle(jsonArray);
    } 

    void AddDailyPuzzle(JArray jArray)
    {
        for (int i = 0; i < jArray.Count; i++)
        {
            var recivedClass = JsonConvert.DeserializeObject<DailyPuzzleAPIRoot>(jArray[i].ToString());

            DailyPuzzleAPIRoot dailyPuzzleBaseClass = new DailyPuzzleAPIRoot();
            dailyPuzzleBaseClass = recivedClass;
      
            JArray dailyPuzzleJArray = JArray.Parse(dailyPuzzleBaseClass.dailypuzzle);

            for (int j = 0; j < dailyPuzzleJArray.Count; j++)
            {
                var d = JsonConvert.DeserializeObject<List<string>>(dailyPuzzleJArray[j].ToString());
                List<List<string>> list = new List<List<string>>();
                list.Add(d);

                DailPuzzleItems da = new DailPuzzleItems();

                foreach(List<string> s in list)
                {
                    foreach(string _s in s)
                    {
                        da.dailyPuzzleLetter.Add(_s);
                    }
                    
                }

                dailyPuzzleBaseClass.dailPuzzlesLettersList.Add(da);
            }

       
            var v = dailyPuzzleBaseClass.list_of_words.Split(',').ToList();
            
            foreach(string s in v)
            {
                string cleaned = s.Replace("\n", "").Replace("\r", "");
                dailyPuzzleBaseClass.words.Add(cleaned);
            }

          //  print("utc date : "+ dailyPuzzleBaseClass.date_of_assigning);
          //  print("system date : "+ System.DateTime.Today.Date);

            if(dailyPuzzleBaseClass.date_of_assigning == DateTime.Today.Date)
            {
                APIDataContainer.instance.dailyPuzzle.dailyPuzzleAPIRootList.Add(dailyPuzzleBaseClass);
                return;
            }
         
        }
    }

    public async void PostUserLoginDetails(string _userName,string _email,string _mode, string modeID)
    {      
        Globals.isLoggedIn = true;
        UserDetails ud = new UserDetails();
        ud.username = _userName;
        ud.email = _email;
        ud.mode = _mode;
        ud.mode_id = modeID;
        ud.avatar = Globals.avatar;
 
        var jsonString = JsonConvert.SerializeObject(ud);
        print(jsonString);
     
        var result = await PostRequest.Post(URL.postUserInfo, jsonString);//setting user details in api

        SaveLoadSystem.SaveUserDetails();
        GetUserprofile(SaveLoadSystem.LoadUserDetails(),true);
    }

    public async void GetUserprofile(SaveDetails s , bool cameFromMainLogin)
    {   

        if (!LoadingCircle.instance.loadingCircleObj.activeInHierarchy)
        UIManager.instance.loadingPanel.SetActive(true);


        try
        {
            var locurl = URL.getUserInfo;
            locurl += s.modeID;
         //   print(locurl);
            var result = await GetRequest.Get<GetUserInfo>(locurl);  //getting user details from api
         //   print(result);
            JArray array = JArray.Parse(result);
            var recivedClass = JsonConvert.DeserializeObject<GetUserInfo>(array[0].ToString());        

            Globals.UserName = recivedClass.username;
            Globals.email = recivedClass.email;        
            Globals.avatar = recivedClass.avatar;
            if (Globals.avatar == "0")
                Globals.avatar = "1";
            Globals.dbID = recivedClass.id.ToString();
            _ColyseusClient.instance.dbid = int.Parse(Globals.dbID);
            Globals.modeID = s.modeID;                     
            Globals.isLoggedIn = true;
            Globals.isGuestLoggedIn = false;
            Globals.loggedInMode = recivedClass.mode;

            UserProfileManager.loginButtonEvent.Invoke();
            UserProfileManager.eventUpdateDetails(Globals.UserName, GetProfileImage.Get(Globals.avatar));
            LoggedInStateObj.i.SetObj();
            _ColyseusClient.instance.JoinWorldRoom();              
            GetMyRank();

            if (cameFromMainLogin)
                ShowMessage.instance.Show("You have successfully logged in with " + Globals.loggedInMode);

            SaveLoadSystem.SaveUserDetails();
            LoadingCircle.instance.Loading(false);

        }

        catch(Exception e)
        {
            LoadingCircle.instance.Loading(false);
            SaveLoadSystem.UnSaveUserID();
            UserProfileManager.loginButtonEvent.Invoke();
            GuestLogin();


            if (cameFromMainLogin)
                ShowMessage.instance.Show(e.ToString());

            else
                ShowMessage.instance.Show(e.ToString());

           
        }
     
        if(GameObject.Find("SettingPanel") != null)
        {
            GameObject.Find("SettingPanel").SetActive(false);
        }

       // 
    }

    public async void UpdateAvatar(int id, string avatar)
    {
        AvatarUpdate a = new AvatarUpdate();
        a.id = id;
        a.avatar = avatar;
        var jsonString = JsonConvert.SerializeObject(a);
        print(jsonString);
        var result = await PostRequest.Post(URL.updateAvatar, jsonString);
        print(result);
    }

    public async void GetLeaderBoardDetails(Action<string> resultCallBack, string url)
    {
        string result = await GetRequest.Get<LeaderBoardRoot>(url);
        resultCallBack(result);
      
    }

    void GetMyRank()
    {
        try
        {
            GetLeaderBoardDetails(Response, URL.multiplayerLeaderBoard);
            void Response(string result)
            {
                print(result);
                JArray jsonArray = JArray.Parse(result);

                foreach (var v in jsonArray)
                {
                    var recivedClass = JsonConvert.DeserializeObject<LeaderBoardRoot>(v.ToString());
                    if (recivedClass.user_id.ToString() == Globals.dbID)
                    {
                        Globals.rank = recivedClass.rank;
                        print("My rank " + Globals.rank);
                        UserProfileManager.eventUpdateProgress(recivedClass);
                        return;
                    }
                }

            }
        }

        catch
        {
            LoadingCircle.instance.Loading(false);
        }
       

    }

    public async void PostMatchDetails(int winCount,int lossCount, int score,bool isDailPuzzle, bool getTodayRank)
    {
        string _URL = "";

        if (isDailPuzzle)
        {
            _URL = URL.dailyPuzzleLeaderBoard;
            DailyPuzzleScoreDetails d = new DailyPuzzleScoreDetails();
            d.user_id = int.Parse(Globals.dbID);
            d.wins = winCount;
            d.loss = lossCount;
            d.score = score;
            d.todayScore = score;
            d.avatar = Globals.avatar;
            d.puzzlePlayedAt = GetDateTime.Get();
            var jsonString = JsonConvert.SerializeObject(d);
            var result = await PostRequest.Post(_URL, jsonString);

            if(getTodayRank)
            SingleplayerGameManager.Instance.GetTodayRank();

            print("Posted score "+result);
        }
            
        else
        {
            _URL = URL.multiplayerLeaderBoard;
            MultiplayerScoreDetails m = new MultiplayerScoreDetails();
            m.user_id = int.Parse(Globals.dbID);
            m.wins = winCount;
            m.loss = lossCount;
            m.score = score;
            m.avatar = Globals.avatar;
            var jsonString = JsonConvert.SerializeObject(m);
            print(jsonString);
            var result = await PostRequest.Post(_URL, jsonString);
           // SingleplayerGameManager.Instance.GetTodayRank();
            print(result);
        }   
    }


    public void GuestLogin()
    {
       // SaveLoadSystem.UnSaveUserID();

        if (SaveLoadSystem.LoadGuestDetails() != null)
        {
            Globals.UserName = SaveLoadSystem.LoadGuestDetails()["guestName"];
        }
        else
        {
            SaveLoadSystem.SaveGuestDetails();
        }

        _ColyseusClient.instance.LeaveWorld();
        Globals.isLoggedIn = false;
        Globals.isGuestLoggedIn = true;
        UserProfileManager.eventUpdateDetails(Globals.UserName, GetProfileImage.Get(Globals.avatar));
        UIManager.instance.loadingPanel.SetActive(false);
        UIManager.instance.mainLoginPanel.SetActive(false);
        UserProfileManager.loginButtonEvent.Invoke();
        LoggedInStateObj.i.SetObj();

    }

    public void LogOut()
    {
        if (!Globals.isLoggedIn)
            return;

        if (Globals.loggedInMode == "facebook")
            GameObject.Find("FacebookManager").GetComponent<FacebookManager>().FacebookLogout();

        Globals.isLoggedIn = false;
        SaveLoadSystem.UnSaveUserID();
        _ColyseusClient.instance.LeaveWorld();
        
        GuestLogin();
    }

    public async void DeleteAccount()
    {
        LoadingCircle.instance.Loading(true);
        var delUrl = URL.deleteUserDetails;
        delUrl += Globals.dbID;
        string result = await DeleteRequest.Delete(delUrl);
        print(result);
        LoadingCircle.instance.Loading(false);
        GuestLogin();
        ShowMessage.instance.Show("Account deleted successfully!");
      
    }

    public async void GetInAppMessage(Action<string> resultCallBack)
    {
        string result = await GetRequest.Get<InAppMessageBase>(URL.inAppMsg);
        resultCallBack(result);
    }

   


}




