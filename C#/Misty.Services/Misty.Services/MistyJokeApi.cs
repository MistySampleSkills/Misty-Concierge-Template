using System;
using System.Net.Http;
using MistyRobotics.SDK.Messengers;
using Newtonsoft.Json;

namespace Misty.Services
{
    /// <summary>
    /// This service calls a Joke API to return a random joke to Misty.
    /// </summary>
    public class MistyJokeApi
    {
        private IRobotMessenger _misty;

        public MistyJokeApi(IRobotMessenger misty1)
        {
            _misty = misty1;
        }

        // API Website:  https://icanhazdadjoke.com/
        public string GetJokeInfo()
        {
            string jokeText = "";
            try
            {
                _misty.SkillLogger.LogVerbose($"Misty.Services: IN GetJokeInfo() -- MistyApiInfo.JokeBaseUrl: {MistyApiInfo.JokeBaseUrl}");
                var response = GetResponse(MistyApiInfo.JokeBaseUrl);
                Newtonsoft.Json.Linq.JObject jsonResp = Newtonsoft.Json.Linq.JObject.Parse(response);
                _misty.SkillLogger.LogVerbose($"Misty.Services: IN GetJokeInfo() -- jsonResp: {jsonResp.ToString(Formatting.Indented)}");
                //{
                //    "id": "R7UfaahVfFd",
                //    "joke": "My dog used to chase people on a bike a lot. It got so bad I had to take his bike away.",
                //    "status": 200
                //}
                jokeText = (string)jsonResp["joke"];
                _misty.SkillLogger.LogVerbose($"Misty.Services: IN GetJokeInfo() -- jokeText: {jokeText}");
            }
            catch (Exception ex)
            {
                _misty.SkillLogger.Log($"Misty.Services: IN GetJokeInfo() => Exception", ex);
            }
            return jokeText;
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
