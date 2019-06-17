using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeatherTest : MonoBehaviour
{
    private void Awake()
    {
        StartCoroutine(LoadWeatherforecast());
    }

    private IEnumerator LoadWeatherforecast()
    {
        var www = new WWW("https://api.darksky.net/forecast/e747bff29f4a249e4793652f08bdda0b/37.8267,-122.4233");

        while (!www.isDone)
        {
            yield return null;
        }

        var jsonString = www.text;

        string weather = "";
        while (true)
        {
            // summaryから始まる部分を探す
            if (jsonString.StartsWith("summary"))
            {
                // summaryの次のstringを抜き出す処理
                // "summary":" の部分がいらないため11文字Removeする
                jsonString = jsonString.Remove(0, 10);
                

                while (!jsonString.StartsWith("\"")) 
                {
                    weather += jsonString[0];
                    jsonString = jsonString.Remove(0, 1);

                }
            }
            else
            {
                // 1文字目を削除->jsonStringを先に進めていくイメージ
                jsonString = jsonString.Remove(0, 1);
            }

            if (jsonString.StartsWith("minutely"))
            {
                break;
            }
        }
        Debug.Log(weather);
    }
}
