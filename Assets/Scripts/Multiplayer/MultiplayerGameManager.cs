using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LS;
using TMPro;
using UnityEngine.SceneManagement;
using static ColyseusRoomBase;
using System.Linq;
using UnityEngine.UI;

public class MultiplayerGameManager : Singleton<MultiplayerGameManager>
{
    public GameObject BoardPrefab;
    public GameObject[,] Board = new GameObject[7, 4];
    public GameObject BoardParent;

    public GameObject PlayerLetterTable;
    public GameObject PassButton, SubmitButton;
    public GameObject AlphabetPrefab;

    public TextMeshProUGUI WordTextBox;
    public TextMeshProUGUI PlayerScoreText;
    public TextMeshProUGUI TileText;
    public TextMeshProUGUI levelText;

    public GameObject LastPlaced;
    public GameObject EndPanel;

    public int PlayerScore;
    [HideInInspector] public int Multiplier;
    public int totalTiles;
    public int tileFilledCount;
    public int maxTilesInGrid;

    public GameObject opPlayerTempTable;//for storing submitted letter temporarily
    public bool isMyTurn;

    public List<GameObject> SelectedLetters = new List<GameObject>();
    public List<string> ListOfPlayedWords = new List<string>();
    public List<AddedLetter> addedLettersInGridBox = new List<AddedLetter>();
    public GameObject msgSpawner;
    public List<Player> gameRoomPlayers = new List<Player>();
    public Dictionary<int, int> playersCurrentRoundScoreDict;
    public GameObject opponentReconnecting;
    int currentWinnerIndex;
    public bool addedLetterFromRack;
    public List<string> letterdUsed = new List<string>();


