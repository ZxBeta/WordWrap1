using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformGameObj : MonoBehaviour
{
    public enum ObjPlatform
    { 
        Android,
        IOS
    
    
    }

    public ObjPlatform platform;

    private void Start()
    {
        if (platform == ObjPlatform.Android)
        {
            if (Application.platform != RuntimePlatform.Android)
            {
                gameObject.SetActive(false);
            }
        }

        else if(platform == ObjPlatform.IOS)
        {
            if (Application.platform != RuntimePlatform.IPhonePlayer)
            {
                gameObject.SetActive(false);
            }
        }
    }


}
