
using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;


public static class DeleteRequest 
{
    public static async Task<string> Delete(string URL)
    {
        using (UnityWebRequest request = UnityWebRequest.Delete(URL))
        {
            try
            {
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
                UnityEngine.Debug.Log($"{nameof(Delete)} failed: {ex.Message}");
                return null;
            }


        }


    }
}
