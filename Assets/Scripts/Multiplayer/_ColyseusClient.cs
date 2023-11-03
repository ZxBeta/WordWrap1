using Colyseus;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using static ColyseusRoomBase;
using static UserDetailsBase;


public class _ColyseusClient : MonoBehaviour
{
    public static _ColyseusClient instance;

    protected ColyseusClient client;
    protected ColyseusRoom<MyRoomState> worldRoom;
    protected ColyseusRoom<MyRoomState> lobbyRoom;
    protected ColyseusRoom<MyRoomState> gameRoom;
    string gameRoomID;
    string gameRoomSessionId;

    Dictionary<string, object> worldRoomDict;
    Dictionary<string, object> lobbyRoomdDict;
    Dictionary<string, object> gameRoomDict;
 
    [SerializeField] string serverAdress;
    [SerializeField] string worldRoomName;
    [SerializeField] string mainRoomName; 
  
    public int totalPlayersCount;  
    public int totalRounds;
    public int currentRound;
    public int myIndex;
    public int totalTime = 150;
    public int dbid;
    public bool hasTimer;
    [SerializeField] bool worldRoomJoined;
    public bool lobbyRoomJoined;
    public bool gameroomJoined;
    [HideInInspector]public LobbyRoomPlayerInfo   LobbyRoomInfo;
    public Dictionary<int, int> playersTotalScoreDict;
    public Dictionary<int, int> playersTotalWinCount;
    public Dictionary<string, int> letterFrequency;
    public List<string> previousRoundLetters = new List<string>();
    public bool roundReloaded;
    public string letterFreqText;

    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(gameObject);
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Start()
    {
        if(_ColyseusManager.Instance != null)
        Connect();
    }

    void Connect()
    {
        Debug.Log("Connecting to " + serverAdress);
        client = _ColyseusManager.Instance.CreateClient(serverAdress);   
    }

    public void InitPlayersDict()
    {
        playersTotalScoreDict = new Dictionary<int, int>();
        playersTotalWinCount = new Dictionary<int, int>();

        for (int i = 0; i < totalPlayersCount; i++)
        {
            playersTotalScoreDict.Add(i,0);
            playersTotalWinCount.Add(i,0);
        }

        //letterFrequency = new Dictionary<string, int>()
        //{
        //    {"A", 9}, {"B", 3}, {"C", 6}, {"D", 6}, {"E", 10},
        //    {"F", 3}, {"G", 3}, {"H", 4}, {"I", 8}, {"J", 1},
        //    {"K", 3}, {"L", 6}, {"M", 4}, {"N", 9}, {"O", 8},
        //    {"P", 4}, {"Q", 1}, {"R", 9}, {"S", 8}, {"T", 9},
        //    {"U", 6}, {"V", 3}, {"W", 3}, {"X", 1}, {"Y", 3},
        //    {"Z", 1}, {"*", 1}
        //};
        letterFrequency = new Dictionary<string, int>();
        CreateLetterDict createLetterDict = new CreateLetterDict();
        foreach(var v in createLetterDict.GetLettersFrequency(myIndex, totalPlayersCount))
        {
            letterFrequency.Add(v.Key,v.Value);
           
        }

        //ShowLetters();
    }

    public void ShowLetters()
    {
        letterFreqText = "";

        foreach (var v in letterFrequency)
        {
            letterFreqText += v.Key + " = " + v.Value;
            letterFreqText += " , ";
        }

        print(letterFreqText);
    }

    public async void JoinWorldRoom()
    {
        UpdateWorldRoomDict();

        try   
        {
          
            worldRoom = await client.JoinOrCreate<MyRoomState>(worldRoomName, worldRoomDict);
            Debug.Log("Joined world room successfully." + worldRoom.Id);
            ColyseusEvents.GetTotalPlayerInWorld(worldRoom);
            ColyseusEvents.ChallengeRecivedEvent(worldRoom);
            UIManager.instance.mainLoginPanel.SetActive(false);
            worldRoomJoined = true;
            worldRoomDict.Clear(); 
        }

        catch (Exception ex)
        {
            ShowMessage.instance.Show(ex.Message);
            Debug.Log("Error joining: " + ex.Message);
        }

        LoadingCircle.instance.Loading(false);
        UIManager.instance.loadingPanel.SetActive(false);
    }

