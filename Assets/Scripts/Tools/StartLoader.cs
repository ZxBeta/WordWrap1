using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartLoader : MonoBehaviour
{
    GameObject loaderObj;
    GameObject spawnedLoaderObj;

    public StartLoader(GameObject targetObj )
    {
        if (targetObj == null)
            targetObj = GameObject.Find("Canvas").gameObject;

        loaderObj = (Resources.Load("Loading") as GameObject);
        spawnedLoaderObj = Instantiate(loaderObj, targetObj.transform, false);
      
    }

    public void Destroy()
    {
        Destroy(spawnedLoaderObj);
    }
}
