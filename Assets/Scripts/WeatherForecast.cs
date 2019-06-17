using System;
using System.Text;
using System.Xml.Serialization;


/// <summary>
/// 天気予報の情報を管理するクラス
/// </summary>
[XmlRoot("weatherforecast")]
public class WeatherForecast
{
    /// <summary>
    /// タイトルを取得または設定します
    /// </summary>
    [XmlElement("title")]
    public string Title { get; set; }

    /// <summary>
    /// XML ファイルの URL を取得または設定します
    /// </summary>
    [XmlElement("link")]
    public string Link { get; set; }

    /// <summary>
    /// 概要を取得または設定します
    /// </summary>
    [XmlElement("description")]
    public string Description { get; set; }

    /// <summary>
    /// 更新日時を取得または設定します
    /// </summary>
    [XmlElement("pubDate")]
    public string PubDate { get; set; }

    /// <summary>
    /// 作成者を取得または設定します
    /// </summary>
    [XmlElement("author")]
    public string Author { get; set; }

    /// <summary>
    /// 編集者を取得または設定します
    /// </summary>
    [XmlElement("managingEditor")]
    public string ManagingEditor { get; set; }

    /// <summary>
    /// 都道府県の情報を取得または設定します
    /// </summary>
    [XmlElement("pref")]
    public Pref[] Pref { get; set; }

    /// <summary>
    /// 現在のオブジェクトを表す文字列を返します
    /// </summary>
    public override string ToString()
    {
        var builder = new StringBuilder();
        builder.AppendLine(@"<weatherforecast>");
        builder.AppendFormat(@"<title>{0}</title>", Title).AppendLine();
        builder.AppendFormat(@"<link>{0}</link>", Link).AppendLine();
        builder.AppendFormat(@"<description>{0}</description>", Description).AppendLine();
        builder.AppendFormat(@"<pubDate>{0}</pubDate>", PubDate).AppendLine();
        builder.AppendFormat(@"<author>{0}</author>", Author).AppendLine();
        builder.AppendFormat(@"<managingEditor>{0}</managingEditor>", ManagingEditor).AppendLine();
        for (int i = 0; i < Pref.Length; i++)
        {
            builder.AppendLine(Pref[i].ToString());
        }
        builder.AppendLine(@"</weatherforecast>");
        return builder.ToString();
    }
}

/// <summary>
/// 確率の情報を管理するクラス
/// </summary>
[XmlRoot("period")]
public class Period
{
    /// <summary>
    /// 時間を取得または設定します
    /// </summary>
    [XmlAttribute("hour")]
    public string Hour { get; set; }

    /// <summary>
    /// テキストを取得または設定します
    /// </summary>
    [XmlText]
    public string Text { get; set; }

    /// <summary>
    /// 値を返します
    /// </summary>
    [XmlText]
    public int Value { get { return int.Parse(Text); } }

    /// <summary>
    /// 現在のオブジェクトを表す文字列を返します
    /// </summary>
    public override string ToString()
    {
        return string.Format(
            @"<period hour=""{0}"">{1}</period>",
            Hour,
            Text
        );
    }
}

/// <summary>
/// 降水確率の情報を管理するクラス
/// </summary>
[XmlRoot("rainfallchance")]
public class Rainfallchance
{
    /// <summary>
    /// 単位を取得または設定します
    /// </summary>
    [XmlAttribute("unit")]
    public string Unit { get; set; }

    /// <summary>
    /// 確率の情報を取得または設定します
    /// </summary>
    [XmlElement("period")]
    public Period[] Period { get; set; }

    /// <summary>
    /// 0 時から 6 時までの確率を返します
    /// </summary>
    public Period Period00To06 { get { return Array.Find(Period, c => c.Hour == "00-06"); } }

    /// <summary>
    /// 6 時から 12 時までの確率を返します
    /// </summary>
    public Period Period06To12 { get { return Array.Find(Period, c => c.Hour == "06-12"); } }

