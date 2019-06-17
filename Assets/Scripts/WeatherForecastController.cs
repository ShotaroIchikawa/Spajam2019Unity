using System.Collections;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

public class WeatherForecastController : MonoBehaviour
{
    private void Awake()
    {
        StartCoroutine(LoadWeatherforecast());
    }

    private IEnumerator LoadWeatherforecast()
    {
        var www = new WWW("http://www.drk7.jp/weather/xml/01.xml");

        while (!www.isDone)
        {
            yield return null;
        }

        var text = www.text;
        var xs = new XmlSerializer(typeof(WeatherForecast));
        var sr = new StringReader(text);
        var obj = xs.Deserialize(sr) as WeatherForecast;
        Debug.Log(obj.Title);
        Debug.Log(obj.Pref);
        Debug.Log(obj.PubDate);
        Debug.Log(obj.Description);
    }
}