    public async void CreateOrJoinRandomLobby()
    {
        UpdateUserLobbyRoomDic();
        StartLoader startLoader = new StartLoader(null);

        try
        {         
            lobbyRoom = await client.JoinOrCreate<MyRoomState>(mainRoomName, lobbyRoomdDict);
        
            SetLobby();
            ColyseusEvents.RandomLobbyTimer(lobbyRoom);
            UIManager.instance.OnWaitingLobby();
            Globals.isRandomLobby = true;
            Globals.isPrivateLobby = false;
            Debug.Log("Joined lobby room successfully." + lobbyRoom.Id);
        }

        catch(Exception ex)
        {
            ShowMessage.instance.Show(ex.Message);
            Debug.Log("Error joining: " + ex.Message);
        }

        startLoader.Destroy();
    }

    public async void CreatePrivateLobby(TMP_Text RoomIdtext)
    {
        UpdateUserLobbyRoomDic();
        StartLoader startLoader = new StartLoader(null);

        try
        {
            lobbyRoom = await client.Create<MyRoomState>("private_room", lobbyRoomdDict);
            Debug.Log("Created room successfully." + lobbyRoom.Id);
            RoomIdtext.text = lobbyRoom.Id;
            SetLobby();
            UIManager.instance.createRoomPanel.SetActive(true);
            Globals.isRandomLobby = false;
            Globals.isPrivateLobby = true;
            UIManager.instance.OnWaitingLobby();
            

        }
        catch (Exception ex)
        {
            ShowMessage.instance.Show(ex.Message);
            Debug.Log("Error creating: " + ex.Message);
        }

        startLoader.Destroy();
    }

   
    public async void JoinPrivateLobby(string privateRoomId, Action callback)
    {
        UpdateUserLobbyRoomDic();
        StartLoader startLoader = new StartLoader(null);

        try
        {
            lobbyRoom = await client.JoinById<MyRoomState>(privateRoomId, lobbyRoomdDict);
            Debug.Log("Joined room successfully." + lobbyRoom.Id);
            SetLobby();
            UIManager.instance.OnWaitingLobby();
            Globals.isRandomLobby = false;
            Globals.isPrivateLobby = true;
            if (callback != null)
                callback.Invoke();
        }

        catch(Exception e)
        {
            ShowMessage.instance.Show(e.Message);
            Debug.Log("Error creating: " + e.Message);
            Debug.Log("Error creating: " + e.Message);
        }
      
      
        startLoader.Destroy();
    }

    public async void JoinGameRoom(Action action, bool reconnect)
    {
        try
        {
            if(!reconnect)
                gameRoom = await client.JoinById<MyRoomState>(gameRoomID, gameRoomDict);

            else
            {
                print("session id " + gameRoomSessionId);
                gameRoom = await client.Reconnect<MyRoomState>(gameRoomID, gameRoomSessionId);
            }

            ColyseusEvents.LettersRecived(gameRoom);
            ColyseusEvents.RoomStateOnChange(gameRoom);
            gameRoom.OnStateChange += ColyseusEvents.OnStateChange;
            gameRoom.State.players.OnAdd += ColyseusEvents.OnPlayerAdd;
            gameRoom.State.players.OnRemove += ColyseusEvents.OnPlayerRemove;
            gameRoom.State.players.OnChange += ColyseusEvents.OnPlayerChange;
            ColyseusEvents.PlayerLeftGameRoom(gameRoom);
            ColyseusEvents.OpponentReconnected(gameRoom);
            ColyseusEvents.PlayerSubmitted(gameRoom);
            ColyseusEvents.RoundEnded(gameRoom);
            ColyseusEvents.GameEnded(gameRoom);
          
            gameRoomSessionId = gameRoom.SessionId;
            gameroomJoined = true;

            if (!reconnect && action != null)
                action();

        }

        catch (Exception ex)
        {
            ShowMessage.instance.Show(ex.Message);
            Debug.Log("Error joining: " + ex.Message);
        }



    }

    public async void ChallengePlayers(ChallengeRequestDetails challengeRequest)
    {
        await worldRoom.Send("SendInvite", challengeRequest);
        UIManager.instance.challengeMorePopup.SetActive(true);
    }

    void SetLobby()
    {
        GetGameRoomDetailsFromLobby("ROOM_CONNECT");
        lobbyRoomJoined = true;
        lobbyRoom.State.players.OnAdd += ColyseusEvents.OnPlayerAdd;
        lobbyRoom.State.players.OnRemove += ColyseusEvents.OnPlayerRemove;
       // lobbyRoom.OnStateChange += ColyseusEvents.OnStateChange;
        ColyseusEvents.GetTotalPlayerInLobby(lobbyRoom);
        ColyseusEvents.PlayerLeftLobbyRoom(lobbyRoom);
        WaitingLobby.setActivePlayerIcon.Invoke();
        lobbyRoomdDict.Clear();    
    
    }

