using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class AudioHandler : MonoBehaviour
{
    public static AudioHandler instance;
    public AudioSource oneShotAudioSrc;
    public AudioClip btnPress;
    public AudioClip endedGame;
    public AudioClip submitedWord;
    public AudioClip notAllowedMove;

    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(gameObject);
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }




    public void ButtonPressed()
    {
        oneShotAudioSrc.PlayOneShot(btnPress);
    }

    public void NotValidMove()
    {
        oneShotAudioSrc.PlayOneShot(notAllowedMove);
    }

    public void SubmitWord()
    {
        oneShotAudioSrc.PlayOneShot(submitedWord);
    }

    public void EndGame()
    {
        oneShotAudioSrc.PlayOneShot(endedGame);
    }
}
