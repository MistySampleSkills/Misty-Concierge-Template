using System;
using System.Net.Http;
using MistyRobotics.SDK.Messengers;
using Newtonsoft.Json;

// A template file for creating a new Service to include in the project.
namespace Misty.Services
{
    public class MistyServiceApi
    {
        private IRobotMessenger _misty;

        public MistyServiceApi(IRobotMessenger misty1)
        {
            _misty = misty1;
        }

        /// <summary>
        /// Calls the XXX api to return xxx.
        /// API Website: 
        /// </summary>
        /// <param name="param1"></param>
        /// <returns></returns>
        public string GetXXXInfo(string param1)
        {
            string textInfo = "";
            try
            {
                var apiUrl = "";
                var response = GetResponse(apiUrl);
                Newtonsoft.Json.Linq.JObject jsonResp = Newtonsoft.Json.Linq.JObject.Parse(response);
                _misty.SkillLogger.LogVerbose($"Misty.Services: IN GetXXXInfo() -- jsonResp: {jsonResp.ToString(Formatting.Indented)}");
                textInfo = jsonResp.ToString();
            }
            catch (Exception ex)
            {
                _misty.SkillLogger.Log($"Misty.Services: IN GetXXXInfo() => Exception", ex);
            }
            return textInfo;
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
