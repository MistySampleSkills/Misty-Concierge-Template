using System;
using System.Net.Http;
using MistyRobotics.SDK.Messengers;

namespace Misty.Services
{
    /// <summary>
    /// This service calls a News API to return one of the top news headlines from the site.
    /// </summary>
    public class MistyNewsApi
    {
        public IRobotMessenger _misty;
        /// <summary>
        /// Random number generator
        /// </summary>
        private Random _randomGenerator = new Random();
        private const int _maxNewsReturned = 20;

        public MistyNewsApi(IRobotMessenger misty1)
        {
            _misty = misty1;
        }

        /// <summary>
        /// Calls a News API to return one of the top news headlines from the site.
        /// API Website:  https://newsapi.org/docs/get-started
        /// IMPORTANT NOTE:  when registering for the API Key you must agree to:
        ///      I promise to add an attribution link on my website or app to NewsAPI.org.
        /// </summary>
        /// <returns></returns>
        public string GetNewsInfo()
        {
            string newsHeadline = "";
            try
            {
                var topHeadlineNewsUrl = MistyApiInfo.NewsaoiorgBaseUrl +
                    "/top-headlines?" +
                    "country=ca&" +
                    "apiKey=" + MistyApiInfo.NewsapiorgApiKey;
                var response = GetResponse(topHeadlineNewsUrl);
                _misty.SkillLogger.LogVerbose($"Misty.Services: IN GetNewsInfo() -- jsonStrResponse: " + response);
                // {
                //      "status": "ok",
                //      "totalResults": 38,
                //      "articles": [
                //            {
                //                "source": {
                //                "id": null,
                //                "name": "Bbc.com"
                //            },
                //            "author": "https://www.facebook.com/bbcnews",
                //            "title": "Coronavirus: China enacts tighter restrictions in Hubei - BBC News",
                //            "description": "China says its measures are working as cases fall for a third day straight.",
                //            "url": "https://www.bbc.com/news/world-asia-china-51523835",
                //            "urlToImage": "https://ichef.bbci.co.uk/news/1024/branded_news/37A2/production/_110924241_mediaitem110924237.jpg",
                //            "publishedAt": "2020-02-16T14:37:30Z",
                //            "content": "Image copyrightGetty ImagesImage caption\r\n Activities in Wuhan are being further restricted\r\nChina is tightening curbs on movement in Hubei province as it battles the outbreak of new coronavirus. \r\nSixty-million people have been told to stay at home unless th… [+3300 chars]"
                //       ]
                // },
                Newtonsoft.Json.Linq.JObject jsonResp = Newtonsoft.Json.Linq.JObject.Parse(response);
                // Generate a random number of top news articles returned.
                int newsItemIndex = (int)_randomGenerator.Next(0, _maxNewsReturned); // Max value must be _maxNewsReturned + 1 to generate a number from 0 to _maxNewsReturned - 1.
                string newsTitle = (string)jsonResp["articles"][newsItemIndex]["title"];
                _misty.SkillLogger.LogVerbose($"Misty.Services: IN GetNewsInfo() -- newsTitle: {newsTitle}");
                newsHeadline = "From the site NewsAPI.org. A top news headline is ----------" + newsTitle;
            }
            catch (Exception ex)
            {
                _misty.SkillLogger.Log($"Misty.Services: IN GetNewsInfo() => Exception", ex);
            }
            return newsHeadline;
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
