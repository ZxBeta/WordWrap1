using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WildLetterHandler : MonoBehaviour
{
    public GameObject WildLetterPanel;
    public GameObject LetterPrefab;

    public static WildLetterHandler instance;

    private void Awake()
    {
        instance = this;
    }

    public void FillWildLetterPanel(Dictionary<string, int> letterFrequency)
    {
        Debug.Log("Filling panel");
        if (!WildLetterPanel.activeSelf)
        {
            WildLetterPanel.SetActive(true);
            if (WildLetterPanel.transform.childCount == 0)
            {
                foreach (var element in letterFrequency)
                {
                    if (element.Key != "*")
                    {
                        GameObject current = Instantiate(LetterPrefab, WildLetterPanel.transform);
                        current.GetComponent<LetterScript>().initializeLetter(element.Key, 1, false, true);
                    }
                }
            }
        }
    }

    public void CloseWildLetterPanel()
    {
        if (WildLetterPanel.activeSelf)
            WildLetterPanel.SetActive(false);
    }

    public void FitWildLetter(string st, TMP_Text WordTextBox)
    {
        WordTextBox.text = WordTextBox.text.ToString().Replace("*", st);
    }
}
