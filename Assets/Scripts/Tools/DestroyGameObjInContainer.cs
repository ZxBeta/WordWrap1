using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyGameObjInContainer : MonoBehaviour
{    
    public DestroyGameObjInContainer(GameObject Container)
    {
        if(Container.transform.childCount > 0)
        {
            for (int i = 0; i < Container.transform.childCount; i++)
                Destroy(Container.transform.GetChild(i).gameObject);
        }       

    }  

}
