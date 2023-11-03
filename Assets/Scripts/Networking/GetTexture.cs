using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GetTexture : MonoBehaviour
{
    public IEnumerator Get(string url)
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
        Debug.Log("Sending Request " + url);
        yield return www.SendWebRequest();

        Texture2D downloadedTexture = ((DownloadHandlerTexture)www.downloadHandler).texture;

        try
        {
            Sprite downloadedSpriet = Sprite.Create(downloadedTexture, new Rect(0, 0, downloadedTexture.width, downloadedTexture.height),
            new Vector2(0.5f, 0.5f), 100);

        }

        catch (Exception e)
        {
            print(e);
        }
    }
    
}
