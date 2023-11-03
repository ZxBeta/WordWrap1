
using UnityEngine.Networking;
using System.Threading.Tasks;
using System;


public static class GetRequest
{
  
    public static async Task<string> Get<TResultType>(string url)
    {
        try
        {   
            using var www = UnityWebRequest.Get(url);

            www.SetRequestHeader("Content-Type", "application/json");

            var operation = www.SendWebRequest();

            while (!operation.isDone)
                await Task.Yield();

            if (www.result != UnityWebRequest.Result.Success)
                UnityEngine.Debug.Log($"Failed: {www.error}");

            return www.downloadHandler.text;

        }
      
        catch (Exception ex)
        {
            UnityEngine.Debug.Log($"{nameof(Get)} failed: {ex.Message}");  
            return null;
        }
    }
}



