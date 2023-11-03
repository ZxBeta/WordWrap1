using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using LS;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Newtonsoft.Json.Linq;
using System.Linq;
using Newtonsoft.Json;

public class SingleplayerGameManager : Singleton<SingleplayerGameManager>
{
    public GameObject BoardPrefab;
    public GameObject[,] Board;
    public GameObject BoardParent;

    //public GameObject PlayerLetterTable;
    public GameObject LetterPrefab;

    public TextMeshProUGUI WordTextBox;
    public TextMeshProUGUI PlayerScoreText;
    public TextMeshProUGUI TileText;
    public TextMeshProUGUI EndPanelScoreText;
    public TextMeshProUGUI TargetText;
    public TextMeshProUGUI bonusTxt;

    public GameObject WildLetterPanel;
    public GameObject EndPanel;

    List<GameObject> SelectedLetters;
    List<string> ListOfPlayedWords;

    public GameObject LastPlaced;

    public Dictionary<string, int> letterFrequency;

    public int PlayerScore;
    public int Multiplier;
    public int TileCount;
    public int targetScore;
    public int addedLettersInGridCount;

    public GameObject EndGameButton, SubmitButton;

    public Image profileImage;
    public Image endProfile;
    public GameObject targetObj;
    public TMP_Text endPanelTextStatus;
    public GameObject playedWordHolder;
    public GameObject playedWordPrefab;
    int currentWordLength;
    int bonus;
    public TMP_Text endProgressText;


    private void Awake()
    {
        if (SceneManager.GetActiveScene().buildIndex  == 0)
        {
            Destroy(gameObject);
        }

    }

    void Start()
    {
        Board = new GameObject[7, 4];// row:0-1-2-3  col: 0-1-2-3-4-5-6
        SelectedLetters = new List<GameObject>();
        ListOfPlayedWords = new List<string>();
        LastPlaced = null;

        letterFrequency = new Dictionary<string, int>()
        {
            {"A", 9}, {"B", 2}, {"C", 5}, {"D", 4}, {"E", 10},
            {"F", 2}, {"G", 2}, {"H", 3}, {"I", 8}, {"J", 1},
            {"K", 2}, {"L", 5}, {"M", 3}, {"N", 8}, {"O", 8},
            {"P", 3}, {"Q", 1}, {"R", 8}, {"S", 8}, {"T", 8},
            {"U", 5}, {"V", 2}, {"W", 2}, {"X", 1}, {"Y", 2},
            {"Z", 1}, {"*", 1}
        };

        PlayerScore = 0;
        Multiplier = 1;
        TileCount = 7 * 4;

        Globals.isSingleplayerMode = true;
        Globals.isMultiplayerMode = false;

        ResetBoard();
        ClearWordBox();
        StartTurn();
        UpdateRemainingTileCounter();
        SetProfileImage();
        _ColyseusClient.instance.LeaveWorld();

        if (Globals.isPractisePuzzle)
            targetObj.SetActive(false);


    }

    public void StartTurn()
    {
        Multiplier = 1;
        DistributeLetters();
        TimeManager.Instance.StopWatchTimer();//<- give time to user
    }

    void SetProfileImage()
    {
        int index = int.Parse(Globals.avatar);
        profileImage.sprite = APIDataContainer.instance.AvatarImagelist[index];
        endProfile.sprite = profileImage.sprite;
    }

  
    public void ResetBoard()
    {
        for (int i = 0; i < Board.GetLength(0); i++)
        {
            for (int j = 0; j < Board.GetLength(1); j++)
            {
                GameObject current = Instantiate(BoardPrefab, BoardParent.transform);
                Board[i, j] = current;
                current.GetComponent<BoardItem>().Initialize(i, j);
                current.GetComponent<BoardItem>().myPos = new Vector2(i, j);
            }
        }
    }


    public void ClearWordBox()
    {
        WordTextBox.text = "";
    }

    public void InsertLetter(string st)
    {
        WordTextBox.text += st;
    }

    //distribute letters
    public void DistributeLetters()
    {

        DailyPuzzle dailyPuzzle = APIDataContainer.instance.dailyPuzzle;
        DailyPuzzleAPIRoot dailyPuzzleAPIRoot = null;

        if (!Globals.isPractisePuzzle)
        {
            dailyPuzzleAPIRoot = dailyPuzzle.dailyPuzzleAPIRootList[0];
            targetScore = dailyPuzzleAPIRoot.target_score;
            TargetText.text = targetScore.ToString();
        }

        for (int i = 0; i < Board.GetLength(0); i++)
        {
            for (int j = 0; j < Board.GetLength(1); j++)
            {
                GameObject current = Instantiate(LetterPrefab, Board[i, j].transform);
                string currentLetter = "";

                if (Globals.isPractisePuzzle)
                    currentLetter = GenerateRandomLetters();
                else
                    currentLetter = dailyPuzzleAPIRoot.dailPuzzlesLettersList[i].dailyPuzzleLetter[j];

                int mux = GetMultiplierValue(currentLetter);
                current.GetComponent<LetterScript>().initializeLetter(currentLetter, mux, false, false);//letter distributor function
                Board[i, j].GetComponent<BoardItem>().SetLetter(current.GetComponent<LetterScript>());
            }
        }

    }

