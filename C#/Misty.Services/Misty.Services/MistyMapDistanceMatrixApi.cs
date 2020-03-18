using System;
using System.Net.Http;
using MistyRobotics.SDK.Messengers;

namespace Misty.Services
{
    /// <summary>
    /// A Service which calls the Google Map - Distance Matrix API.  The Distance Matrix API is a service that provides travel distance and time for a matrix 
    /// of origins and destinations, based on the recommended route between start and end points.
    /// </summary>
    public class MistyMapDistanceMatrixApi
    {
        public IRobotMessenger _misty;

        public MistyMapDistanceMatrixApi(IRobotMessenger misty1)
        {
            _misty = misty1;
        }

        /// <summary>
        /// Call the Google Distance Matrix API to pass the location information.
        /// ================== Google Map - Distance Matrix API ================
        /// https://developers.google.com/maps/documentation/distance-matrix/start
        /// </summary>
        /// <param name="data"></param>
        public string CallGoogleDistanceMatrixAsync(string origin, string destiniation)
        {
            string distanceDurationText = "";
            try
            {
                _misty.SkillLogger.LogVerbose($"Misty.Services : IN CallGoogleDistanceMatrixAsync");
                // https://maps.googleapis.com/maps/api/distancematrix/json?units=imperial&origins=PORT%20COQUITLAM,BC&destinations=Vancouver,BC&key=YOUR_API_KEY
                string googleDistanceMatrixUrl = MistyApiInfo.GoogleDistanceMatrixUrl + "?origins=" + origin + "&destinations=" + destiniation + "&key=" + MistyApiInfo.GoogleDistanceMatrixApiKey;
                _misty.SkillLogger.LogVerbose($"Misty.Services : IN CallGoogleDistanceMatrixAsync - Before calling API" + googleDistanceMatrixUrl);
                // call the Google DistanceMatrix API
                var response = GetResponse(googleDistanceMatrixUrl);
                _misty.SkillLogger.LogVerbose($"Misty.Services: IN CallGoogleDistanceMatrixAsync -- jsonStrResponse: " + response);
                //{
                //   "destination_addresses" : [ "Kelowna, BC, Canada" ],
                //   "origin_addresses" : [ "Port Coquitlam, BC, Canada" ],
                //   "rows" : [
                //      {
                //         "elements" : [
                //            {
                //               "distance" : {
                //                  "text" : "230 mi",
                //                  "value" : 370796
                //               },
                //               "duration" : {
                //                  "text" : "3 hours 45 mins",
                //                  "value" : 13518
                //               },
                //               "status" : "OK"
                //            }
                //         ]
                //      } 
                //   ],
                //   "status" : "OK"
                //} 
                Newtonsoft.Json.Linq.JObject jsonResp = Newtonsoft.Json.Linq.JObject.Parse(response);
                string originText = (string)jsonResp["origin_addresses"][0];
                string destinationText = (string)jsonResp["destination_addresses"][0];
                string distanceText = (string)jsonResp["rows"][0]["elements"][0]["distance"]["text"];
                string durationText = (string)jsonResp["rows"][0]["elements"][0]["duration"]["text"];
                distanceDurationText = "From " + originText + " to " + destinationText + " the distance is " + distanceText + " and the duration is " + durationText;
            }
            catch (Exception ex)
            {
                _misty.SkillLogger.Log($"Misty.Services: IN CallGoogleDistanceMatrixAsync: => Exception", ex);
            }
            return distanceDurationText;
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