    private void Awake()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            Destroy(gameObject);
        }

    }

    void Start()
    {
        _ColyseusClient.instance.totalPlayersCount = _ColyseusClient.instance.LobbyRoomInfo.gamePlayerList.Length;
        Globals.gameEnded = true;
        Globals.isMultiplayerMode = true;
        Globals.isSingleplayerMode = false;
        ResetBoard();
        ClearWordBox();
        SetCurrentRoundDict();


        foreach (var v in _ColyseusClient.instance.LobbyRoomInfo.gamePlayerList)
        {
            gameRoomPlayers.Add(v);
        }

        StartTurn();
        Invoke("LeaveOtherLoby", 2f);

    }
    void LeaveOtherLoby()
    {
        _ColyseusClient.instance.LeaveLobby();
        _ColyseusClient.instance.LeaveWorld();
    }

    public void OpponenntReconnecting(string name, bool flag)
    {
        opponentReconnecting.transform.GetChild(0).GetComponent<TMP_Text>().text = name + " Reconnecting";

        if (_ColyseusClient.instance.hasTimer)
        {
            TimeManager.Instance.timerIsRunning = !flag;
        }

        opponentReconnecting.SetActive(flag);
    }

    void SetCurrentRoundDict()
    {
        playersCurrentRoundScoreDict = new Dictionary<int, int>();

        for (int i = 0; i < _ColyseusClient.instance.totalPlayersCount; i++)
        {
            // print("adding");
            playersCurrentRoundScoreDict.Add(i, 0);
        }
    }

    public void StartTurn()
    {
        //CheckMyLettersUsed();

        if (_ColyseusClient.instance.currentRound > 1)
        {
            PlayerScore = _ColyseusClient.instance.playersTotalScoreDict[_ColyseusClient.instance.myIndex];
            for (int i = 0; i < _ColyseusClient.instance.totalPlayersCount; i++)
            {
                print("adding " + i + " " + _ColyseusClient.instance.playersTotalScoreDict[i]);
                MultiPlayerUI.ScoreUpdateaction(i, _ColyseusClient.instance.playersTotalScoreDict[i]);
            }

        }

        Multiplier = 1;


        totalTiles = (_ColyseusClient.instance.totalPlayersCount * 28) + 21;
        TileText.text = totalTiles.ToString();

        if (_ColyseusClient.instance.hasTimer)
            TimeManager.Instance.timeOutEvent.AddListener(TimeOut);
        else
            TimeManager.Instance.TimerObj.SetActive(false);

        levelText.text = "<color=yellow>" + _ColyseusClient.instance.currentRound + "</color >" + "/" + _ColyseusClient.instance.totalRounds;

        if (_ColyseusClient.instance.currentRound == 1)
        {
            CheckIsMyTurn(_ColyseusClient.instance.myIndex == 0);
        }



        if (_ColyseusClient.instance.currentRound > 1)
            CheckIsMyTurn(_ColyseusClient.instance.myIndex == Globals.currentTurnIndex);


        DistributeLetters();

        //if (_ColyseusClient.instance.myIndex == 0 && _ColyseusClient.instance.currentRound == 1)
        //    DistributeLetters();
        //if (_ColyseusClient.instance.currentRound > 1)
        //    DistributeLetters();

        MultiPlayerUI.i.UpdatePlayerIndicator(Globals.currentTurnIndex);

    }


    void StartTimer()
    {
        if (!_ColyseusClient.instance.hasTimer)
            return;

        TimeManager.Instance.timeRemaining = _ColyseusClient.instance.totalTime;
        TimeManager.Instance.ShowTimer(true);
        TimeManager.Instance.timerIsRunning = true;
    }


    public void CheckIsMyTurn(bool myturn)
    {
        if (!myturn)
        {
            SubmitButton.SetActive(false);
            PassButton.SetActive(false);
            isMyTurn = false;
            //  PlayerLetterTable.gameObject.SetActive(false);
            TimeManager.Instance.ShowTimer(false);
        }

        else
        {
            SubmitButton.SetActive(true);
            PassButton.SetActive(true);
            //   PlayerLetterTable.gameObject.SetActive(true);
            //new ShowMessage("Your turn", msgSpawner.transform);
            isMyTurn = true;

            StartTimer();
        }


    }

    public void ResetBoard()
    {
        for (int i = 0; i < Board.GetLength(0); i++)
        {
            for (int j = 0; j < Board.GetLength(1); j++)
            {
                GameObject current = Instantiate(BoardPrefab, BoardParent.transform);
                Board[i, j] = current;
                current.GetComponent<BoardItem>().myPos = new Vector2(i, j);
                current.GetComponent<BoardItem>().Initialize(i, j);
            }
        }
    }

    public void ClearWordBox()
    {
        WordTextBox.text = "";
        addedLetterFromRack = false;
    }

    public void InsertLetter(string st)
    {
        WordTextBox.text += st;
    }

    public void DistributeLetterAfteFirst()
    {
        if (PlayerLetterTable.transform.childCount > 0)
            return;

        DistributeLetters();
    }

    public void DistributeLetters()
    {
        if (PlayerLetterTable.transform.childCount < 7)
        {
            if (_ColyseusClient.instance.previousRoundLetters.Count > 0)
            {
                for (int i = 0; i < _ColyseusClient.instance.previousRoundLetters.Count; i++)
                {
                    GameObject current = Instantiate(AlphabetPrefab, PlayerLetterTable.transform);
                    string currentLetter = _ColyseusClient.instance.previousRoundLetters[i];
                    int mux = GetMultiplierValue(currentLetter);
                    current.GetComponent<LetterScript>().initializeLetter(currentLetter, mux, true, false);
                    // print("adding " + currentLetter);
                }

                _ColyseusClient.instance.previousRoundLetters.Clear();
                _ColyseusClient.instance.roundReloaded = false;

            }

            else
            {
                while (PlayerLetterTable.transform.childCount < 7)
                {
                    GameObject current = Instantiate(AlphabetPrefab, PlayerLetterTable.transform);
                    string currentLetter = GenerateRandomLetters();
                    int mux = GetMultiplierValue(currentLetter);
                    //  print("adding " + currentLetter);
                   // letterdUsed.Add(currentLetter);
                    current.GetComponent<LetterScript>().initializeLetter(currentLetter, mux, true, false);
                }


            }
        }

      
    }

    public void AddToSelectedLetters(GameObject current)
    {
        SelectedLetters.Add(current);
    }

    //set a particular column as selectable
    public void SetSelectable(int columnIndex)
    {
        int nextColumnIndex;
        nextColumnIndex = columnIndex == 3 ? 0 : columnIndex + 1;

        for (int i = 0; i < Board.GetLength(0); i++)
        {
            for (int j = 0; j < Board.GetLength(1); j++)
            {
                if (nextColumnIndex == j)
                {
                    if (Board[i, j].GetComponent<BoardItem>().hasLetter)
                    {
                        Board[i, j].GetComponentInChildren<LetterScript>().canSelect = true;
                    }
                }
                else
                {
                    if (Board[i, j].GetComponent<BoardItem>().hasLetter)
                    {
                        Board[i, j].GetComponentInChildren<LetterScript>().canSelect = false;
                    }
                }
            }
        }
    }

    public void PushLetterInColumn(int columnValue, LetterScript letter)
    {
        for (int i = 0; i < Board.GetLength(0); i++)
        {
            //  Debug.Log(columnValue + ":" + i);
            if (!Board[i, columnValue].GetComponent<BoardItem>().hasLetter)
            {
                Board[i, columnValue].GetComponent<BoardItem>().SetLetter(letter);

                AddedLetter l = new AddedLetter();
                l.letter = letter.Textbox.text;
                l.posInGrid = new Vector2(i, columnValue);
                addedLettersInGridBox.Add(l);
                letter.gameObject.transform.GetChild(3).gameObject.SetActive(true);

                break;
            }
        }
    }

    public void OnClickClearButton()
    {
        ClearWordBox();
        foreach (GameObject go in SelectedLetters)
        {
            go.GetComponent<LetterScript>().isSelected = false;
        }
        SelectedLetters.Clear();
        LastPlaced = null;
        for (int i = 0; i < Board.GetLength(0); i++)
        {
            for (int j = 0; j < Board.GetLength(1); j++)
            {
                if (Board[i, j].transform.childCount > 0)
                {
                    Board[i, j].GetComponentInChildren<LetterScript>().canSelect = true;
                }
            }
        }

        UndoAddedLettersInGrid();
    }

    public void OnClickPassButton()
    {
        SubmitUserTurn(false);
        OnClickClearButton();
    }

    void TimeOut()
    {
        // SubmitUserTurn(false);
        OnClickClearButton();
        ShowMessage.instance.Show("Time Out!");
    }

    public void OnClickSubmitButton()
    {
        if (WordTextBox.text == "")
            return;


        if (WordTextBox.text.Length < 3)
        {
            ShowMessage.instance.Show("Please make at least a 3 letter word!");
            AudioHandler.instance.NotValidMove();
            return;
        }

        if (!addedLetterFromRack && tileFilledCount < maxTilesInGrid)
        {
            ShowMessage.instance.Show("Please use a letter from you rack!");
            AudioHandler.instance.NotValidMove();
            return;
        }


        if (WordTextBox.text.Contains("*"))
        {
            print("wildletter");
            WildLetterHandler.instance.FillWildLetterPanel(_ColyseusClient.instance.letterFrequency);
        }


        else
            CheckWord();


    }

    //sound icon
    // practice puzzle button up
    //reconnecting thing while switching app
    //ipad thing
    // ipad 9th gen
    //highest score winner



    public void CheckWord()
    {

        if (ListOfPlayedWords.Contains(WordTextBox.text))
        {
            ShowMessage.instance.Show("Already a added word!");

            AudioHandler.instance.NotValidMove();
            return;
        }

        print(addedLettersInGridBox.Count);



        if (!ListOfPlayedWords.Contains(WordTextBox.text))
        {
            bool correctWord = WordGameDict.Instance.CheckWord(WordTextBox.text);

            if (correctWord)
            {
                SubmitUserTurn(true);
                DistributeLetters();

            }
            else
            {
                AudioHandler.instance.NotValidMove();
                ShowMessage.instance.Show("Not a valid word!");
            }
        }
    }

    void SubmitUserTurn(bool addedWord)
    {

        TimeManager.Instance.ShowTimer(false);
        TimeManager.Instance.timerIsRunning = false;

        PlayerSumbitData plyrTurnData = new PlayerSumbitData();
        plyrTurnData.addedWord = addedWord;
        //  plyrTurnData.score = PlayerScore;

        if (WordTextBox.text != "")
            plyrTurnData.wordMade = WordTextBox.text;
        else if (WordTextBox.text == "" || !addedWord)
            plyrTurnData.wordMade = "";

        if (addedLettersInGridBox.Count > 0 && addedWord)
        {
            plyrTurnData.addedLettersList = new List<AddedLetter>();
            plyrTurnData.addedLettersList = FilterOnlyTextBoxLetter();
         

        }

        // print("letter used count " + letterdUsed.Count);
        //  plyrTurnData.letterFrequency = letterdUsed;

        CalculateScore(plyrTurnData);

        Globals.msgTime = 7;
        string myname = gameRoomPlayers[_ColyseusClient.instance.myIndex].userName;
  

        OnClickClearButton();
        AudioHandler.instance.SubmitWord();
        UndoAddedLettersInGrid();
        CheckIsMyTurn(false);
;
    }

    List<AddedLetter> FilterOnlyTextBoxLetter()
    {
        List<AddedLetter> filterAddedLetters = new List<AddedLetter>();
        List<AddedLetter> tempAddedLetters = addedLettersInGridBox;

        for (int j = 0; j < SelectedLetters.Count; j++)
        {
            var pos = SelectedLetters[j].transform.parent.GetComponent<BoardItem>().myPos;

            for (int k = 0; k < tempAddedLetters.Count; k++)
            {
                if (pos == tempAddedLetters[k].posInGrid)
                {
                    filterAddedLetters.Add(tempAddedLetters[k]);
                    addedLettersInGridBox.RemoveAt(k);
                    break;
                }
            }
        }

        return filterAddedLetters;
    }

    void UndoAddedLettersInGrid()
    {
        if (addedLettersInGridBox.Count > 0)
        {
            foreach (AddedLetter l in addedLettersInGridBox)
            {
                SpawnLetterInPlayerTable(l.letter);
                int x = (int)l.posInGrid.x;
                int y = (int)l.posInGrid.y;
                Destroy(Board[x, y].transform.GetChild(0).gameObject);
                Board[x, y].transform.GetComponent<BoardItem>().hasLetter = false;
            }

            addedLettersInGridBox.Clear();
        }
    }

    void SpawnLetterInPlayerTable(string letter)
    {
        GameObject current = Instantiate(AlphabetPrefab, PlayerLetterTable.transform);
        string currentLetter = letter;
        int mux = GetMultiplierValue(currentLetter);
        current.GetComponent<LetterScript>().initializeLetter(currentLetter, mux, true, false);//letter distributor function
    }

    public string GenerateRandomLetters()
    {
        string retVal = "";
        int count = 0;
        foreach (var element in _ColyseusClient.instance.letterFrequency)
        {
            count += element.Value;
        }
        if (count > 0)
        {
            int random = Random.Range(1, count + 1);
            foreach (var element in _ColyseusClient.instance.letterFrequency)
            {
                random -= element.Value;
                if (random <= 0 && element.Value > 0)
                {
                    retVal = element.Key;
                    _ColyseusClient.instance.letterFrequency[retVal] -= 1;         
                    break;
                }
            }

           // print(retVal + "  " + _ColyseusClient.instance.letterFrequency[retVal]);
            //CheckMyLettersUsed();
          //  _ColyseusClient.instance.ShowLetters();
        }
        return retVal;
    }

    public int GetMultiplierValue(string st)
    {
        List<string> TwoPoint = new List<string>() { "B", "F", "G", "K", "P", "V", "W", "Y" };
        List<string> ThreePoint = new List<string>() { "J", "Q", "X", "Z" };

        if (TwoPoint.Contains(st))
            return 2;
        if (ThreePoint.Contains(st))
            return 3;
        return 1;
    }

    public void CalculateScore(PlayerSumbitData plyrTurnData)
    {
        Multiplier = 1;

        char[] l = WordTextBox.text.ToCharArray();

        if (l.Length > 6 && l.Length < 10)
            PlayerScore += 10;
        if (l.Length >= 10)
            PlayerScore += 25;

        // PlayerScore += l.Length;
        //print("lenth  "+l.Length);
        //print("PlayerScore -- 1   " + PlayerScore);

        foreach (GameObject k in SelectedLetters)
        {
            Multiplier *= k.GetComponent<LetterScript>().multiplier;
        }

        PlayerScore += (Multiplier * l.Length);
        // print("PlayerScore -- 2   " + PlayerScore);

        if (plyrTurnData.addedWord)
            ShowMessage.instance.Show("You" + " made " + "'" + WordTextBox.text +
                "'" + " and scored " + "'" + (Multiplier * l.Length) + "'");
        else
            ShowMessage.instance.Show("You pass the turn");


        plyrTurnData.score = PlayerScore;


        _ColyseusClient.instance.SubmitTurn(plyrTurnData);
        letterdUsed.Clear();

    }

    public void ClearBoard()
    {
        for (int i = 0; i < Board.GetLength(0); i++)
        {
            for (int j = 0; j < Board.GetLength(1); j++)
            {
                if (Board[i, j].GetComponent<BoardItem>().hasLetter)
                {
                    //destroy childcomponent

                }
            }
        }
    }


    public void UpdateLettersFromSubmittion(string Letter, Vector2 posInGrid)
    {
        int i = (int)posInGrid.x;
        int j = (int)posInGrid.y;

        if (Board[i, j].GetComponent<BoardItem>().hasLetter)
        {
            Board[i, j].transform.GetChild(0).GetComponent<LetterScript>().HighLight();
            return;
        }

        GameObject current = Instantiate(AlphabetPrefab, opPlayerTempTable.transform);

        string currentLetter = Letter;
        int mux = GetMultiplierValue(currentLetter);
        current.GetComponent<LetterScript>().initializeLetter(currentLetter, mux, true, false);
        Board[i, j].GetComponent<BoardItem>().SetLetter(current.GetComponent<LetterScript>());
        current.GetComponent<LetterScript>().HighLight();
    }


    public void UpdateAddedWordFromSubmit(string word, List<AddedLetter> addedLettersList)
    {
        if (word != "")
        {
            ListOfPlayedWords.Add(word);

            totalTiles -= tileFilledCount;
            TileText.text = totalTiles.ToString();

            if (addedLettersList != null)
            {
                tileFilledCount += addedLettersList.Count;

                foreach (AddedLetter ad in addedLettersList)
                {
                    UpdateLettersFromSubmittion(ad.letter, ad.posInGrid);
                }
            }

        }
    }


    public void CheckNextState(int userIndex)
    {
        if (tileFilledCount >= maxTilesInGrid)
        {
            EndRound();
            FindWinner();
            NextRound();
        }


    }

    public void FindWinner()
    {
        currentWinnerIndex = playersCurrentRoundScoreDict.OrderByDescending(x => x.Value).First().Key;
        var highestScore = playersCurrentRoundScoreDict[currentWinnerIndex];
        // print("Won by " + currentWinnerIndex);
        _ColyseusClient.instance.playersTotalWinCount[currentWinnerIndex] += 1;
    }

    public void NextRound()
    {
        TimeManager.Instance.ShowTimer(false);
        TimeManager.Instance.timerIsRunning = false;

        if (_ColyseusClient.instance.currentRound < _ColyseusClient.instance.totalRounds)
        {
            string winnerName = _ColyseusClient.instance.LobbyRoomInfo.gamePlayerList[currentWinnerIndex].userName;
            // new ShowMessage(winnerName + " is on the lead " , null);
            _ColyseusClient.instance.currentRound++;
            _ColyseusClient.instance.roundReloaded = true;

            _ColyseusClient.instance.previousRoundLetters.Clear();

            for (int i = 0; i < PlayerLetterTable.transform.childCount; i++)
            {
                _ColyseusClient.instance.previousRoundLetters.Add(PlayerLetterTable.transform.GetChild(i).GetComponent<LetterScript>().Textbox.text);
            }

            Invoke("ReloadGame", 1.2f);

        }

        else
        {
            GameEnded();
        }

    }

    public void GameEnded()
    {
        Globals.gameEnded = true;
        AudioHandler.instance.EndGame();
        _ColyseusClient.instance.LeaveGameRoom();
        TimeManager.Instance.timerIsRunning = false;
        MultiPlayerUI.GameEndedAction.Invoke();
    }


    public void HomeButton()
    {
        APIManager.instance.PostMatchDetails(0, 1,
    _ColyseusClient.instance.playersTotalScoreDict[_ColyseusClient.instance.myIndex], false,true);
        _ColyseusClient.instance.LeaveGameRoom();
        SceneManager.LoadSceneAsync("MenuScene");
        Destroy(gameObject);
    }

    public void Endgame()
    {
        PlayerRoundDetails p = new PlayerRoundDetails();
        p.score = PlayerScore;
        _ColyseusClient.instance.GameEnded(p);

    }

    public void EndRound()
    {
        PlayerRoundDetails p = new PlayerRoundDetails();
        p.score = PlayerScore;
        _ColyseusClient.instance.RoundEnded(p);
    }


    void ReloadGame()
    {
        ShowMessage.instance.Show("Loading Next Round ");
        Invoke("LoadGameScene", 1.2f);
    }

    void LoadGameScene()
    {
        SceneManager.LoadSceneAsync("GameSceneMultiplayer");
        Start();
    }

    public void PostMyMatchDetails(int winnerIndex)
    {
        if (winnerIndex == _ColyseusClient.instance.myIndex)
            APIManager.instance.PostMatchDetails(1, 0, _ColyseusClient.instance.playersTotalScoreDict[_ColyseusClient.instance.myIndex], false,true);
        else
            APIManager.instance.PostMatchDetails(0, 1, _ColyseusClient.instance.playersTotalScoreDict[_ColyseusClient.instance.myIndex], false,true);
    }

  
    new void OnApplicationQuit()
    {
        APIManager.instance.PostMatchDetails(0, 1, _ColyseusClient.instance.playersTotalScoreDict[_ColyseusClient.instance.myIndex], false,false);
    }
}
