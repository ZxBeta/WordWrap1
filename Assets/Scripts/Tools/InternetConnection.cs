using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InternetConnection : MonoBehaviour
{

    [SerializeField]bool disconnected;
    [SerializeField]bool connected;
    [SerializeField] GameObject reconnectingObj;


    private void Update()
    {  
        CheckInternetConnection();
    }

    void CheckInternetConnection()
    {

       // print(Screen.height);
        if (Check() || Globals.isGuestLoggedIn)
        {

            if(!connected)
            {
                connected = true;

                if (disconnected)
                {
                    disconnected = false;
                    reconnectingObj.SetActive(false);
                    if (_ColyseusClient.instance.gameroomJoined)
                    {
                        if (_ColyseusClient.instance.hasTimer)
                            TimeManager.Instance.timerIsRunning = true;
                        _ColyseusClient.instance.JoinGameRoom(null, true);
                    }
                       
                }
            }
        }

        else if (!Globals.isGuestLoggedIn && !Globals.isPractisePuzzle)
        {
            if (!disconnected)
            {
                disconnected = true;
                connected = false;
                if (_ColyseusClient.instance.hasTimer)
                    TimeManager.Instance.timerIsRunning = false;
                reconnectingObj.SetActive(true);
                // opponentDisconnected.SetActive(false);
            }

        }



    }

    private void OnApplicationPause(bool pause)
    {
        if(!pause)
        {
            if (!connected)
            {
                connected = true;

                if (disconnected)
                {
                    disconnected = false;
                    reconnectingObj.SetActive(false);
                    if (_ColyseusClient.instance.gameroomJoined)
                    {
                        if (_ColyseusClient.instance.hasTimer)
                            TimeManager.Instance.timerIsRunning = true;
                        _ColyseusClient.instance.JoinGameRoom(null, true);
                    }

                }
            }
        }

        else
        {
            if (!disconnected)
            {
                disconnected = true;
                connected = false;
                if (_ColyseusClient.instance.hasTimer)
                    TimeManager.Instance.timerIsRunning = false;
                reconnectingObj.SetActive(true);
                // opponentDisconnected.SetActive(false);
            }
        }
    }



    bool Check()
    {
        return !(Application.internetReachability == NetworkReachability.NotReachable
            );
    }

}
