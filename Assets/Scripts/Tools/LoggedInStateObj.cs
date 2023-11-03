using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoggedInStateObj : MonoBehaviour
{

    public GameObject[] objs;
    public static LoggedInStateObj i;

    private void Awake()
    {
        i = this;
    }

    public void SetObj()
    {
        if(Globals.isLoggedIn)
            foreach(var o in objs)
                o.SetActive(Globals.isLoggedIn);
        else
            foreach (var o in objs)
                o.SetActive(Globals.isLoggedIn);
    }
}
