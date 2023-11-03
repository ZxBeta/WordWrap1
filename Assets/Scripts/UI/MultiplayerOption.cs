using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MultiplayerOption : MonoBehaviour
{
   public void SetNoOfPlayers(InputField p)
   {
        int val = int.Parse(p.text);

        if (val <= 0)
        { val = 2; p.text = val.ToString(); }

        if (val > 4)
        { val = 4; p.text = val.ToString(); }

        _ColyseusClient.instance.totalPlayersCount = val;

    }

    public void SetNoOfLevels(InputField l)
    {
        int val = int.Parse(l.text);

        if (val <= 0)
        { val = 1; l.text = val.ToString(); }

        if (val > 4)
        { val = 4; l.text = val.ToString(); }

        _ColyseusClient.instance.totalRounds = val;
    }

    public void SetTimeValue(InputField time)
    {
       _ColyseusClient.instance.totalTime = int.Parse(time.text); 
    }

    public void SetTimer(Toggle toggle)
    {
        _ColyseusClient.instance.hasTimer = toggle.isOn;
    }

  
}
