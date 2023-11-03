using System.Collections;
using System.Collections.Generic;
using TMPro;
using Tools;
using UnityEngine;
using UnityEngine.UI;

public class SliderValue : MonoBehaviour
{
    [SerializeField] bool setToOdd;

   public void SetValue(Slider slider)
   {      
        if(setToOdd)
        {
            var oddNo = ConvertInteger.ConverToOdd((int)slider.value);
            GetComponent<TMP_Text>().text = oddNo.ToString();
        }

        else
        GetComponent<TMP_Text>().text = slider.value.ToString();      
   }
}
