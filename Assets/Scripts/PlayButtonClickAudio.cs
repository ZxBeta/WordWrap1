using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayButtonClickAudio : MonoBehaviour
{

    private void Start()
    {
        transform.GetComponent<Button>().onClick.AddListener(Play);
    }

    void Play()
    {
        AudioHandler.instance.ButtonPressed();
    }
}
