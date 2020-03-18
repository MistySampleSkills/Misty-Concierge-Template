using System;
using System.Net.Http;
using MistyRobotics.SDK.Messengers;
using Newtonsoft.Json;

namespace Misty.Services
{
    /// <summary>
    /// This service call a quote api to return an inspiration quote of the data.
    /// </summary>
    public class MistyQuoteApi
    {
        private IRobotMessenger _misty;

        public MistyQuoteApi(IRobotMessenger misty1)
        {
            _misty = misty1;
        }

        /// <summary>
        /// Call the quote api and return the daily inspriational quote.
        /// API Website:  https://quotes.rest/
        /// Notes from vendor: You are welcome to use our API in any of your open source / commercial projects.  For open source community project we require a credit and a link back 
        /// to theysaidso.com in the project page.And all the restrictions mentioned in section 9 under API use still applies to the projects as well.  A credit 
        /// where quotes are displayed is highly appreciated but not strictly required.
        /// </summary>
        /// <returns></returns>
        public string GetQuoteInfo()
        {
            string quoteText = "";
            try
            {
                // Current Quote API Endpoint
                // http://quotes.rest/qod.json?category=inspire
                var quoteUrl = MistyApiInfo.QuoteBaseUrl +
                    "qod.json?category=inspire";
                _misty.SkillLogger.LogVerbose($"Misty.Services: IN GetQuoteInfo() -- quoteUrl: {quoteUrl}");
                var response = GetResponse(quoteUrl);
                Newtonsoft.Json.Linq.JObject jsonResp = Newtonsoft.Json.Linq.JObject.Parse(response);
                _misty.SkillLogger.LogVerbose($"Misty.Services: IN GetQuoteInfo() -- jsonResp: {jsonResp.ToString(Formatting.Indented)}");
                /*
                 {
                    "success": {
                        "total": 1
                    },
                    "contents": {
                        "quotes": [
                            {
                                "quote": "I wanted to achieve something essential in life, something that is not measured by money or position in society. The mountains are not 
                                stadiums where I satisfy my ambitions to achieve. They are my cathedrals, the houses of my religion. In the mountains I attempt to understand my 
                                life. They are the way I practice my religion. In the mountains I celebrate creation, on each journey I am reborn.",
                                "length": "394",
                                "author": "Anatoli Boukreev",
                                "tags": {
                                    "0": "achievement",
                                    "1": "ambition",
                                    "2": "inspire",
                                    "3": "journey",
                                    "4": "mountaineering",
                                    "6": "tso-life"
                                },
                                "category": "inspire",
                                "language": "en",
                                "date": "2020-02-27",
                                "permalink": "https://theysaidso.com/quote/anatoli-boukreev-i-wanted-to-achieve-something-essential-in-life-something-that",
                                "id": "C_3w5S9DpW74kC_1VBAINQeF",
                                "background": "https://theysaidso.com/img/qod/qod-inspire.jpg",
                                "title": "Inspiring Quote of the day"
                            }
                        ]
                    },
                    "baseurl": "https://theysaidso.com",
                    "copyright": {
                        "year": 2022,
                        "url": "https://theysaidso.com"
                    }
                 }
                */
                // This API requires credit to be given to the "They Said So" web site -- see https://quotes.rest/
                quoteText = $"On TheySaidSo.com, the inspiration quote of the day is.  {(string)jsonResp["contents"]["quotes"][0]["quote"]}";
                _misty.SkillLogger.LogVerbose($"Misty.Services: IN GetQuoteInfo() -- quoteText: {quoteText}");
            }
            catch (Exception ex)
            {
                _misty.SkillLogger.Log($"Misty.Services: IN GetNewsInfo() => Exception", ex);
            }
            return quoteText;
        }

        private string GetResponse(string url)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
                return httpClient.GetStringAsync(new Uri(url)).Result;
            }
        }
    }
}