    //add to list of selected letters
    public void AddToSelectedLetters(GameObject current)
    {
        SelectedLetters.Add(current);
        SetButton();
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
                    if (Board[i, j].GetComponent<BoardItem>().hasLetter && Board[i, j].GetComponentInChildren<LetterScript>())
                    {
                        Board[i, j].GetComponentInChildren<LetterScript>().canSelect = true;
                    }
                }
                else
                {
                    if (Board[i, j].GetComponent<BoardItem>().hasLetter && Board[i, j].GetComponentInChildren<LetterScript>())
                    {
                        Board[i, j].GetComponentInChildren<LetterScript>().canSelect = false;
                    }
                }
            }
        }
    }

    public void OnClickClearButton()
    {
        currentWordLength = SelectedLetters.Count;
        ClearWordBox();
        foreach (GameObject go in SelectedLetters)
        {
            go.GetComponent<LetterScript>().isSelected = false;
            go.GetComponent<LetterScript>().UnPressed();
        }

        SelectedLetters.Clear();
        LastPlaced = null;

        MakeAllSelectable();

       StartCoroutine(FillTheGaps());

        SetButton();
    }

    void MakeAllSelectable()
    {
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
    }

    IEnumerator FillTheGaps()
    {
        int loopCount = 1;
        if(currentWordLength > 4)
        {
            loopCount += (currentWordLength - 1) / 4; 
        }

        for (int m = 0; m < loopCount; m++)
        {

            for (int row = 0; row < Board.GetLength(1); row++)//filling the gaps;
            {
                for (int column = 0; column < Board.GetLength(0); column++)
                {
                    if (!Board[column, row].transform.GetComponent<BoardItem>().hasLetter || 
                        Board[column, row].transform.GetComponent<BoardItem>().transform.
                        childCount <= 0)
                    {
                        for (int _column = column; _column < 6; _column++)
                        {
                            if (Board[_column + 1, row].transform.childCount > 0)
                            {
                                Board[_column, row].GetComponent<BoardItem>().SetLetter
                                (Board[_column + 1, row].GetComponentInChildren<LetterScript>());
                            }

                        }
                    }

                }
            }

            yield return null;
        }
        
    }


    public void OnClickSubmitButton()
    {
        if (WordTextBox.text != "" && WordTextBox.text.Length >= 3)
        {
            if (WordTextBox.text.Contains("*"))
               WildLetterHandler.instance.FillWildLetterPanel(letterFrequency);
            else
                CheckCorrectWord();
        }

        else
        {
            AudioHandler.instance.NotValidMove();
            ShowMessage.instance.Show("Please make at least a 3 letter word!");
        }
    }



    public void CheckCorrectWord()
    {
        if (WordGameDict.Instance.CheckWord(WordTextBox.text))
        {
            if (!ListOfPlayedWords.Contains(WordTextBox.text))
            {
                ListOfPlayedWords.Add(WordTextBox.text);

                char[] l = WordTextBox.text.ToCharArray();
                addedLettersInGridCount += l.Length;



                if (l.Length > 6 && l.Length < 10)
                    bonus = 10;
                else if (l.Length >= 10)
                    bonus = 25;
                else
                    bonus = 0;

                CalculateScore();
              
                AudioHandler.instance.SubmitWord();
            }

            else
            {
                AudioHandler.instance.NotValidMove();
                ShowMessage.instance.Show("Already a added word!");
            }
        }
        else
        {
            AudioHandler.instance.NotValidMove();
            ShowMessage.instance.Show("Not a valid word!");
        }
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

    public string GenerateRandomLetters()
    {
        string retVal = "";
        int count = 0;
        foreach (var element in letterFrequency)
        {
            count += element.Value;
        }
        if (count > 0)
        {
            int random = Random.Range(1, count + 1);
            foreach (var element in letterFrequency)
            {
                random -= element.Value;
                if (random <= 0)
                {
                    retVal = element.Key;
                    break;
                }
            }
         
            letterFrequency[retVal] -= 1;
           // Debug.Log(count + ":" + random + ":" + retVal);
        }
        return retVal;
    }

    public void CalculateScore()
    {
        int currentScore =0;
        Multiplier = 1;
        foreach (GameObject k in SelectedLetters)
        {
            Multiplier *= k.GetComponent<LetterScript>().multiplier;
        }

        currentScore += bonus;
        currentScore += Multiplier * WordTextBox.text.Length;

        GameObject plydWrd = Instantiate(playedWordPrefab, playedWordHolder.transform);
        plydWrd.GetComponent<TMP_Text>().text = WordTextBox.text + " : " + currentScore;

        PlayerScore += currentScore;
        PlayerScoreText.text = PlayerScore.ToString();

        //add remaining letter
        Globals.msgTime = 4;
        ShowMessage.instance.Show("You made : " + "'" + WordTextBox.text + "'" + " , and scored : " + "'" + currentScore + "'");

        foreach (GameObject k in SelectedLetters)
        {
            int i = (int)k.transform.parent.GetComponent<BoardItem>().myPos.x;
            int j = (int)k.transform.parent.GetComponent<BoardItem>().myPos.y;

            Destroy(Board[i, j].transform.GetChild(0).gameObject);
            Board[i, j].transform.GetComponent<BoardItem>().hasLetter = false;
            Destroy(k);
            TileCount--;
        }


        UpdateRemainingTileCounter();
        OnClickClearButton();

    }



    public void SetButton()
    {
        if (WordTextBox.text == "")
        {
            // EndGameButton.SetActive(true);
            SubmitButton.SetActive(false);
        }
        else
        {
            // EndGameButton.SetActive(false);
            SubmitButton.SetActive(true);
        }
    }

    public void UpdateRemainingTileCounter()
    {
        TileText.text = TileCount.ToString();
    }

    public void EndTurn()
    {
        BonusPoint();

        EndPanelScoreText.text = PlayerScore.ToString();
        EndPanel.transform.Find("Profile").GetComponent<Image>().sprite = GetProfileImage.Get(Globals.avatar);


        if (!Globals.isPractisePuzzle)
        {
            if (PlayerScore >= targetScore)
                APIManager.instance.PostMatchDetails(1, 0, PlayerScore, true, true);
            else if(PlayerScore < targetScore)
                APIManager.instance.PostMatchDetails(0, 1, PlayerScore, true, true);

            if (PlayerScore >= targetScore)
            endPanelTextStatus.text = "Winner – you beat the target";       
        }

        else
        {
            if (PlayerScore < 25)
                endPanelTextStatus.text = "Good Job!";
            else if (PlayerScore >= 25 && PlayerScore < 40)
                endPanelTextStatus.text = "Excellent!";
            else if (PlayerScore >= 40 && PlayerScore < 50)
                endPanelTextStatus.text = "Brilliant!";
            else if (PlayerScore >= 50)
                endPanelTextStatus.text = "Genius!";
        }
               
        

        AudioHandler.instance.EndGame();
        EndPanel.SetActive(true);
    }

    public void GetTodayRank()
    {
        APIManager.instance.GetLeaderBoardDetails(Response, URL.todayDailyPuzzleLeaderBoard);


        void Response(string result)
        {
           
            JArray jsonArray = JArray.Parse(result);
            List<JToken> sortedJsonArray = new List<JToken>();

            sortedJsonArray = jsonArray.OrderByDescending(x => x.SelectToken("score")).ToList();
            endProgressText.gameObject.transform.parent.gameObject.SetActive(true);

           // print("total plyrs "+sortedJsonArray.Count);

            int totalPlyrs = sortedJsonArray.Count;
           // int totalScore = sortedJsonArray.Sum(x => (int)x["sum"]);
            int myIndex = sortedJsonArray.FindIndex(x => x.SelectToken("user_id").ToString() == Globals.dbID);

          //  print("totalscore " + totalScore);
           // print("my index " + (myIndex));

            float percentage = ((float)(totalPlyrs - myIndex) / (float)(totalPlyrs)) * 100f;
            string fP = percentage.ToString("F2");

            endProgressText.text = "Today’s daily puzzle has been played " + sortedJsonArray.Count +
                       " times and your score is in the top " + fP + "%";        
        }
    }
    void BonusPoint()
    {
        int remainingLetterGapInGrid = 28 - addedLettersInGridCount;
        int bonus = 0;

        if (remainingLetterGapInGrid == 0)
            bonus = 50;
        else if (remainingLetterGapInGrid == 1)
            bonus = 25;
        else if (remainingLetterGapInGrid == 2 || remainingLetterGapInGrid == 3)
            bonus = 15;
        else if (remainingLetterGapInGrid == 4 || remainingLetterGapInGrid == 5)
            bonus = 10;

        PlayerScore += bonus;

       // bonusTxt.text = "Remaining tiles : " + remainingLetterGapInGrid + " , Bonus : " + bonus;
        bonusTxt.text = "Bonus : " + bonus;

    }


    public void HomeButton()
    {
        SceneManager.LoadSceneAsync("MenuScene");
        Destroy(gameObject);
    }

    public void RedoButton()
    {
        SceneManager.LoadSceneAsync("GameSceneSingleplayer");
        Start();
    }

    private new void OnApplicationQuit()
    {
        if (Globals.isPractisePuzzle)
            return;

            if (PlayerScore >= targetScore)
            APIManager.instance.PostMatchDetails(1, 0, PlayerScore, true,false);
        else if (PlayerScore < targetScore)
            APIManager.instance.PostMatchDetails(0, 1, PlayerScore, true,false);
    }


}
