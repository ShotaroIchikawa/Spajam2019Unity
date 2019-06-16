using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

public class WeatherLoader : MonoBehaviour
{
    public string BaseUrl = "http://api.openweathermap.org/data/2.5/weather";
    public string ApiKey = "73c1dd52d7bac9af74f5d2fbd1f03244"; // APIキーを指定する
    public int TimeOutSeconds = 10;

    public float latitude;
    public float longitude;
    public WeatherLoader Loader;

    void Start()
    {
        StartCoroutine(Load(latitude, longitude, weatherEntity =>
        {
            Render(weatherEntity);
        }));
    }

    void Render(WeatherEntity weatherEntity)
    {
        //WeatherText.text = string.Format("weather:{0}", weatherEntity.weather[0].main);
        Debug.Log(string.Format("weather:{0}", weatherEntity.weather[0].main));
        //WindForceText.text = string.Format("wind: {0}m", weatherEntity.wind.speed);
        Debug.Log(string.Format("wind: {0}m", weatherEntity.wind.speed));
    }

    public IEnumerator Load(double latitude, double longitude, UnityAction<WeatherEntity> callback)
    {
        var url = string.Format("{0}?lat={1}&lon={2}&appid={3}", BaseUrl, latitude.ToString(), longitude.ToString(), ApiKey);
        var request = UnityWebRequest.Get(url);
        var progress = request.Send();

        int waitSeconds = 0;
        while (!progress.isDone)
        {
            yield return new WaitForSeconds(1.0f);
            waitSeconds++;
            if (waitSeconds >= TimeOutSeconds)
            {
                Debug.Log("timeout:" + url);
                yield break;
            }
        }

        if (request.isNetworkError)
        {
            Debug.Log("error:" + url);
        }
        else
        {
            string jsonText = request.downloadHandler.text;
            callback(JsonUtility.FromJson<WeatherEntity>(jsonText));
            yield break;
        }
    }
}

[System.Serializable]
public class WeatherEntity
{
    public Weather[] weather;
    public Wind wind;
}

[System.Serializable]
public class Weather
{
    public string main; // Rain, Snow, Clouds ... etc
}

[System.Serializable]
public class Wind
{
    public float deg;
    public float speed;
}
