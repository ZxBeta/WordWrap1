using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static ColyseusRoomBase;

public class MultiPlayerUI : MonoBehaviour
{
    public Image[] playersImage;
    public TMP_Text[] score;
    public TMP_Text[] playerName;
    public GameObject[] playersObj;
    public GameObject[] playersIndicator;
    LobbyRoomPlayerInfo lobbyRoomInfo;
    public static Action<int, int> ScoreUpdateaction;
    public static Action GameEndedAction;


    public static MultiPlayerUI i;

    private void Awake()
    {
        i = this;
        ScoreUpdateaction += UpdateScore;
        GameEndedAction += ShowEndPanel;
    }

    private void Start()
    {

      

        playersObj[0].SetActive(false);
        playersObj[1].SetActive(false);
        playersObj[2].SetActive(false);
        playersObj[3].SetActive(false);

        lobbyRoomInfo = _ColyseusClient.instance.LobbyRoomInfo;

        int i = 0;

        foreach (Player p in lobbyRoomInfo.gamePlayerList)
        {
            int index = (int)p.index;

            playersObj[index].SetActive(true);

            if(p.dbId.ToString() == Globals.dbID)
                playerName[index].text = "You";

            else
                playerName[index].text = p.userName;

            if(_ColyseusClient.instance.currentRound<2)
            score[index].text = 0.ToString();

            int avIndex = int.Parse(p.avatar);
            playersImage[index].sprite = APIDataContainer.instance.AvatarImagelist[avIndex];

            i++;
        }

     
    }

    void UpdateScore(int pIndex, int pScore)
    {
        _ColyseusClient.instance.playersTotalScoreDict[pIndex] = pScore;
        MultiplayerGameManager.Instance.playersCurrentRoundScoreDict[pIndex] = pScore;
        score[pIndex].text = pScore.ToString();
    }

    public void UpdatePlayerIndicator(int pIndex)
    {
        playersIndicator[0].gameObject.SetActive(false);
        playersIndicator[1].gameObject.SetActive(false);
        playersIndicator[2].gameObject.SetActive(false);
        playersIndicator[3].gameObject.SetActive(false);

        playersIndicator[pIndex].gameObject.SetActive(true);
    }

    public void ShowEndPanel()
    {

        int index = 0;
        int winnerIndex = 0;
        int origScore = 0;

        if (MultiplayerGameManager.Instance.gameRoomPlayers.Count <= 1)
        {
          
            var i = MultiplayerGameManager.Instance.gameRoomPlayers[0].index;
            print(MultiplayerGameManager.Instance.gameRoomPlayers[0].userName + " win by");
            _ColyseusClient.instance.playersTotalWinCount[(int)i] = 10;
            origScore = _ColyseusClient.instance.playersTotalScoreDict[(int)i];
            _ColyseusClient.instance.playersTotalScoreDict[(int)i] = 1000;   
        }

        foreach (KeyValuePair<int, int> player in _ColyseusClient.instance.playersTotalScoreDict.OrderByDescending(key => key.Value))
        {
            if (index == 0)
            {
                winnerIndex = player.Key;
                if (MultiplayerGameManager.Instance.gameRoomPlayers.Count <= 1)
                    _ColyseusClient.instance.playersTotalScoreDict[(int)MultiplayerGameManager.Instance.gameRoomPlayers[0].index] = origScore;
            }
                

            MultiPlayeResultPanel.instance.playerScore[index].text = _ColyseusClient.instance.playersTotalScoreDict[player.Key].ToString();
            MultiPlayeResultPanel.instance.playersImage[index].sprite = playersImage[player.Key].sprite;
            MultiPlayeResultPanel.instance.playersName[index].text = playerName[player.Key].text;
            index++;
        }

        MultiplayerGameManager.Instance.PostMyMatchDetails(winnerIndex);
        MultiPlayeResultPanel.instance.endPanel.SetActive(true);
        _ColyseusClient.instance.currentRound = 1;
    }


}
