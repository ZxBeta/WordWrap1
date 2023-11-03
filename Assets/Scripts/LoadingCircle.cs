using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingCircle : MonoBehaviour
{
    public static LoadingCircle instance;

    public GameObject loadingCircleObj;


    private void Awake()
    {
        instance = this;
    
    }

    public void Loading(bool flag)
    {
        loadingCircleObj.SetActive(flag);
    }
}
