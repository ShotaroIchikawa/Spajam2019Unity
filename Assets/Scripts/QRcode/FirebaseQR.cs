using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

public class FirebaseQR
{

    const string cfUrl = "https://us-central1-spajam2019-1560354088089.cloudfunctions.net/";

    class Item1
    {
        public string lockerId;
        public string password;
    }
    class Item12
    {
        public string boxId;
    }
    class Item2
    {
        public string lockerId;
        public string boxId;
    }

    public IEnumerator CloseBox(string lockerId, string password, Action<string> callback)
    {
        const string url = cfUrl + "closeBox";

        var item = new Item1();
        item.lockerId = lockerId;
        item.password = password;

        Debug.Log(JsonUtility.ToJson(item));
        byte[] data = System.Text.Encoding.UTF8.GetBytes(JsonUtility.ToJson(item));
        var request = new UnityWebRequest(url, "POST");
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(data);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        yield return request.SendWebRequest();

        if (request.isHttpError || request.isNetworkError)
        {
            Debug.LogError(request.error);
            yield break;
        }

        var text = request.downloadHandler.text;
        Item12 result = JsonUtility.FromJson<Item12>(text);

        callback(result.boxId);
    }

    public IEnumerator OpenBox(string lockerId, string boxId, Action<bool> callback)
    {
        const string url = cfUrl + "openBox";
        var item = new Item2();
        item.lockerId = lockerId;
        item.boxId = boxId;

        Debug.Log(JsonUtility.ToJson(item));
        byte[] data = System.Text.Encoding.UTF8.GetBytes(JsonUtility.ToJson(item));
        var request = new UnityWebRequest(url, "POST");
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(data);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        yield return request.SendWebRequest();

        if (request.isHttpError || request.isNetworkError)
        {
            Debug.LogError(request.error);
            yield break;
        }

        callback(request.downloadHandler.text == "true");
    }
}
