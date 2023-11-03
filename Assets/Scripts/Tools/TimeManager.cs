using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LS;
using TMPro;

public class TimeManager : Singleton<TimeManager>
{
    public TextMeshProUGUI TimeDisplay;
    public GameObject TimerObj;
    public float timeRemaining = 10;
    public bool timerIsRunning = false;
    public EventsBus timeOutEvent = new EventsBus();


    private void Update()
    {
        if (timerIsRunning)
        {
            if (timeRemaining > 0)
            {
                // Debug.Log("Timer");
                timeRemaining -= Time.deltaTime;
                int time = (int)timeRemaining;
                TimeDisplay.text = (time / 60).ToString("d2") + ": " + (time % 60).ToString("d2");

            }
            else
            {
                Debug.Log("Time has run out!");         
                timeRemaining = 0;
                timerIsRunning = false;
                int time = (int)timeRemaining;
                TimeDisplay.text = (time / 60).ToString("d2") + ": " + (time % 60).ToString("d2");
                timeOutEvent.Invoke();


            }
        }
    }


    public void StopWatchTimer()
    {
        StartCoroutine(TimeIncrement());
    }

    public void ShowTimer(bool status)
    {
        TimerObj.SetActive(status);
    }

    IEnumerator TimeIncrement()
    {
        int time = 0;
        TimeDisplay.text = (time / 60).ToString("d2") + ": " + (time % 60).ToString("d2");
        while (time<600)//10 mins
        {
            yield return new WaitForSeconds(1);
            time++;
            TimeDisplay.text = (time / 60).ToString("d2") + ": " + (time % 60).ToString("d2");
        }
        Debug.Log("end");
    }
}
