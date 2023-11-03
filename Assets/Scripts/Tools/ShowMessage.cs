using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowMessage : MonoBehaviour
{
    public static ShowMessage instance;
    GameObject msgBoxObj;
    GameObject spawnedMsgBoxBoj;


    private void Awake()
    {
        instance = this;
    }

    public void Show(string msg)
    {
        msgBoxObj = (Resources.Load("msgpopup") as GameObject);
        spawnedMsgBoxBoj = Instantiate(msgBoxObj, transform, false);
        spawnedMsgBoxBoj.transform.GetChild(0).Find("Text").GetComponent<Text>().text = msg;

        if (Globals.msgTime == 0)
               Globals.msgTime = 2;
        

        Destroy(spawnedMsgBoxBoj, Globals.msgTime);

        Globals.msgTime = 0;

    }
}