    void UpdateWorldRoomDict()
    {
        worldRoomDict = new Dictionary<string, object>();
        worldRoomDict.Add("userName", Globals.UserName);
        worldRoomDict.Add("dbId", Globals.dbID);
        worldRoomDict.Add("avatar", Globals.avatar);
        worldRoomDict.Add("mode", Globals.loggedInMode);
        worldRoomDict.Add("maxClients", 100);
        worldRoomDict.Add("rank", Globals.rank);
    }

    void UpdateUserLobbyRoomDic()
    {
        print("sending total players "+totalPlayersCount);
        lobbyRoomdDict = new Dictionary<string, object>();
        gameRoomID = "";
        lobbyRoomdDict.Add("coin", 100);
        lobbyRoomdDict.Add("userName", Globals.UserName);
        lobbyRoomdDict.Add("dbId", Globals.dbID);
        lobbyRoomdDict.Add("avatar", Globals.avatar);
        lobbyRoomdDict.Add("numClientsToMatch", totalPlayersCount);
        lobbyRoomdDict.Add("hasTimer", hasTimer);
        lobbyRoomdDict.Add("totalTime", totalTime);
        lobbyRoomdDict.Add("noOfLevels", totalRounds);
    }

    void GetGameRoomDetailsFromLobby(string type)
    {
        lobbyRoom.OnMessage<GameRoomPlayerInfo>(type, (message) => 
        {
            Debug.Log("ROOM_CONNECT Recived ");

            gameRoomDict = new Dictionary<string, object>();
            gameRoomDict.Add("roomId", message.roomId);
            gameRoomDict.Add("userName", Globals.UserName);
            gameRoomDict.Add("dbId", Globals.dbID);
            gameRoomDict.Add("team", "a");
           // gameRoomDict.Add("coin", message.);
            gameRoomDict.Add("avatar", Globals.avatar);
            gameRoomDict.Add("type", message.type);
            gameRoomDict.Add("userIndex", message.userIndex);
            gameRoomID = message.roomId;
            myIndex = message.userIndex;
            totalRounds = message.noOfLevels;
            totalTime = message.totalTime;
            if(!Globals.isRandomLobby)
              totalPlayersCount = message.minplayersToMatch;
          //  minplayersToMatch = message.minplayersToMatch;
            print("has time "+message.hasTimer);
            print("Total min players "+message.minplayersToMatch);
            print("No of levels "+ message.noOfLevels);
            print("gameRoomID  " + gameRoomID);

            hasTimer = message.hasTimer;

            if(Globals.isPrivateLobby)
                WaitingLobby.updateWaitingLobby.Invoke();

        });

    
    }

    public async void SubmitTurn(PlayerSumbitData playerSumbitData)
    {
        playerSumbitData.userIndex = myIndex;
        playerSumbitData.dbID = int.Parse(Globals.dbID);
        await gameRoom.Send("playersubmitted", playerSumbitData);

    }
    public async void SubmitLetterUsed(PlayerSumbitData playerSumbitData)
    {
        await gameRoom.Send("playersubmitted", playerSumbitData);

    }

    public async void RoundEnded(PlayerRoundDetails p)
    {
        p.dbID = int.Parse(Globals.dbID);
        p.userIndex = myIndex;
        p.currentRound = currentRound;
        await gameRoom.Send("roundWinner", p);
    }

    public async void GameEnded(PlayerRoundDetails p)
    {
        p.dbID = int.Parse(Globals.dbID);
        p.userIndex = myIndex;
        p.currentRound = currentRound;
        p.totalWinCount = playersTotalWinCount[myIndex];
        await gameRoom.Send("gameWinner", p);
        
    }


    public void LeaveWorld()
    {
        if (worldRoomJoined)
        {
            worldRoom.Leave();        
            worldRoomJoined = false;
          
        }
          
    }


    public void LeaveLobby()
    {
        if (lobbyRoomJoined)
        {
            lobbyRoomJoined = false;
            lobbyRoom.Leave();
            lobbyRoom = null;          
        }
           
    }

    public void LeaveGameRoom()
    {
        if (gameroomJoined)
        {
            gameroomJoined = false;
            totalPlayersCount = 4;
            hasTimer = true;
            totalRounds = 3;
            gameRoom.Leave(true);
           
        }
      
    }

    void OnApplicationQuit()
    {
        if (worldRoomJoined)
            worldRoom.Leave();

        if (lobbyRoomJoined)
            lobbyRoom.Leave(true);

        if (gameroomJoined)
            gameRoom.Leave(true);
    }





}









