using System;

namespace Misty.Services
{
    /// <summary>
    /// Store the base urls and api keys for the external apis called by the Concierge application.
    /// </summary>
    public class MistyApiInfo
    {
        public const string CloudFunctionAuthTokenUrl = "https://xxxx.cloudfunctions.net/authToken";
        public const string CurrentLocation = "city state province";

        public const string FoursquareBaseUrl = "https://api.foursquare.com/v2";
        public const string FoursquareClientId = "xxxxx";
        public const string FoursquareClientSecret = "xxxx";
        // the latitude and longitude for the location to search around to find a venue.
        public const string FoursquareLocationLatLon = "99.251546,-999.7665771";

        public const string GoogleCloudProjectId = "misty-concierge-xxxx";

        public const string GoogleDistanceMatrixUrl = "https://maps.googleapis.com/maps/api/distancematrix/json";
        public const string GoogleDistanceMatrixApiKey = "xxxxx";

        public const string GoogleTextToSpeechUrl = "https://texttospeech.googleapis.com/v1beta1/text:synthesize";

        public const string GoogleDialogflowUrl = "https://dialogflow.googleapis.com/v2/projects/";

        public const string JokeBaseUrl = "https://icanhazdadjoke.com/";

        public const string NewsaoiorgBaseUrl = "https://newsapi.org/v2";
        public const string NewsapiorgApiKey = "xxxxx";

        public const string QuoteBaseUrl = "https://quotes.rest/";

        public const string StockBaseUrl = "https://www.alphavantage.co";
        public const string StockApiKey = "xxxxx";

        public const string TwilioAccountSid = "xxxxx";
        public const string TwilioAuthToken = "xxxxx";
        public const string TwilioPhoneNumber = "+19998887777";

        public const string WeatherBaseUrl = "http://api.weatherstack.com";
        public const string WeatherApiKey = "xxxxx";

        public const string WemoDeviceIpAddress = "http://10.0.0.999";
    }
}
