using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneLoader
{
    public enum Scene
    {
        GameSceneSingleplayer,
        GameSceneMultiplayer,
        MenuScene,
        SampleScene
    }


    public static void LoadScene(Scene scene)
    {
        SceneManager.LoadSceneAsync(scene.ToString());
    }

}
