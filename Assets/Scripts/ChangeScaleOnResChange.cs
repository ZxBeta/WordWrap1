using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeScaleOnResChange : MonoBehaviour
{
    public Vector3 scaleForTab;
    public Vector3 posForTab;
    public bool setPos;
    public bool setLocalPos;
    public bool setScale;


    private void OnEnable()
    {
        IsTablet();

    }

 


    public void IsTablet()
    {
        if(Globals.currentAspectRatio==0)
            Globals.currentAspectRatio = (float)Screen.height / (float)Screen.width; 

        Debug.Log("Aspect Ratio:" + Globals.currentAspectRatio + " || height " + Screen.height + " width "+Screen.width);
       
        if(Globals.currentAspectRatio >= 1.2f && Globals.currentAspectRatio <= 1.5f)
        {
          
            if(setPos)
                transform.position = posForTab;
            if(setLocalPos)
                transform.localPosition = posForTab;
            if (setScale)
                transform.localScale = scaleForTab;
            Debug.Log("4:3");
        }
    }
}
