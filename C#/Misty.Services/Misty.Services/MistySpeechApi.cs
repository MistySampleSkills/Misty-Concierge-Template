using System;
using System.Threading.Tasks;
using MistyRobotics.SDK.Responses;
using MistyRobotics.SDK.Messengers;

namespace Misty.Services
{
    /// <summary>
    /// This service calls the Google Text To Speech API to convert the text value to an audio file and then play the audio file.  If the service 
    /// is busy processing other text then the calling module will wait until this service is available to process the text.
    /// </summary>
    public class MistySpeechApi
    {
        private IRobotMessenger _misty;

        public bool IsBusy { get; private set; }
        public string GoogleAuthToken { get; set; }

        public MistySpeechApi(IRobotMessenger misty1)
        {
            _misty = misty1;
        }

        /// <summary>
        /// Converts the text value to an audio file and then plays the audio file.
        /// </summary>
        /// <param name="textToSpeak">stores the text to be converted to audio</param>
        public async Task<bool> SpeakTheTextAsync(string textToSpeak)
        {
            _misty.ChangeLED(0, 255, 255, null); // light blue

            IsBusy = true; // Set busy flag so others will wait until not busy speaking the text.
            try
            {
                _misty.SkillLogger.LogVerbose($"MistySpeechApi : IN speakTheTextAsync -- textToSpeak = XX{textToSpeak}XX");
                // Parameters to send with request to Google TTS API.
                // For more information see the Google TTS API developer docs:
                // https://cloud.google.com/text-to-speech/docs/reference/rest/v1beta1/text/synthesize
                var arguments = @"{
                    'input': {
                        'text': '" + textToSpeak +
                        @"'},
                    'voice': {
                        'languageCode': 'en-US',
                        'ssmlGender': 'FEMALE'
                    },
                    'audioConfig': {
                        'audioEncoding': 'LINEAR16',
                        'effectsProfileId': [
                            'handset-class-device'
                        ],
                        'pitch': 0,
                        'speakingRate': 0.91
                    }
                }";
                _misty.SkillLogger.LogVerbose("SendExternalRequestAsync - Sending TextToSpeak to Google TTS Api");
                ISendExternalRequestResponse sdata = await _misty.SendExternalRequestAsync("POST", MistyApiInfo.GoogleTextToSpeechUrl, "Bearer", GoogleAuthToken, arguments, false, false, null, "application/json");
                _misty.SkillLogger.LogVerbose("SendExternalRequestAsync - BACK FROM Google TTS Api");

                string jsonStrResponse = sdata.Data.Data.ToString();
                Newtonsoft.Json.Linq.JObject jsonResp = Newtonsoft.Json.Linq.JObject.Parse(jsonStrResponse);

                byte[] audio_data = Convert.FromBase64String(jsonResp["audioContent"].ToString());
                string audio_filename = "tts_response.wav";
                // Save the base64 audio file.
                _misty.SaveAudio(audio_filename, audio_data, true, true, null);
                _misty.Wait(5000);
                _misty.ChangeLED(0, 255, 0, null); // Green
                return true;
            }
            catch (Exception ex)
            {
                _misty.SkillLogger.Log($"MistySpeechApi: IN SpeechResponseCallback: => Exception", ex);
                return true;
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