    /// <summary>
    /// 12 時から 18 時までの確率を返します
    /// </summary>
    public Period Period12To18 { get { return Array.Find(Period, c => c.Hour == "12-18"); } }

    /// <summary>
    /// 18 時から 24 時までの確率を返します
    /// </summary>
    public Period Period18To24 { get { return Array.Find(Period, c => c.Hour == "18-24"); } }

    /// <summary>
    /// 現在のオブジェクトを表す文字列を返します
    /// </summary>
    public override string ToString()
    {
        var builder = new StringBuilder();
        builder.AppendFormat(@"<rainfallchance unit=""{0}"">", Unit).AppendLine();
        for (int i = 0; i < Period.Length; i++)
        {
            builder.AppendLine(Period[i].ToString());
        }
        builder.Append(@"</rainfallchance>");
        return builder.ToString();
    }
}

/// <summary>
/// 範囲の情報を管理するクラス
/// </summary>
[XmlRoot("range")]
public class Range
{
    /// <summary>
    /// 摂氏の識別子を取得または設定します
    /// </summary>
    [XmlAttribute("centigrade")]
    public string Centigrade { get; set; }

    /// <summary>
    /// 値を取得または設定します
    /// </summary>
    [XmlText]
    public string Value { get; set; }

    /// <summary>
    /// 現在のオブジェクトを表す文字列を返します
    /// </summary>
    public override string ToString()
    {
        return string.Format(
            @"<range centigrade=""{0}"">{1}</range>",
            Centigrade,
            Value
        );
    }
}

/// <summary>
/// 気温の情報を管理するクラス
/// </summary>
[XmlRoot("temperature")]
public class Temperature
{
    /// <summary>
    /// 単位を取得または設定します
    /// </summary>
    [XmlAttribute("unit")]
    public string Unit { get; set; }

    /// <summary>
    /// 範囲の情報を取得または設定します
    /// </summary>
    [XmlElement("range")]
    public Range[] Range { get; set; }

    /// <summary>
    /// 最大値を返します
    /// </summary>
    public int Max
    {
        get
        {
            var result = Array.Find(Range, c => c.Centigrade == "max");
            return int.Parse(result.Value);
        }
    }

    /// <summary>
    /// 最小値を返します
    /// </summary>
    public int Min
    {
        get
        {
            var result = Array.Find(Range, c => c.Centigrade == "min");
            return int.Parse(result.Value);
        }
    }

    /// <summary>
    /// 平均値を返します
    /// </summary>
    public int Average { get { return (Max + Min) / 2; } }

    /// <summary>
    /// 現在のオブジェクトを表す文字列を返します
    /// </summary>
    public override string ToString()
    {
        var builder = new StringBuilder();
        builder.AppendFormat(@"<temperature unit=""{0}"">", Unit).AppendLine();
        for (int i = 0; i < Range.Length; i++)
        {
            builder.AppendLine(Range[i].ToString());
        }
        builder.Append(@"</temperature>");
        return builder.ToString();
    }
}

/// <summary>
/// 詳細情報を管理するクラス
/// </summary>
[XmlRoot("info")]
public class Info
{
    /// <summary>
    /// 日時を取得または設定します
    /// </summary>
    [XmlAttribute("date")]
    public string Date { get; set; }

    /// <summary>
    /// 天気を取得または設定します
    /// </summary>
    [XmlElement("weather")]
    public string Weather { get; set; }

    /// <summary>
    /// 画像ファイルの URL を取得または設定します
    /// </summary>
    [XmlElement("img")]
    public string Img { get; set; }

    /// <summary>
    /// 天気の詳細を取得または設定します
    /// </summary>
    [XmlElement("weather_detail")]
    public string WeatherDetail { get; set; }

    /// <summary>
    /// 波を取得または設定します
    /// </summary>
    [XmlElement("wave")]
    public string Wave { get; set; }

