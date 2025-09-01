using System;

[Serializable]
public class WeatherData
{
    public WeatherProperties properties;
}

[Serializable]
public class WeatherProperties
{
    public WeatherPeriod[] periods;
}

[Serializable]
public class WeatherPeriod
{
    public int number;
    public string name;
    public int temperature;
    public string temperatureUnit;
    public string shortForecast;
    public string icon;
}
