
using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public static class PostRequest
{
  
    public static async Task<string> Post(string URL, string jsonData)
    {
        using (UnityWebRequest request = UnityWebRequest.Post(URL, jsonData))
        {
            try
            {           
                byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(jsonData);
                request.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
                request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");
                var operation = request.SendWebRequest();

                while (!operation.isDone)
                    await Task.Yield();

                if (request.result != UnityWebRequest.Result.Success)
                    UnityEngine.Debug.Log($"Failed: {request.error}");

                return request.downloadHandler.text;
            }

            catch (Exception ex)
            {
                UnityEngine.Debug.Log($"{nameof(Post)} failed: {ex.Message}");
                return null;
            }

 
        }


    }


}
