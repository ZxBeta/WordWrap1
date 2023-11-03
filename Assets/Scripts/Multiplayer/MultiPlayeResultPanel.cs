using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MultiPlayeResultPanel : MonoBehaviour
{
    public GameObject endPanel;
    public Image[] playersImage;
    public TMP_Text[] playersName, playerScore;
    public GameObject[] playersObj;

    public static MultiPlayeResultPanel instance;

    private void Awake()
    {
        instance = this;
    }


    private void Start()
    {
        InitPlayers();
    }

    public void InitPlayers()
    {
        int oppCount = _ColyseusClient.instance.totalPlayersCount - 1;

        for (int i=0; i < oppCount ; i++)
        {
            print("sssss");
            playersObj[i].SetActive(true);
        }
    }
}
