using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CountDownTimer : MonoBehaviour
{
    public float timeRemaining = 10;
    public bool timerIsRunning = false;
    public TMP_Text displayTimeText;
    public EventsBus timeOutEvent = new EventsBus();

    void Update()
    {
        if (timerIsRunning)
        {
            if (timeRemaining > 0)
            {
               // Debug.Log("Timer");
                timeRemaining -= Time.deltaTime;
                displayTimeText.text =((int)timeRemaining).ToString();
            

            }
            else
            {
               // Debug.Log("Time has run out!");         
                timeRemaining = 0;
                timerIsRunning = false;
                displayTimeText.text = ((int)timeRemaining).ToString();
                timeOutEvent.Invoke();


            }
        }

       

    }
}
