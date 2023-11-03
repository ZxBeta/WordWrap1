 using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject waitingLobbyPanel;
    public GameObject createRoomPanel;
    public GameObject loadingPanel;
    public GameObject mainLoginPanel;
    public GameObject challengeMorePopup;
    public GameObject challengeRecievePopup;
    public GameObject multiplayerObj;
    public GameObject OnlineplayerObj;
    public GameObject[] sharingCodeObjs;
    public static UIManager instance;

    private void Awake()
    {
        instance = this;
    }

    #region Multiplayer
    public void JoinRandomLobby()
    {
        if (!Globals.isLoggedIn)
        { ShowMessage.instance.Show("Please Login first"); return; }

        _ColyseusClient.instance.totalPlayersCount = 4;
        _ColyseusClient.instance.totalRounds = 3;
        _ColyseusClient.instance.totalTime = 150;
        _ColyseusClient.instance.hasTimer = true;
        SharingObjsStatus(sharingCodeObjs, false);
        _ColyseusClient.instance.CreateOrJoinRandomLobby();
 
    }

    public void CreatePrivateRoom(TMP_Text roomIdText)
    {
        if (!Globals.isLoggedIn)
        { ShowMessage.instance.Show("Please Login first"); return; }

        SharingObjsStatus(sharingCodeObjs,true);
        _ColyseusClient.instance.CreatePrivateLobby(roomIdText);

        //  _ColyseusClient.instance.roomCreator = true;
    }

    public void JoinPrivateRoom(TMP_InputField roomIdText)
    {

        if (!Globals.isLoggedIn)
        { ShowMessage.instance.Show("Please Login first"); return; }

        print(roomIdText.text);
        if (roomIdText.text == "")
        { ShowMessage.instance.Show("Enter room ID"); return; }
        SharingObjsStatus(sharingCodeObjs, false);
        _ColyseusClient.instance.JoinPrivateLobby(roomIdText.text,null);

        //   _ColyseusClient.instance.roomCreator = false;
    }
    public void SharingObjsStatus(GameObject[] obj, bool flag)
    {
        foreach (var i in obj)
        {
            i.SetActive(flag);
        }
    }


    public void SendCode(TMP_Text textToCopy)
    {
        GUIUtility.systemCopyBuffer = textToCopy.text;
        NativeSharing.ShareText(textToCopy.text);      
    }

    public void OnWaitingLobby()
    {
        waitingLobbyPanel.SetActive(true);
    }

    public void LeaveLobbyRoom() 
    {
        _ColyseusClient.instance.LeaveLobby();
    }

    #endregion

    public void InviteFriends()
    {
        string text = "Come join me in Word Wrap game!";
        NativeSharing.ShareText(text);
    }





}
