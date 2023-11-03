using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class LeaderBoardManager : MonoBehaviour
{
    public GameObject rankDetailsPrefab;
    public Transform listSpawnTarget;
    public List<LeaderBoardRoot> leaderBoardRootList = new List<LeaderBoardRoot>();
    public TMP_Text PlayedAndScoreText;
    public TMP_Text score_GameWonText;


    private void OnEnable()
    {
        GetDailyRank();

    }
    public void GetMultiPlayerRank()
    {
        PlayedAndScoreText.gameObject.SetActive(true);
        score_GameWonText.gameObject.SetActive(true);
        GetRankDetails(URL.multiplayerLeaderBoard, 1);
    }

    public void GetDailyRank()
    {
        PlayedAndScoreText.gameObject.SetActive(true);
        score_GameWonText.gameObject.SetActive(true);  
        GetRankDetails(URL.dailyPuzzleLeaderBoard, 2);
    }

    public void GetTodayDailyRank()
    {
        PlayedAndScoreText.gameObject.SetActive(false);
        score_GameWonText.gameObject.SetActive(false);
        GetRankDetails(URL.todayDailyPuzzleLeaderBoard, 3);
    }

    //1 multiplyr
    //2 dailypuzzle
    //3 todaypuzzle

    public void GetRankDetails(string url, int puzzleIndex)
    {
        StartLoader startLoader = new StartLoader(gameObject);

        if(listSpawnTarget != null)
        new DestroyGameObjInContainer(listSpawnTarget.gameObject);
        print(url);
        leaderBoardRootList.Clear();

        APIManager.instance.GetLeaderBoardDetails(Response, url);

        void Response(string result)
        {
            JArray jsonArray = JArray.Parse(result);
            List<JToken> sortedJsonArray = new List<JToken>();
            print(result);
            if (jsonArray.Count <= 0)
            {
                startLoader.Destroy();
                return;
            }


            if (puzzleIndex == 1)
                //sortedJsonArray = jsonArray.OrderBy(x => x.SelectToken("rank")).ToList();
                sortedJsonArray = jsonArray.OrderByDescending(x => (float)x["wins"] / ((int)x["wins"] + (int)x["loss"])).ToList();
            if (puzzleIndex == 2)
                sortedJsonArray = jsonArray.OrderByDescending(x => (float)x["wins"] / ((int)x["wins"] + (int)x["loss"])).ToList();  
            if(puzzleIndex == 3)            
                sortedJsonArray = jsonArray.OrderByDescending(x => x.SelectToken("todayScore")).ToList();

            List<int> ids = new List<int>();

            for (int i = 0; i < sortedJsonArray.Count; i++)
            {
                var recivedClass = JsonConvert.DeserializeObject<LeaderBoardRoot>(sortedJsonArray[i].ToString());

                if (ids.Count > 0)
                    if (ids.Contains(recivedClass.user_id))
                        continue;

                ids.Add(recivedClass.user_id);

                leaderBoardRootList.Add(recivedClass);

                GameObject obj = Instantiate(rankDetailsPrefab, listSpawnTarget);
                UserRankDetailButton userRankDetailButton = obj.GetComponent<UserRankDetailButton>();              
                userRankDetailButton.nameText.text = recivedClass.username != null ? recivedClass.username : "WordWrapUser";
                userRankDetailButton.ChangeDefaultColorByRank(i);
                userRankDetailButton.totalGamePlayedText.text = (recivedClass.wins + recivedClass.loss).ToString();

                if (puzzleIndex != 3)
                {
                    var winningPerecentage = (recivedClass.wins * 100) / (recivedClass.wins + recivedClass.loss);
                    userRankDetailButton.percentText.text = winningPerecentage.ToString();
                }
                  
                else
                {
                    userRankDetailButton.totalGamePlayedText.gameObject.SetActive(false);
                    userRankDetailButton.gameWonText.gameObject.SetActive(false);
                    userRankDetailButton.percentText.text = recivedClass.todayScore.ToString();
                }
              

                userRankDetailButton.gameWonText.text = recivedClass.wins.ToString();
                         
     
                if(puzzleIndex == 1)
                {                
                    if (recivedClass.user_id.ToString() == Globals.dbID)
                    {
                        UserProfileManager.eventUpdateProgress(recivedClass);
                    }
                }

                startLoader.Destroy();

            }
        }



    }
}
