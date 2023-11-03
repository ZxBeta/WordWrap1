using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhoneSetting : MonoBehaviour
{
    private void Awake()
    {
        Application.targetFrameRate = 60;
        Application.runInBackground = true;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }
}
