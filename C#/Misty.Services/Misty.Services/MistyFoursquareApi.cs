using System;
using System.Net.Http;
using MistyRobotics.SDK.Messengers;

namespace Misty.Services
{
    /// <summary>
    /// This service calls the Foursqaure Places API.  The Places API offers real-time access to Foursquare’s global database of rich venue data and user 
    /// content to power location-based experiences.
    /// </summary>
    public class MistyFoursquareApi
    {
        private const int numberOfResults = 4;

        public IRobotMessenger _misty;

        public MistyFoursquareApi(IRobotMessenger misty1)
        {
            _misty = misty1;
        }

        /// <summary>
        /// Call the Foursquare Location api to return venues in the area.
        /// https://developer.foursquare.com/docs/api
        /// </summary>
        /// <param name="searchQuery">the type of venue to search for</param>
        /// <returns></returns>
        public string GetVenuesInfo(string searchQuery)
        {
            string venuesResponse = "";
            try {
                // the latitude 
                var latituteLongitude = MistyApiInfo.FoursquareLocationLatLon;
                var venuesUrl = MistyApiInfo.FoursquareBaseUrl + "/venues/explore?client_id=" + MistyApiInfo.FoursquareClientId + "&client_secret=" + MistyApiInfo.FoursquareClientSecret + "&v=20180323&limit=" + numberOfResults + "&ll=" + latituteLongitude + "&query=" + searchQuery;
                _misty.SkillLogger.LogVerbose($"MistyFoursquareApi : IN GetVenueInfo - {venuesUrl}");
                // Call the Foursquares API
                var response = GetResponse(venuesUrl);
                //_misty.SkillLogger.Log($"MistyFoursquareApi : IN GetVenueInfo - response json: {response}");
                Newtonsoft.Json.Linq.JObject jsonResp = Newtonsoft.Json.Linq.JObject.Parse(response);
                // { "meta":{ 
                //      "code":200,
                //      "requestId":"5e49807b006dce001b64ed2f"
                //      },
                //  "response":{
                //      "suggestedFilters":{ 
                //          "header":"Tap to show:",
                //          "filters":[{"name":"Open now","key":"openNow"}]},"suggestedRadius":5873,"headerLocation":"Port Coquitlam","headerFullLocation":"Port Coquitlam","headerLocationGranularity":"city",
                //      "query":"restaurants",
                //      "totalResults":118,
                //      "suggestedBounds":{"ne":{"lat":49.27830514616493,"lng":-122.73593470988092},"sw":{"lat":49.24079193053648,"lng":-122.79941234714852}},
                //      "groups":[{"type":"Recommended Places","name":"recommended",
                //         "items":[
                //         { "reasons":{"count":0,
                //            "items":[{
                //              "summary":"This spot is popular","type":"general","reasonName":"globalInteractionReason"}]},
                //            "venue":{
                //                  "id":"4e5e9f91a809fd79d29b7ed6",
                //                  "name":"Santa Rosa","contact":{},
                //                  "location":{"address":"121-1585 Broadway St",
                //                  "crossStreet":"at Mary Hill Bypass",
                //                  "lat":49.242497076701405,"lng":-122.7645993232727,
                //                  "labeledLatLngs":[{
                //                      "label":"display",
                //                      "lat":49.242497076701405,"lng":-122.7645993232727}],
                //                  "distance":1017,
                //                  "postalCode":"V3C 2M7","cc":"CA",
                //                  "city":"Port Coquitlam",
                //                  "state":"BC",
                //                  "country":"Canada",
                //                  "formattedAddress":[
                //                      "121-1585 Broadway St (at Mary Hill Bypass)","Port Coquitlam BC V3C 2M7","Canada"]},
                //              "categories":[{"id":"4bf58dd8d48988d1c1941735","name":"Mexican Restaurant","pluralName":"Mexican Restaurants","shortName":"Mexican","icon":{"prefix":"https:\/\/ss3.4sqi.net\/img\/categories_v2\/food\/mexican_","suffix":".png"},"primary":true}],"verified":false,"stats":{"tipCount":0,"usersCount":0,"checkinsCount":0,"visitsCount":0},"beenHere":{"count":0,"lastCheckinExpiredAt":0,"marked":false,"unconfirmedCount":0},"photos":{"count":0,"groups":[]},"hereNow":{"count":0,"summary":"Nobody here","groups":[]}},"referralId":"e-0-4e5e9f91a809fd79d29b7ed6-0"},
                //        { "reasons":{"count":0,"items":[{"summary":"This spot is popular","type":"general","reasonName":"globalInteractionReason"}]},"venue":{"id":"4bca6077937ca59322d7a792","name":"Pallas Athena","contact":{},"location":{"address":"101 - 1250 Dominion Ave","crossStreet":"at Ottawa St","lat":49.25836705904778,"lng":-122.74873434597104,"labeledLatLngs":[{"label":"display","lat":49.25836705904778,"lng":-122.74873434597104}],"distance":1502,"postalCode":"V3B 8G7","cc":"CA","city":"Port Coquitlam","state":"BC","country":"Canada","formattedAddress":["101 - 1250 Dominion Ave (at Ottawa St)","Port Coquitlam BC V3B 8G7","Canada"]},"categories":[{"id":"4bf58dd8d48988d10e941735","name":"Greek Restaurant","pluralName":"Greek Restaurants","shortName":"Greek","icon":{"prefix":"https:\/\/ss3.4sqi.net\/img\/categories_v2\/food\/greek_","suffix":".png"},"primary":true}],"verified":false,"stats":{"tipCount":0,"usersCount":0,"checkinsCount":0,"visitsCount":0},"beenHere":{"count":0,"lastCheckinExpiredAt":0,"marked":false,"unconfirmedCount":0},"photos":{"count":0,"groups":[]},"hereNow":{"count":0,"summary":"Nobody here","groups":[]}},"referralId":"e-0-4bca6077937ca59322d7a792-1"},
                //        { "reasons":{"count":0,"items":[{"summary":"This spot is popular","type":"general","reasonName":"globalInteractionReason"}]},"venue":{"id":"4c031cc59a7920a1b288cf79","name":"Dinakis Mediterranean Grill","contact":{},"location":{"address":"101-2020 Oxford Connector","lat":49.26532448553105,"lng":-122.77246835948404,"labeledLatLngs":[{"label":"display","lat":49.26532448553105,"lng":-122.77246835948404}],"distance":1592,"postalCode":"V3C 0A4","cc":"CA","city":"Port Coquitlam","state":"BC","country":"Canada","formattedAddress":["101-2020 Oxford Connector","Port Coquitlam BC V3C 0A4","Canada"]},"categories":[{"id":"4bf58dd8d48988d1c0941735","name":"Mediterranean Restaurant","pluralName":"Mediterranean Restaurants","shortName":"Mediterranean","icon":{"prefix":"https:\/\/ss3.4sqi.net\/img\/categories_v2\/food\/mediterranean_","suffix":".png"},"primary":true}],"verified":false,"stats":{"tipCount":0,"usersCount":0,"checkinsCount":0,"visitsCount":0},"beenHere":{"count":0,"lastCheckinExpiredAt":0,"marked":false,"unconfirmedCount":0},"photos":{"count":0,"groups":[]},"hereNow":{"count":0,"summary":"Nobody here","groups":[]}},"referralId":"e-0-4c031cc59a7920a1b288cf79-2"},
                //        { "reasons":{"count":0,"items":[{"summary":"This spot is popular","type":"general","reasonName":"globalInteractionReason"}]},"venue":{"id":"5010a60be4b0d8233cd74a26","name":"Sushi K Kamizato 神里","contact":{},"location":{"address":"2105 - 2850 Shaughnessy St","crossStreet":"at Lougheed Hwy","lat":49.26530381698295,"lng":-122.7775546404203,"labeledLatLngs":[{"label":"display","lat":49.26530381698295,"lng":-122.7775546404203}],"distance":1726,"postalCode":"V3C 0A4","cc":"CA","neighborhood":"Shaughnessy Station","city":"Port Coquitlam","state":"BC","country":"Canada","formattedAddress":["2105 - 2850 Shaughnessy St (at Lougheed Hwy)","Port Coquitlam BC V3C 0A4","Canada"]}
                string venuesText = "";
                string totalResults = (string)jsonResp["response"]["totalResults"];
                if (totalResults != "0")
                {
                    venuesText = "A few restaurants in the area include ";
                    for (int i = 0; i < numberOfResults; i++)
                    {
                        if (i > 0)
                        {
                            venuesText += ". and ";
                        }
                        venuesText += (string)jsonResp["response"]["groups"][0]["items"][i]["venue"]["name"];
                    }
                }
                else
                {
                    venuesText = "Sorry I could not find any results for " + searchQuery;
                }
                _misty.SkillLogger.LogVerbose($"MistyFoursquareApi : IN GetVenueInfo() - venuesText: {venuesText}");
                return venuesText;
            }
            catch (Exception ex)
            {
                _misty.SkillLogger.Log($"Misty.Services: IN GetVenueInfo() => Exception", ex);
            }
            return venuesResponse;
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
