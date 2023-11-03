using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using static ColyseusRoomBase;

public class ChallengeManager : MonoBehaviour
{
    public static ChallengeRecived challengeRecived;
    public TMP_Text challengerName;
    public static UnityEvent showChallenge = new UnityEvent();


    private void Awake()
    {
        showChallenge.AddListener(ShowChallengeRequest);
    }

    public void ShowChallengeRequest()
    {
        print(challengeRecived.roomId);
        
        challengerName.text = challengeRecived.senderUserName + " has challenged you";
        UIManager.instance.challengeRecievePopup.SetActive(true);
    }

    public void GoToLobby()
    {
        _ColyseusClient.instance.totalPlayersCount = challengeRecived.minplayersToMatch;
        UIManager.instance.SharingObjsStatus(UIManager.instance.sharingCodeObjs, false);
        UIManager.instance.multiplayerObj.SetActive(true);
        _ColyseusClient.instance.JoinPrivateLobby(challengeRecived.roomId,Callback);
        UIManager.instance.multiplayerObj.SetActive(true);
        Globals.totalPlyrsInChalangeLobby = 1;
        
        void Callback()
        {
            UIManager.instance.OnlineplayerObj.SetActive(false);
        }
    }

 

}