    /// <summary>
    /// 気温の情報を取得または設定します
    /// </summary>
    [XmlElement("temperature")]
    public Temperature Temperature { get; set; }

    /// <summary>
    /// 降水確率の情報を取得または設定します
    /// </summary>
    [XmlElement("rainfallchance")]
    public Rainfallchance Rainfallchance { get; set; }

    /// <summary>
    /// 現在のオブジェクトを表す文字列を返します
    /// </summary>
    public override string ToString()
    {
        var builder = new StringBuilder();
        builder.AppendFormat(@"<info date=""{0}"">", Date).AppendLine();
        builder.AppendFormat(@"<weather>{0}</weather>", Weather).AppendLine();
        builder.AppendFormat(@"<img>{0}</img>", Img).AppendLine();
        builder.AppendFormat(@"<weather_detail>{0}</weather_detail>", WeatherDetail).AppendLine();
        builder.AppendFormat(@"<wave>{0}</wave>", Wave).AppendLine();
        builder.AppendLine(Temperature.ToString());
        builder.AppendLine(Rainfallchance.ToString());
        builder.Append(@"</info>");
        return builder.ToString();
    }
}

/// <summary>
/// 緯度と経度の情報を管理するクラス
/// </summary>
[XmlRoot("geo")]
public class Geo
{
    /// <summary>
    /// 経度を取得または設定します
    /// </summary>
    [XmlElement("long")]
    public string Long { get; set; }

    /// <summary>
    /// 緯度を取得または設定します
    /// </summary>
    [XmlElement("lat")]
    public string Lat { get; set; }

    /// <summary>
    /// 現在のオブジェクトを表す文字列を返します
    /// </summary>
    public override string ToString()
    {
        var builder = new StringBuilder();
        builder.AppendLine(@"<geo>");
        builder.AppendFormat(@"<long>{0}</long>", Long).AppendLine();
        builder.AppendFormat(@"<lat>{0}</lat>", Lat).AppendLine();
        builder.Append(@"</geo>");
        return builder.ToString();
    }
}

/// <summary>
/// 地域の情報を管理するクラス
/// </summary>
[XmlRoot("area")]
public class Area
{
    /// <summary>
    /// ID を取得または設定します
    /// </summary>
    [XmlAttribute("id")]
    public string Id { get; set; }

    /// <summary>
    /// 緯度と経度の情報を取得または設定します
    /// </summary>
    [XmlElement("geo")]
    public Geo Geo { get; set; }

    /// <summary>
    /// 詳細情報を取得または設定します
    /// </summary>
    [XmlElement("info")]
    public Info[] Info;

    /// <summary>
    /// 現在のオブジェクトを表す文字列を返します
    /// </summary>
    public override string ToString()
    {
        var builder = new StringBuilder();
        builder.AppendFormat(@"<area id=""{0}"">", Id).AppendLine();
        builder.AppendLine(Geo.ToString());
        for (int i = 0; i < Info.Length; i++)
        {
            builder.AppendLine(Info[i].ToString());
        }
        builder.Append(@"</area>");
        return builder.ToString();
    }
}

/// <summary>
/// 都道府県の情報を管理するクラス
/// </summary>
[XmlRoot("pref")]
public class Pref
{
    /// <summary>
    /// ID を取得または設定します
    /// </summary>
    [XmlAttribute("id")]
    public string Id { get; set; }

    /// <summary>
    /// 地域の情報を取得または設定します
    /// </summary>
    [XmlElement("area")]
    public Area[] Area { get; set; }

    /// <summary>
    /// 現在のオブジェクトを表す文字列を返します
    /// </summary>
    public override string ToString()
    {
        var builder = new StringBuilder();
        builder.AppendFormat(@"<pref id=""{0}"">", Id).AppendLine();
        for (int i = 0; i < Area.Length; i++)
        {
            builder.AppendLine(Area[i].ToString());
        }
        builder.Append(@"</pref>");
        return builder.ToString();
    }
}
