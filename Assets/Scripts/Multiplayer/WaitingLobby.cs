using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static ColyseusRoomBase;

public class WaitingLobby : MonoBehaviour
{
    public Image[] playersAvatar;
    public TMP_Text[] userNameText;
    public TMP_Text countDownText;
    public GameObject vsImg;
    public List<float> addedDBID;

    LobbyRoomPlayerInfo _lobbyRoomInfo;
    [SerializeField] GameObject lobbyObj;

    public static EventsBus updatePlayersInLobby = new EventsBus();
    public static EventsBus updateWaitingLobby = new EventsBus();
    public static TEventBus<PlayerLeftLobby> playerLeft = new TEventBus<PlayerLeftLobby>();
    public static EventsBus startCountDown = new EventsBus();
    public static EventsBus setActivePlayerIcon = new EventsBus();
    public static EventsBus CloseMe = new EventsBus();

    public static List<Player> lobbyPlayers = new List<Player>();
    int length;
    int lengthOfPlayers;
    bool countDownStarted;
    public GameObject[] searchingObj;
    public Sprite waitingIcon;
    Color defaultColor;

    private void Awake()
    {
       
        updatePlayersInLobby.AddListener(UpdatePlayers);
        updateWaitingLobby.AddListener(UpdateWaitingLobby);
        playerLeft.AddListener(PlayerLeft);
        startCountDown.AddListener(StartCountDown);
        setActivePlayerIcon.AddListener(SetActivePlayersIcon);
        CloseMe.AddListener(CloseWaitingLobby);
        countDownText.gameObject.SetActive(false);
        vsImg.SetActive(true);
        defaultColor = playersAvatar[0].color;
      //  print("awake done");


    }

 
    void UpdatePlayers() 
    {
        if (!_ColyseusClient.instance.lobbyRoomJoined)
        {
            return;
        }
         
        if (length != _ColyseusClient.instance.LobbyRoomInfo.gamePlayerList.Length)
        {      
            length = _ColyseusClient.instance.LobbyRoomInfo.gamePlayerList.Length;
            lobbyPlayers.Clear();
            _lobbyRoomInfo = _ColyseusClient.instance.LobbyRoomInfo;
          
            foreach (var p in _lobbyRoomInfo.gamePlayerList)
            {
                lobbyPlayers.Add(p);
            }

            UpdateWaitingLobby();
        }

       // SetActivePlayersIcon();

    }

   

    void UpdateWaitingLobby()
    {
        

        for (int index = 0; index < lobbyPlayers.Count; index++)
        {
            print("added "+ lobbyPlayers[index].userName);

            if (addedDBID.Contains(lobbyPlayers[index].dbId))
                continue;
                

            userNameText[index].text = lobbyPlayers[index].userName;
            playersAvatar[index].sprite = GetProfileImage.Get(lobbyPlayers[index].avatar);
            playersAvatar[index].color = new Color(1, 1, 1);
            searchingObj[index].SetActive(false);
            addedDBID.Add(lobbyPlayers[index].dbId);

        }

        if (lobbyPlayers.Count >= _ColyseusClient.instance.totalPlayersCount
            && Globals.isPrivateLobby)
        {
          //  print("Starting " + _ColyseusClient.instance.totalPlayersCount);
            StartCountDown();
        }

    }

    void PlayerLeft(PlayerLeftLobby playerLeftLobby)
    {
        lobbyPlayers.Remove(lobbyPlayers.SingleOrDefault(x => x.dbId == playerLeftLobby.dbId));
        addedDBID.Remove(addedDBID.SingleOrDefault(x => x == playerLeftLobby.dbId));
        UpdateWaitingLobbyForPlayerLeft(playerLeftLobby.index);


        ShowMessage.instance.Show(playerLeftLobby.userName + " left lobby");
            
        if(gameObject.GetComponent<CountDownTimer>() && lobbyPlayers.Count < 2)
        {
            Destroy(GetComponent<CountDownTimer>());
            countDownText.gameObject.SetActive(false);
            vsImg.SetActive(true);
            countDownStarted = false;
            CloseWaitingLobby();
        }
     
    }

    void UpdateWaitingLobbyForPlayerLeft(int i)
    {
        userNameText[i].text = "";
        playersAvatar[i].sprite = waitingIcon;
        playersAvatar[i].color = defaultColor;
        searchingObj[i].SetActive(true);
    }

    //if player left on my countdown add a handling

    void StartCountDown()
    {
        if (countDownStarted)
            return;

        if (lobbyPlayers.Count < 2)//if lobby server waiting time up
        {
            ShowMessage.instance.Show("Unable to find any player try again later");
            _ColyseusClient.instance.LeaveLobby();
            CloseWaitingLobby();
            return;
        }
         

        for (int i = 0; i < 4; i++)
        {
            searchingObj[i].SetActive(false);
        }

       _ColyseusClient.instance.totalPlayersCount = lobbyPlayers.Count;    

         countDownStarted = true;
        CountDownTimer ct = gameObject.AddComponent<CountDownTimer>();
        vsImg.SetActive(false);
        countDownText.gameObject.SetActive(true);
        ct.timeRemaining = 10;
        ct.displayTimeText = countDownText;
        ct.timerIsRunning = true;        
        ct.timeOutEvent.AddListener(CallBackTimeOut);
        //add callback msg
    }

    void CallBackTimeOut()
    {
        _ColyseusClient.instance.InitPlayersDict();
        _ColyseusClient.instance.JoinGameRoom(MultiplayerScene,false);
        countDownStarted = false;

        void MultiplayerScene()
        {
            SceneLoader.LoadScene(SceneLoader.Scene.GameSceneMultiplayer);
        }
    }

    public void SetActivePlayersIcon()
    {
        lengthOfPlayers = _ColyseusClient.instance.totalPlayersCount;

        for (int i = 0; i < 4; i++)
        {
            searchingObj[i].SetActive(false);
        }

        for (int i = 0; i < _ColyseusClient.instance.totalPlayersCount; i++)
        {
            searchingObj[i].SetActive(true);
        }

        if (_ColyseusClient.instance.playersTotalScoreDict != null)
        {
            _ColyseusClient.instance.playersTotalScoreDict.Clear();
            _ColyseusClient.instance.playersTotalWinCount.Clear();
        }
     
       

    }

    public void ClearMe()
    {
        _ColyseusClient.instance.LeaveLobby();

        if (gameObject.GetComponent<CountDownTimer>() != null)
        gameObject.GetComponent<CountDownTimer>().timerIsRunning = false;

        for (int i = 0; i < length; i++)
        {
            userNameText[i].text = "";
            playersAvatar[i].sprite = waitingIcon;
            playersAvatar[i].color = defaultColor;
            searchingObj[i].SetActive(true);
       
      
        }
        vsImg.SetActive(true);
        countDownText.gameObject.SetActive(false);
        countDownStarted = false;
        length = 0;
        lengthOfPlayers = 0;
        addedDBID.Clear();
 

    }

    void CloseWaitingLobby()
    {     
        Invoke("InvokeClose", 1.8f);
       
    }

    void InvokeClose()
    {
      
        ClearMe();
        lobbyObj.SetActive(false);
    }
}
