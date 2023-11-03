using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UserDetailsBase;

public class InAppMessage : MonoBehaviour
{

    public GameObject msgPrefab;
    public Transform spawnPoint;
    public GameObject notificationObj;

    private void Start()
    {
       GetMessage();
    }


    public void GetMessage()
    {
        APIManager.instance.GetInAppMessage(Response);

        void Response(string result)
        {
            print(result);

            if(spawnPoint != null)
            new DestroyGameObjInContainer(spawnPoint.gameObject);

            var recivedClass = JsonConvert.DeserializeObject<List<InAppMessageBase>>(result);

            //if (recivedClass.Count < 1)
            //    return;

            int count;

            count = SaveLoadSystem.LoadInAppMsgCount();

            if (count == -1)
            {
                SaveLoadSystem.SaveInAppMsgCount(recivedClass.Count);
            }

            else if (recivedClass.Count != count)
            {
                notificationObj.SetActive(true);
                SaveLoadSystem.SaveInAppMsgCount(recivedClass.Count);
            }       

            int index = 0;
            foreach (var i in recivedClass)
            {
                SpawnMessage(recivedClass[index].message, (index+1).ToString());
                index++;
            }      
        }
    }

    void SpawnMessage(string msg, string id)
    {
        GameObject msgObj = Instantiate(msgPrefab, spawnPoint);
        msgObj.transform.GetComponent<TMP_Text>().text = "<b>"+id+"</b>"+". "+ msg;
    }



}
