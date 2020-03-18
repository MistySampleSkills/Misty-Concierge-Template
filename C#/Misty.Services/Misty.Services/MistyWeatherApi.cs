using System;
using System.Net.Http;
using MistyRobotics.SDK.Messengers;
using Newtonsoft.Json;

namespace Misty.Services
{
    /// <summary>
    /// This service calls a weather API to return the current temperature and weather description to Misty.
    /// </summary>
    public class MistyWeatherApi
    {
        private IRobotMessenger _misty;

        public MistyWeatherApi(IRobotMessenger misty1)
        {
            _misty = misty1;
        }

        /// <summary>
        /// Calls the weather api to return the current temperature and weather description.
        /// API Website:  https://weatherstack.com/
        /// </summary>
        /// <param name="location">the location to retrieve the weather information for</param>
        /// <returns></returns>
        public string GetCurrentWeatherInfo(string location)
        {
            string currentWeatherText = "";
            try
            {
                // Current Weather API Endpoint
                // http://api.weatherstack.com/current
                var currentWeatherUrl = MistyApiInfo.WeatherBaseUrl +
                    "/current" +
                    "?access_key=" + MistyApiInfo.WeatherApiKey +
                    "&query=" + location +
                    "&units=m";
                _misty.SkillLogger.LogVerbose($"Misty.Services: IN GetCurrentWeatherInfo() -- currentWeatherUrl: {currentWeatherUrl}");
                var response = GetResponse(currentWeatherUrl);
                Newtonsoft.Json.Linq.JObject jsonResp = Newtonsoft.Json.Linq.JObject.Parse(response);
                _misty.SkillLogger.LogVerbose($"Misty.Services: IN GetCurrentWeatherInfo() -- jsonResp: {jsonResp.ToString(Formatting.Indented)}");
                // {
                //    "request": {
                //        "type": "City",
                //        "query": "New York, United States of America",
                //        "language": "en",
                //        "unit": "m"
                //                },
                //    "location": {
                //        "name": "New York",
                //        "country": "United States of America",
                //        "region": "New York",
                //        "lat": "40.714",
                //        "lon": "-74.006",
                //        "timezone_id": "America/New_York",
                //        "localtime": "2019-09-07 08:14",
                //        "localtime_epoch": 1567844040,
                //        "utc_offset": "-4.0"
                //    },
                //    "current": {
                //        "observation_time": "12:14 PM",
                //        "temperature": 13,
                //        "weather_code": 113,
                //        "weather_icons": [
                //            "https://assets.weatherstack.com/images/wsymbols01_png_64/wsymbol_0001_sunny.png"
                //        ],
                //        "weather_descriptions": [
                //            "Sunny"
                //        ],
                //        "wind_speed": 0,
                //        "wind_degree": 349,
                //        "wind_dir": "N",
                //        "pressure": 1010,
                //        "precip": 0,
                //        "humidity": 90,
                //        "cloudcover": 0,
                //        "feelslike": 13,
                //        "uv_index": 4,
                //        "visibility": 16
                //    }
                //}
                string weatherDescs = "";
                var descs = jsonResp["current"]["weather_descriptions"];
                foreach (var desc in descs)
                {
                    weatherDescs += " " + desc;
                }
                currentWeatherText = $"The current temperature for {location} is {(string)jsonResp["current"]["temperature"]} degrees celcius and it is {weatherDescs}";
                _misty.SkillLogger.LogVerbose($"Misty.Services: IN GetCurrentWeatherInfo() -- currentWeatherText: {currentWeatherText}");
            }
            catch (Exception ex)
            {
                _misty.SkillLogger.Log($"Misty.Services:: IN GetCurrentWeatherInfo() => Exception", ex);
            }
            return currentWeatherText;
        }

        private string GetResponse(string url)
        {
            using (var httpClient = new HttpClient())
            {
                return httpClient.GetStringAsync(new Uri(url)).Result;
            }
        }
    }
}
