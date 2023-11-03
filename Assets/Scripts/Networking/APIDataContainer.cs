using System;
using System.Collections.Generic;
using UnityEngine;

public class APIDataContainer : MonoBehaviour
{
    public static APIDataContainer instance;

    public DailyPuzzle dailyPuzzle;
    public List<Sprite> AvatarImagelist;
    public List<Player> onlinePlayersList = new List<Player>();


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
}

[Serializable]
public class DailyPuzzle
{
    public List<DailyPuzzleAPIRoot> dailyPuzzleAPIRootList = new List<DailyPuzzleAPIRoot>();
}
