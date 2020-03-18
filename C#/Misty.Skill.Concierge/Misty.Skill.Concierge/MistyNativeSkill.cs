using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using MistyRobotics.Common.Data;
using MistyRobotics.SDK.Events;
using MistyRobotics.SDK.Responses;
using MistyRobotics.Common.Types;
using MistyRobotics.SDK;
using MistyRobotics.SDK.Messengers;
using Misty.Services;
using Newtonsoft.Json;

namespace Misty.Skill.Concierge
{
	internal class MistyNativeSkill : IMistySkill
	{
		/// <summary>
		/// Hold the misty robot interface
		/// </summary>
		private IRobotMessenger _misty;
        /// <summary>
        /// Timer object to perform callbacks at a regular interval to get the authorization token
        /// </summary>
        private Timer _getTokenTimer;
        /// <summary>
        /// Timer object to perform callbacks at a regular interval to play information about Misty and the Concierge application.
        /// </summary>
        private Timer _mistyInfoTimer;
        /// <summary>
        /// Timer object to get find face information.
        /// </summary>
        private Timer _getFindFaceTimer;
        /// <summary>
        /// Timer object to have Misty look side to side at a regular interval.
        /// </summary>
        private Timer _lookSideToSideTimer;
        /// <summary>
        /// Timer object to have Misty randomly move her arms on a regular interval.
        /// </summary>
        private Timer _armsRandomTimer;
        /// <summary>
        /// Random number generator
        /// </summary>
        private Random _randomGenerator = new Random();
        // Store the google authorication token to call the Google Dialogflow and TTS apis.
        private String _googleAuthToken = "";
        // Define the instance of each class used by the application.
        private MistySpeechApi _speechApi;
        private MistyFoursquareApi _foursquareApi;
        private MistyMapDistanceMatrixApi _mapDistanceMatrixApi;
        private MistyNewsApi _newsApi;
        private MistyTwilioApi _twilioApi;
        private MistyStockApi _stockApi;
        private MistyWeatherApi _weatherApi;
        private MistySmartDevicesApi _smartDeviceApi;
        private MistyJokeApi _jokeApi;
        private MistyQuoteApi _quoteApi;
        private MistyInfo _mistyInfo;
        private MistyTellTime _tellTime;
        // Global variable to check whether Misty is searching for a face or looking at a face
        private bool _findFace;
        // Global variable to store current pitch and yaw position of the head
        private double _headYaw;
        private double _headPitch;
        private double _yawRight = -90.0;
        private double _yawLeft = 90.0;
        private double _pitchDown = 90.0;
        private double _pitchUp = -90.0;
        // Audio localization variables.
        private double _speechSourceAngle;
        public bool _recordingAudio;
        public bool _playingAudio;

        /// <summary>
        /// Skill details for the robot
        /// 
        /// There are other parameters you can set if you want:
        ///   Description - a description of your skill
        ///   TimeoutInSeconds - timeout of skill in seconds
        ///   StartupRules - a list of options to indicate if a skill should start immediately upon startup
        ///   BroadcastMode - different modes can be set to share different levels of information from the robot using the 'SkillData' websocket
        ///   AllowedCleanupTimeInMs - How long to wait after calling OnCancel before denying messages from the skill and performing final cleanup  
        /// </summary>
        public INativeRobotSkill Skill { get; private set; } = new NativeRobotSkill("Misty.Skill.Concierge", "3a8d8237-8bea-44ff-9313-4f3d2a2b2e63")
        {
            TimeoutInSeconds = 60 * 30, //runs for 30 minutes or until the skill is cancelled
            StartupRules = { NativeStartupRule.Startup, NativeStartupRule.Manual } // Run skill on startup of robot.
        };

        /// <summary>
        ///	This method is called by the wrapper to set your robot interface
        ///	You need to save this off in the local variable commented on above as you are going use it to call the robot
        /// </summary>
        /// <param name="robotInterface"></param>
        public void LoadRobotConnection(IRobotMessenger robotInterface)
		{
			_misty = robotInterface;
		}

        /// <summary>
        /// This event handler is called when the robot/user sends a start message
        /// The parameters can be set in the Skill Runner (or as json) and used in the skill if desired
        /// </summary>
        /// <param name="parameters"></param>
        public void OnStart(object sender, IDictionary<string, object> parameters)
        {
            try
            {
                InstantiateApis();
                //Set LED color
                _misty.ChangeLED(255, 255, 128, null); // yellow
                robotHomePosition();
                // Get the Access Token
                _getTokenTimer = new Timer(TokenTimerCallback, null, 1000, 10000);
                // Speak the introductory message for Misty.
                IntroMessageAndStartKeyPhrase();
                // Register a CapTouch event to touch the front of Misty to turn OFF AV Streaming.
                RegisterCapTouchIsContactEvent();
                // Follow face / look at person components
                _getFindFaceTimer = new Timer(FindFaceCallback, null, 1000, 6000);
                _lookSideToSideTimer = new Timer(LookSidetoSideCallback, null, 1000, 8000);
                _armsRandomTimer = new Timer(ArmsRandomCallback, null, 2000, 10000);
                RegisterYaw();
                RegisterPitch();
                RegisterAudioLocalization();
                _misty.ChangeLED(0,255, 0, null); // green
            }
            catch (Exception ex)
            {
                _misty.SkillLogger.Log($"MistyConcierge : OnStart: => Exception", ex);
            }
        }

        /// <summary>
        /// Register the CapTouch Event for only IsContacted
        /// </summary>
        private void RegisterCapTouchIsContactEvent()
        {
            CapTouchValidation item = new CapTouchValidation
            {
                Name = CapTouchFilter.IsContacted,
                Comparison = ComparisonOperator.Equal,
                ComparisonValue = CapTouchPosition.Front
            };
            IList<CapTouchValidation> ctv = new List<CapTouchValidation>();
            ctv.Add(item);
            _misty.RegisterCapTouchEvent(CapTouchCallback, 0, true,ctv, null, null);
        }

        private void InstantiateApis()
        {
            _speechApi = new MistySpeechApi(_misty);
            _foursquareApi = new MistyFoursquareApi(_misty);
            _mapDistanceMatrixApi = new MistyMapDistanceMatrixApi(_misty);
            _newsApi = new MistyNewsApi(_misty);
            _stockApi = new MistyStockApi(_misty);
            _weatherApi = new MistyWeatherApi(_misty);
            _twilioApi = new MistyTwilioApi(_misty, MistyApiInfo.TwilioAccountSid, MistyApiInfo.TwilioAuthToken);
            _smartDeviceApi = new MistySmartDevicesApi();
            _jokeApi = new MistyJokeApi(_misty);
            _quoteApi = new MistyQuoteApi(_misty);
            _mistyInfo = new MistyInfo(_misty);
            // Start the Tell Time / Clock service.
            _tellTime = new MistyTellTime(_misty, _speechApi);
        }

        #region *** Follow face / Look at person **

        // ====================================================================
        // ================= FOLLOW FACE / LOOK AT PERSON  ====================
        // ====================================================================

        private void robotHomePosition()
        {
            _misty.SkillLogger.LogVerbose($"MistyConcierge: IN robotHomePosition -- MoveHead and MoveArms");
            _misty.MoveHead(0, 0, 0, 60, AngularUnit.Degrees, null);
            _misty.DisplayImage("e_DefaultContent.jpg", null, false, null);
            _misty.MoveArms(90, 90, 100, 60, null, AngularUnit.Degrees, null);
        }

        private void RegisterYaw()
        {
            ActuatorPositionValidation item = new ActuatorPositionValidation
            {
                Name = ActuatorPositionFilter.SensorName,
                Comparison = ComparisonOperator.Equal,
                ComparisonValue = ActuatorPosition.HeadYaw
            };
            IList<ActuatorPositionValidation> apv = new List<ActuatorPositionValidation>();
            apv.Add(item);
            _misty.RegisterActuatorEvent(headYawCallback, 100, true, apv, "headYawCallback", null);
        }

        private void RegisterPitch()
        {
            ActuatorPositionValidation item = new ActuatorPositionValidation
            {
                Name = ActuatorPositionFilter.SensorName,
                Comparison = ComparisonOperator.Equal,
                ComparisonValue = ActuatorPosition.HeadPitch
            };
            IList<ActuatorPositionValidation> apv = new List<ActuatorPositionValidation>();
            apv.Add(item);
            _misty.RegisterActuatorEvent(headPitchCallback, 100, true, apv, "headPitchCallback", null);
        }

        private void headYawCallback(IActuatorEvent data)
        {
            _headYaw = data.ActuatorValue;
        }

        private void headPitchCallback(IActuatorEvent data)
        {
            _headPitch = data.ActuatorValue;
        }

        // ============== Face Recognition Data and Face Follow ===============
        private void FindFaceCallback(object state)
        {
            _findFace = true;
            _misty.DisplayImage("e_DefaultContent.jpg", null, false, null);
        }

        private void LookSidetoSideCallback(object state)
        {
            if (_findFace && !_recordingAudio && !_playingAudio)
            {
                _misty.SkillLogger.LogVerbose($"MistyConcierge: IN LookSidetoSideCallback - _headYaw: {_headYaw} - moving head");
                if (_headYaw > 0) _misty.MoveHead((int)_randomGenerator.Next(-20, 1), 0, -40, 60, AngularUnit.Degrees, null);
                else _misty.MoveHead((int)_randomGenerator.Next(-20, 1), 0, 40, 60, AngularUnit.Degrees, null);
            }
        }

        // Moves arms randomly every x seconds
        private void ArmsRandomCallback(object state)
        {
            if (!_recordingAudio && !_playingAudio)
            {
                _misty.MoveArm((int)_randomGenerator.Next(-29, 81), RobotArm.Left, 60, null, AngularUnit.Degrees, null);
                _misty.MoveArm((int)_randomGenerator.Next(-29, 81), RobotArm.Right, 60, null, AngularUnit.Degrees, null);
            }
        }
        #endregion

        #region *** Calibrate - Head and Arms ***

        // 
        /// <summary>
        /// Misty moves her head as far as she can to the right, left, down, and up, and records the maximum reachable angles.
        /// This calibration is NOT mandatory every time. Run this once, and  look at the web console in the Skill Runner page 
        /// to get the min and max range for pitch and yaw movements for your robot. Update the four global variables below 
        /// with these limits.
        /// </summary>
        // *** calibrate() IS CURRENTLY NOT CALLED ***
        private void calibrate()
        {
            moveHeadAndRecordPosition(0, 0, -90, "yawRight", "headYaw");
            moveHeadAndRecordPosition(0, 0, 90, "yawLeft", "headYaw");
            moveHeadAndRecordPosition(90, 0, 0, "pitchDown", "headPitch");
            moveHeadAndRecordPosition(-90, 0, 0, "pitchUp", "headPitch");

            _misty.SkillLogger.LogInfo($"MistyConcierge : IN calibrate() - CALIBRATION COMPLETE");
            _misty.MoveHead(0, 0, 0, 0, AngularUnit.Degrees, null);
        }

        // *** moveHeadAndRecordPosition() IS CURRENTLY NOT CALLED ***
        private void moveHeadAndRecordPosition(double pitch, double roll, double yaw, string outputsetto, string inputfrom)
        {
            _misty.MoveHead(pitch, roll, yaw, 50, AngularUnit.Degrees, null);
            _misty.Wait(4000);
            if ((inputfrom == "headYaw") && (outputsetto == "yawRight"))
            {
                _yawLeft = _headYaw;
                _misty.SkillLogger.LogVerbose($"MistyConcierge : IN moveHeadAndRecordPosition() - outputsetto/_yawLeft recorded: {_yawLeft}");
            }
            if ((inputfrom == "headYaw") && (outputsetto == "yawLeft"))
            {
                _yawRight = _headYaw;
                _misty.SkillLogger.LogVerbose($"MistyConcierge : IN moveHeadAndRecordPosition() - outputsetto/_yawRight recorded: {_yawRight}");
            }
            if ((inputfrom == "headPitch") && (outputsetto == "yawRight"))
            {
                _pitchDown = _headPitch;
                _misty.SkillLogger.LogVerbose($"MistyConcierge : IN moveHeadAndRecordPosition() - outputsetto/_pitchDown recorded: {_pitchDown}");
            }
            if ((inputfrom == "headPitch") && (outputsetto == "pitchUp"))
            {
                _pitchUp = _headPitch;
                _misty.SkillLogger.LogVerbose($"MistyConcierge : IN moveHeadAndRecordPosition() - outputsetto/_pitchUp recorded: {_pitchUp}");
            }
        }
        #endregion

        #region *** Audio Localization *** 

        /// <summary>
        /// if misty is not already looking at the person's face, we use audio localization data to make misty turn and look 
        /// at the person while speaking out the response
        /// </summary>
        private void RegisterAudioLocalization()
        {
            _misty.SkillLogger.LogInfo($"MistyConcierge: IN RegisterAudioLocalization ");
            _speechSourceAngle = 0.0;

            _misty.RegisterSourceTrackDataEvent(100, true, null, null);
            _misty.SourceTrackDataMessageEventReceived += ProcessSourceTrackDataMessageEvent;
        }

        /// <summary>
        /// Start recording audio to get audio localization data. While recording audio, we pause Misty looking side to side so
        /// that Misty can accurately localize to the speaker's voice.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="sourceTrackDataMessageEvent"></param>
        private void ProcessSourceTrackDataMessageEvent(object sender, ISourceTrackDataMessageEvent sourceTrackDataMessageEvent)
        {
            _misty.SkillLogger.LogVerbose($"MistyConcierge : IN SoundCallBack() - _recordingAudio: {_recordingAudio} -- DegreeOfArrivalSpeech: {sourceTrackDataMessageEvent.DegreeOfArrivalSpeech}");
            if (!_recordingAudio) _recordingAudio = true;
            // We get the location of speech audio in 0-360 degrees w/r/t Misty's head as the local frame. We use the function
            // toRobotFrame() to map this location to the robot's local frame.
            double vector = toRobotFrame((double)sourceTrackDataMessageEvent.DegreeOfArrivalSpeech);
            _speechSourceAngle = vector;
        }

        /// <summary>
        /// Converts 0 to 360 range (CCW) to 0 to +-180 to 0 
        /// </summary>
        /// <param name="soundIn"></param>
        /// <returns></returns>
        private double toRobotFrame(double soundIn)
        {
            if (soundIn > 180) soundIn -= 360;

            // Map audio localization data to robot's frame of reference 
            var actuateTo = _headYaw + soundIn;
            actuateTo = actuateTo > 90 ? 90 : actuateTo < -90 ? -90 : actuateTo;

            _misty.SkillLogger.LogVerbose($"MistyConcierge : IN toRobotFrame() - _headYaw: {_headYaw} -- soundIn: {soundIn} -- actuateTo: {actuateTo}");

            return actuateTo;
        }
        #endregion

        #region *** Concierge ***
        // ====================================================================
        // =========================== CONCIERGE  =============================
        // ====================================================================

        private async void IntroMessageAndStartKeyPhrase()
        {
            string introMessage = "Hello my name is Misty.  I can help you -- just provide me with a command.";
            //introMessage += " --- I can tell you the:  current time, a joke, an inspirational quote, driving distance and time to a location, a top news headline, a current stock price, " +
            //    "find near by restaurants, personal greetings, send text messages, and turn on and off a light as well as turning on my Audio Video streaming.";
            _misty.SkillLogger.Log($"MistyConcierge : IN IntroMessage() - BEFORE speakTheText()");
            _misty.Wait(3000);
            _speechApi.GoogleAuthToken = _googleAuthToken;
            bool done = await _speechApi.SpeakTheTextAsync(introMessage);
            _playingAudio = false;
            _misty.RegisterKeyPhraseRecognizedEvent(KeyPhraseRecognizedCallback, 0, true, null, null);
            _misty.SkillLogger.Log($"MistyConcierge: IN IntroMessageAndStartKeyPhrase -- RegisterKeyPhraseRecognizedEvent and StartKeyPhraseRecognition");
            _misty.RegisterVoiceRecordEvent(OnVoiceRecordReceived, 10, false, "VoiceRecordEvent", null);
            _misty.StartKeyPhraseRecognition(true, true, 7000, null, null);
            _misty.SkillLogger.Log($"MistyConcierge : IN IntroMessage() - After StartKeyPhraseRecognition()");
        }

        /// <summary>
        /// Calls the the Google Cloud function to get an authorization token.
        /// </summary>
        /// <param name="state"></param>
        private void TokenTimerCallback(object state)
        {
            _misty.SendExternalRequest("POST", MistyApiInfo.CloudFunctionAuthTokenUrl, null, null, null, false, false, null, "application/json", AuthTokenCallback);
        }

        /// <summary>
        /// A callback method to store the google authorization token.
        /// </summary>
        /// <param name="data"></param>
        private void AuthTokenCallback(IRobotCommandResponse data)
        {
            SendExternalRequestResponse sdata = (SendExternalRequestResponse)data;

            string jsonStrResponse = sdata.Data.Data.ToString();
            Newtonsoft.Json.Linq.JObject jsonResp = Newtonsoft.Json.Linq.JObject.Parse(jsonStrResponse);
            _googleAuthToken = (string)jsonResp["authToken"];
            _misty.SkillLogger.LogVerbose($"MistyConcierge :  _googleAuthToken = {_googleAuthToken}");
        }

        /// <summary>
        /// Get the audio file and move Misty's head if in find face mode.
        /// </summary>
        /// <param name="voiceEvent"></param>
        private void OnVoiceRecordReceived(IVoiceRecordEvent voiceEvent)
        {
            _misty.SkillLogger.LogVerbose($"MistyConcierge :  OnVoiceRecordReceived -- voiceEvent.ErrorCode: {voiceEvent.ErrorCode}");
            _recordingAudio = true;
            if (voiceEvent.ErrorCode == 0)
            {
                string fileName = voiceEvent.Filename;
                _misty.GetAudio(GetAudioCallback, fileName, true);
                if (_findFace)
                {
                    _misty.MoveHead(-20, 20, 0, 95, AngularUnit.Degrees, null);
                }
            }
            _recordingAudio = false;
        }

        /// <summary>
        /// Callback called when the voice recognition "Hey Misty" event is triggered
        /// </summary>
        /// <param name="voiceEvent"></param>
        private void KeyPhraseRecognizedCallback(IKeyPhraseRecognizedEvent voiceEvent)
        {
            _misty.DisplayImage("e_Admiration.jpg", null, false, null);
        }

        /// <summary>
        /// Callback called getting the audio file from robot and send the audio file to the Google dialogflow.
        /// </summary>
        /// <param name="data"></param>
        private void GetAudioCallback(IGetAudioResponse commandResponse)
        {
            try
            {
                _recordingAudio = false;
                _misty.SkillLogger.LogVerbose($"MistyConcierge: IN GetAudioCallback: CALLING CallDialogflow()");
                // Parameters to send with request to Dialogflow agent.
                string data = commandResponse.Data.Base64.ToString();
                // Send the audio command to Dialogflow
                CallDialogflow(data);
                _misty.RegisterKeyPhraseRecognizedEvent(KeyPhraseRecognizedCallback, 0, true, null, null);
                _misty.RegisterVoiceRecordEvent(OnVoiceRecordReceived, 10, false, "VoiceRecordEvent", null);
                _misty.StartKeyPhraseRecognition(true, true, 7000, null, null);
                _misty.DisplayImage("e_DefaultContent.jpg", null, false, null);
                _misty.ChangeLED(0, 255, 0, null); // Green
            }
            catch (Exception ex)
            {
                _misty.SkillLogger.Log($"MistyConcierge: IN GetAudioCallback: => Exception", ex);
            }
        }

        /// <summary>
        /// Callback called when a face is detected or recognized 
        /// </summary>
        /// <param name="faceRecEvent"></param>
        private async void FaceRecCallback(IFaceRecognitionEvent faceRecEvent)
        {
            _misty.SkillLogger.LogVerbose($"MistyConcierge : FaceRecCallback called");

            // FOLLOW FACE / LOOK AT FACE logic
            if (_findFace)
            {
                _findFace = false;
            }
            double bearing = faceRecEvent.Bearing;      // -13 right and +13 left
            double elevation = faceRecEvent.Elevation;  // -13 up and +13 down
            _misty.SkillLogger.LogVerbose($"MistyConcierge : FaceRecCallback called -- bearing: {bearing} -- elevation: {elevation}");
            if (bearing != 0 && elevation != 0)
            {
                _misty.MoveHead(_headPitch + ((_pitchDown - _pitchUp) / 33) * elevation, 0, _headYaw + ((_yawLeft - _yawRight) / 66) * bearing, 7 / Math.Abs(bearing), AngularUnit.Degrees, null);
            }
            else if (bearing != 0)
            {
                _misty.MoveHead(0, 0, _headYaw + ((_yawLeft - _yawRight) / 66) * bearing, 7 / Math.Abs(bearing), AngularUnit.Degrees, null);
            }
            else
            {
                _misty.MoveHead(_headPitch + ((_pitchDown - _pitchUp) / 33) * elevation, 0, 0, 5 / Math.Abs(elevation), AngularUnit.Degrees, null);
            }
            _getFindFaceTimer = new Timer(FindFaceCallback, null, 6000, 6000);

            string personFaceRecognized = faceRecEvent.Label.ToString().Replace(" ", "_").ToUpper();
            _misty.SkillLogger.LogVerbose($"MistyConcierge :  FaceRecCallback - personFaceRecognized: {personFaceRecognized}");
            // Get the greeting text based on the person recognized.
            MistyFriends.FriendInfo friend = MistyFriends.LookupFriendInfo(personFaceRecognized);
            // Return if the person is unknown or was found in the list because the person has already been greeted.
            // OR if the greetings index exceeds the index into the greeting list then return because no more greetings to play for this person.
            _misty.SkillLogger.LogVerbose($"MistyConcierge :  FaceRecCallback - person_face_recognized -- : {personFaceRecognized} -- Friend.Greetings.count {friend.Greetings.Count}");
            if ((personFaceRecognized == "UNKNOWN_PERSON") || (friend == null) || (friend.Greetings.Count == 0))
            {
                _misty.RegisterFaceRecognitionEvent(FaceRecCallback, 0, false, null, null, null);
                return;
            }
            _misty.ChangeLED(0, 128, 255, null); // light blue
            _misty.DisplayImage("e_Love.jpg", null, false, null);
            string greetingText = friend.Greetings[0]; // Get the 1st / top remaining greeting.
            _misty.SkillLogger.LogVerbose($"MistyConcierge :  FaceRecCallback - greetingText: {greetingText}");

            bool done = false;
            do
            {
                // If not busy processing other text then:
                if (!_speechApi.IsBusy)
                {
                    done = await _speechApi.SpeakTheTextAsync(greetingText);
                } else { _misty.Wait(1000); }
            } while (!done);

            _misty.Wait(5000);  // Wait so it can finish playing the audio before playing the next audio.
            _playingAudio = false;
            // Remove the greeting spoken for this specific person.
            friend.Greetings.RemoveAt(0);
            _misty.DisplayImage("e_DefaultContent.jpg", null, false, null);
            _misty.RegisterFaceRecognitionEvent(FaceRecCallback, 0, false, null, null, null);
            _misty.ChangeLED(0, 255, 0, null); // green
        }

        /// <summary>
        /// Call the Google Dialogflow api to process the user request.
        /// </summary>
        /// <param name="data"></param>
        private void CallDialogflow(string data)
        {
            try
            {
                _misty.ChangeLED(0, 255, 255, null); // light blue
                // Parameters to send with request to Dialogflow agent.
                // For more information see the Dialogflow developer documentation:
                // https://cloud.google.com/dialogflow/docs/reference/rest/v2/projects.agent.sessions/detectIntent
                var arguments = @"{
                    'queryInput': {
                        'audioConfig': {
                            'audioEncoding': 'AUDIO_ENCODING_LINEAR_16',
                            'languageCode': 'en-US'
                        }
                    },
                    'inputAudio': '" + data + @"',
                    'outputAudioConfig': {
                        'audioEncoding': 'OUTPUT_AUDIO_ENCODING_LINEAR_16',
                        'synthesizeSpeechConfig': {
                            'speakingRate': 0.95,
                            'pitch': 0,
                            'volumeGainDb': 0,
                            'effectsProfileId': ['handset-class-device'],
                            'voice': {
                                'name': 'en-US-Wavenet-F'
                            }
                        }
                    }
                }";
                string GoogleDialogflowUrl = MistyApiInfo.GoogleDialogflowUrl + MistyApiInfo.GoogleCloudProjectId + "/agent/sessions/123456:detectIntent";
                _misty.SendExternalRequest("POST", GoogleDialogflowUrl, "Bearer", _googleAuthToken, arguments, false, false, null, "application/json", DialogflowResponseCallback);
            }
            catch (Exception ex)
            {
                _misty.SkillLogger.Log($"MistyConcierge: IN GetAudioCallback: => Exception", ex);
            }
        }
        /// <summary>
        /// Callback called when the response from the dialogflow estion audio file is sent and an answer audio file is returned.
        /// ================= Handle Dialogflow Response =======================
        /// We get the extracted data object from the voice recording and define Misty's response to different intents. In some cases, she speaks the
        /// response text out loud using Google's text-to-speech API.
        /// </summary>
        /// <param name="data"></param>
        private async void DialogflowResponseCallback(IRobotCommandResponse data)
        {
            try
            {
                _misty.SkillLogger.LogVerbose($"MistyConcierge: IN DialogflowResponseCallback");
                SendExternalRequestResponse sdata = (SendExternalRequestResponse)data;
                
                string jsonStrResponse = sdata.Data.Data.ToString();
                Newtonsoft.Json.Linq.JObject jsonResp = Newtonsoft.Json.Linq.JObject.Parse(jsonStrResponse);
                // https://cloud.google.com/dialogflow/docs/detect-intent-tts
                // https://cloud.google.com/dialogflow/docs/reference/rest/v2/projects.agent.sessions/detectIntent
                // https://cloud.google.com/dialogflow/docs/reference/rpc/google.cloud.dialogflow.v2#google.cloud.dialogflow.v2.DetectIntentRequest
                //
                //  "responseId": "e79b06ff-e32c-43b1-8a9b-a2d7e21554fa-2e39b744",
                //  "queryResult": {
                //                    "queryText": "how far to Vancouver",
                //    "speechRecognitionConfidence": 0.9738625,
                //    "parameters": {
                //        "to-location": {
                //             "city": "Vancouver",
                //        "subadmin-area": "",
                //        "zip-code": "",
                //        "street-address": "",
                //        "country": "",
                //        "business-name": "",
                //        "shortcut": "",
                //        "admin-area": "",
                //        "island": ""
                //                        }
                //                    },
                //    "allRequiredParamsPresent": true,
                //    "fulfillmentText": "the distance to  Vancouver        is",
                //    "fulfillmentMessages": [
                //      {
                //        "text": {
                //          "text": [
                //            "the distance to  Vancouver        is"
                //          ]
                //    }
                //}
                //    ],
                //    "intent": {
                //      "name": "projects/misty-concierge-ewnrgd/agent/intents/f05f101b-f300-49c6-9f66-1788a2d64518",
                //      "displayName": "google.distancematrix"
                //    },
                //    "intentDetectionConfidence": 0.7327829,
                //    "languageCode": "en"
                //  },
                //---------------
                //  "outputContexts": [
                //  {
                //    "name": "projects/misty-concierge-ewnrgd/agent/sessions/123456/contexts/computervision",
                //    "lifespanCount": 1
                //  }
                //],
                //"intent": {
                //  "name": "projects/misty-concierge-ewnrgd/agent/intents/2b771b5e-1b6e-4273-b6d2-682b5b9cd03a",
                //  "displayName": "misty.tech.computervision"
                //},
                //-------------------
                //  "outputAudio": 
                //    "outputAudioConfig": {
                //          "audioEncoding": "OUTPUT_AUDIO_ENCODING_LINEAR_16",
                //      "synthesizeSpeechConfig": {
                //                        "speakingRate": 0.95,
                //          "voice": {
                //                            "name": "en-US-Wavenet-F"
                //      },
                //      "effectsProfileId": [
                //        "handset-class-device"
                //      ]
                //    }
                //}
                //}
                string textToSpeak = "";
                var intent = "unknown";
                try
                {
                    intent = (string)jsonResp["queryResult"]["intent"]["displayName"];
                    _misty.SkillLogger.LogInfo($"MistyConcierge: IN DialogflowResponseCallback: " + intent + " was RECOGNIZED");
                    _misty.SkillLogger.LogVerbose($"MistyConcierge: IN DialogflowResponseCallback: -- jsonResp: {jsonResp.ToString(Formatting.Indented)}");
                }
                catch (Exception ex)
                {
                    _misty.SkillLogger.Log($"MistyConcierge: IN DialogflowResponseCallback: Intent NOT RECOGNIZED", ex);
                }
                // Call the specific Api based on the user's intent.
                switch (intent)
                {
                    case "misty.info":
                        // Play information about Misty and the Concierge application.
                        textToSpeak = _mistyInfo.GetNextItem();
                        _misty.SkillLogger.LogVerbose($"MistyConcierge: IN GetAudioCallback: -- _mistyInfo.GetNextItem -- textToSpeak : {textToSpeak}");
                        break;
                    case "mistyinfo.on":
                        // Set timer to automatically provide the user with information about Misty and the Concierge application - play every 2 minute(s).
                        _mistyInfoTimer = new Timer(MistyInfoCallback, null, 2 * 60 * 1000, 2 * 60 * 1000);
                        textToSpeak = "The timer has been turned ON to automatically play information about me and the Concierge application every 2 minutes.";
                        _misty.SkillLogger.LogVerbose($"MistyConcierge: IN GetAudioCallback: -- _mistyInfoTimer -- textToSpeak : {textToSpeak}");
                        break;
                    case "current.time":
                        textToSpeak = _tellTime.GetCurrentTime();
                        _misty.SkillLogger.LogVerbose($"MistyConcierge: IN GetAudioCallback: -- _tellTime.GetCurrentTime() -- textToSpeak : {textToSpeak}");
                        break;
                    case "google.distancematrix":
                        // "queryResult": {
                        //    "queryText": "what is the distance to Richmond",
                        //    "speechRecognitionConfidence": 0.987629,
                        //    "parameters": {
                        //        "to-location": "Richmond"
                        string toLocation = (string)jsonResp["queryResult"]["parameters"]["to-location"];
                        _misty.SkillLogger.LogVerbose($"MistyConcierge: IN GetAudioCallback: -- TO-LOCATION: {toLocation}");
                        _misty.DisplayImage("e_joy.jpg", null, false, null);
                        textToSpeak = _mapDistanceMatrixApi.CallGoogleDistanceMatrixAsync(MistyApiInfo.CurrentLocation, toLocation + " BC");
                        _misty.SkillLogger.LogVerbose($"MistyConcierge: IN GetAudioCallback: -- _mapDistanceMatrixApi.CallGoogleDistanceMatrixAsync() -- textToSpeak : {textToSpeak}");
                        break;
                    case "tell.joke":
                        _misty.DisplayImage("e_joy.jpg", null, false, null);
                        textToSpeak = _jokeApi.GetJokeInfo();
                        _misty.SkillLogger.LogVerbose($"MistyConcierge: IN GetAudioCallback: -- GetJokeInfo() -- textToSpeak : {textToSpeak} ");
                        break;
                    case "tell.quote":
                        _misty.DisplayImage("e_joy.jpg", null, false, null);
                        textToSpeak = _quoteApi.GetQuoteInfo();
                        _misty.SkillLogger.LogVerbose($"MistyConcierge: IN GetAudioCallback: -- GetQuoteInfo() -- textToSpeak : {textToSpeak}");
                        break;
                    case "news.info":
                        _misty.DisplayImage("e_joy.jpg", null, false, null);
                        textToSpeak = _newsApi.GetNewsInfo();
                        _misty.SkillLogger.LogVerbose($"MistyConcierge: IN GetAudioCallback: -- _newsApi.GetNewsInfo() -- textToSpeak : {textToSpeak}");
                        break;
                    case "twilio.sendtext":
                        textToSpeak = SendTextMessage(jsonResp);
                        break;
                    case "stock.info":
                        _misty.DisplayImage("e_joy.jpg", null, false, null);
                        string company = (string)jsonResp["queryResult"]["parameters"]["company"];
                        textToSpeak = _stockApi.GetStockQuoteInfo(company);
                        _misty.SkillLogger.LogVerbose($"MistyConcierge: IN GetAudioCallback: -- GetStockQuoteInfo({company}) -- textToSpeak : {textToSpeak}");
                        break;
                    case "weather.info":
                        _misty.DisplayImage("e_joy.jpg", null, false, null);
                        textToSpeak = _weatherApi.GetCurrentWeatherInfo(MistyApiInfo.CurrentLocation);
                        _misty.SkillLogger.LogVerbose($"MistyConcierge: IN GetAudioCallback: -- GetCurrentWeatherInfo() -- textToSpeak : {textToSpeak} ");
                        break;
                    case "venues.info":
                        _misty.DisplayImage("e_joy.jpg", null, false, null);
                        textToSpeak = _foursquareApi.GetVenuesInfo("Restaurants");
                        _misty.SkillLogger.LogVerbose($"MistyConcierge: IN GetAudioCallback: -- GetVenuesInfo() -- textToSpeak : {textToSpeak}");
                        break;
                    case "greetings.on":
                        _misty.StartFaceRecognition(null);
                        //Register the Face Recognition Event.
                        _misty.RegisterFaceRecognitionEvent(FaceRecCallback, 0, false, null, null, null);
                        textToSpeak = (string)jsonResp["queryResult"]["fulfillmentText"];
                        _misty.SkillLogger.LogVerbose($"MistyConcierge: IN GetAudioCallback: -- greetings.on -- textToSpeak : {textToSpeak}");
                        break;
                    case "greetings.off":
                        _misty.UnregisterEvent("FaceRecognition", null);
                        textToSpeak = (string)jsonResp["queryResult"]["fulfillmentText"];
                        _misty.SkillLogger.LogVerbose($"MistyConcierge: IN GetAudioCallback: -- greetings.off -- textToSpeak : {textToSpeak}");
                        break;
                    case "smartdeviceplug.on":
                        _smartDeviceApi.TurnONPlug();
                        textToSpeak = (string)jsonResp["queryResult"]["fulfillmentText"];
                        _misty.SkillLogger.LogVerbose($"MistyConcierge: IN GetAudioCallback: -- smartdeviceplug.on -- textToSpeak : {textToSpeak}");
                        break;
                    case "smartdeviceplug.off":
                        _smartDeviceApi.TurnOFFPlug();
                        textToSpeak = (string)jsonResp["queryResult"]["fulfillmentText"];
                        _misty.SkillLogger.LogVerbose($"MistyConcierge: IN GetAudioCallback: -- smartdeviceplug.off -- textToSpeak : {textToSpeak}");
                        break;
                    case "avstreaming.on":
                        // IMPORTANT:  After turning ON AV Streaming then Misty no longer uses key phrase "Hey Misty".
                        // Touch the top of Misty's head to deactive the AV Streaming.
                        TurnONAVStreaming();
                        textToSpeak = (string)jsonResp["queryResult"]["fulfillmentText"];
                        _misty.SkillLogger.LogVerbose($"MistyConcierge: IN GetAudioCallback: -- avstreaming.on -- textToSpeak : {textToSpeak}");
                        break;
                    default:
                        // Speaks out any text response from Dialogflow (from the fulfillmentText property) returned by other intents not
                        // explicitly named in the conditional blocks above.
                        textToSpeak = "I am sorry, could you please try again?";

                        if (intent != "Default Fallback Intent" && intent != "unknown")
                        {
                            // Checks if a text response is returned by Dialogflow
                            try
                            {
                                textToSpeak = (string)jsonResp["queryResult"]["fulfillmentText"];
                                _misty.SkillLogger.LogVerbose($"MistyConcierge: IN GetAudioCallback: -- default -- textToSpeak : {textToSpeak}");

                            }
                            catch (Exception ex)
                            {
                                _misty.SkillLogger.Log($"No text response was returned by {intent} intent.", ex);
                            }
                        }
                        break;
                }
                if (textToSpeak != "")
                {
                    // Replace any single quotes in string.
                    textToSpeak = textToSpeak.Replace("'", @"\'");
                    // Turn Misty's head to the direction of the speaker then play the text.
                    TurnSpeakersDirection();
                    _playingAudio = true;
                    _speechApi.GoogleAuthToken = _googleAuthToken;
                    bool done = false;
                    do
                    {
                        // If not busy processing other text then:
                        if (!_speechApi.IsBusy)
                        {
                            done = await _speechApi.SpeakTheTextAsync(textToSpeak);
                        }
                        else { _misty.Wait(1000); }
                    } while (!done);
                    _playingAudio = false;
                }
            }
            catch (Exception ex)
            {
                _misty.SkillLogger.Log($"MistyConcierge: IN DialogflowResponseCallback: => Exception", ex);
            }
        }

        /// <summary>
        /// Callback called when a cap touch sensor is used to turn OFF the AV Streaming
        /// </summary>
        /// <param name="capTouchEvent"></param>
        private async void CapTouchCallback(ICapTouchEvent capTouchEvent)
        {

            if (capTouchEvent.IsContacted)
            {
                switch (capTouchEvent.SensorPosition)
                {
                    case CapTouchPosition.Front:
                        // Touch the Front of Misty's head to turn OFF the AV streaming and enable key phrase voice commands. 
                        TurnOFFAVStreaming();
                        string textToSpeak = "My AV streaming has been turned OFF";
                        _misty.SkillLogger.LogVerbose($"MistyConcierge : IN CapTouchCallback() -- textToSpeak: {textToSpeak}");
                        bool done = false;
                        do
                        {
                            // If not busy processing other text then:
                            if (!_speechApi.IsBusy)
                            {
                                done = await _speechApi.SpeakTheTextAsync(textToSpeak);
                            } else { _misty.Wait(1000); }
                        } while (!done);

                        _misty.RegisterKeyPhraseRecognizedEvent(KeyPhraseRecognizedCallback, 0, true, null, null);
                        _misty.RegisterVoiceRecordEvent(OnVoiceRecordReceived, 10, false, "VoiceRecordEvent", null);
                        _misty.StartKeyPhraseRecognition(true, true, 7000, null, null);
                        break;
                }
            }
        }

        /// <summary>
        /// If Misty is not already locked on to a human face, we use audio localization data to turn her head in the speaker's direction 
        /// </summary>
        private void TurnSpeakersDirection()
        {
            double angleTolook = _speechSourceAngle >= 0 ? _yawLeft * _speechSourceAngle / 90 : _yawRight * Math.Abs(_speechSourceAngle) / 90;
            _misty.SkillLogger.LogVerbose($"MistyConcierge : IN TurnSpeakersDirection() -- _speechSourceAngle: {_speechSourceAngle} -- _yawLeft: {_yawLeft} -- _yawRight: {_yawRight} -- angleTolook: {angleTolook} -- _findFace: {_findFace}");
            if (_findFace)
            {
                _misty.MoveHead(-20, 0, angleTolook, 80 , AngularUnit.Degrees, null);
                _misty.MoveArm(85, (angleTolook >= 0) ? RobotArm.Left : RobotArm.Right, 60, null, AngularUnit.Degrees, null);
                _misty.MoveArm(-29, (angleTolook >= 0) ? RobotArm.Right : RobotArm.Left, 60, null, AngularUnit.Degrees, null);
            }
            else
            {
                _misty.MoveArm(85, (_headYaw >= 0) ? RobotArm.Left : RobotArm.Right, 60, null, AngularUnit.Degrees, null);
                _misty.MoveArm(-29, (_headYaw >= 0) ? RobotArm.Right : RobotArm.Left, 60, null, AngularUnit.Degrees, null);
            }
        }
 
        /// <summary>
        /// Send an SMS text message to the person specified in the intent.  This methods uses the words after "saying" as the text 
        /// for the message being sent.
        /// </summary>
        /// <param name="jsonResp"></param>
        /// <returns></returns>
        private string SendTextMessage(Newtonsoft.Json.Linq.JObject jsonResp)
        {
            // "queryResult": {
            //      "queryText": "send a text message to Wes saying hello how are you today",
            //      "speechRecognitionConfidence": 0.9758415,
            //      "parameters": {
            //          "person": {
            //              "name": "Wes"
            //          }
            _misty.DisplayImage("e_joy.jpg", null, false, null);
            _misty.SkillLogger.LogVerbose($"MistyConcierge: -- GetAudioCallback -- IN SendTextMessage()");
            string audioText = (string)jsonResp["queryResult"]["queryText"];
            string personKey = (string)jsonResp["queryResult"]["parameters"]["person"][0]["name"];
            string personUpperKey = personKey.ToUpper();
            // Check for the key word of 'saying' to extract the message from the user e.g., send a text to Wes saying 'this is a reminder about our meeting this morning'. 
            int wordIndex = audioText.IndexOf("saying");
            if (wordIndex == -1) wordIndex = 0;
            string userText = audioText.Substring(wordIndex + 6);
            string smsMessageText = "MISTY sent you the following text message - PLEASE DO NOT REPLY:  " + userText;
            MistyFriends.FriendInfo friend = MistyFriends.LookupFriendInfo(personUpperKey);
            _misty.SkillLogger.LogVerbose($"MistyConcierge: IN GetAudioCallback: -- FullName: {friend.FullName} - CellPhoneNumber: {friend.CellPhoneNumber} - smsMessageText: {smsMessageText}");
            _twilioApi.SendSMSMessage(friend.CellPhoneNumber, smsMessageText);
            return "Using the Twilio api, I have sent a text message to " + friend.FullName;
        }


        /// <summary>
        /// Callback called to tell the user some useful information.
        /// </summary>
        /// <param name="data"></param>
        private async void MistyInfoCallback(object state)
        {
            try
            {
                // Stop the key phrase recogniztion before playing the useful info audio file because it
                // contains the words "Hey Misty" and Misty will start recording unintentionally.
                _misty.StopKeyPhraseRecognition(null);
                _misty.StopRecordingAudio(null);
                _playingAudio = true;
                // Get the next piece of useful information to tell the user.
                string mistyInfoText = _mistyInfo.GetNextItem();
                bool done = false;
                do
                {
                    // If not busy processing other text then:
                    if (!_speechApi.IsBusy)
                    {
                        done = await _speechApi.SpeakTheTextAsync(mistyInfoText);
                    }
                    else { _misty.Wait(1000); }
                } while (!done);
                _playingAudio = false;
                // Start the key phrase recognition after the audio file has been played.
                _misty.RegisterKeyPhraseRecognizedEvent(KeyPhraseRecognizedCallback, 0, true, null, null);
                _misty.RegisterVoiceRecordEvent(OnVoiceRecordReceived, 10, false, "VoiceRecordEvent", null);
                _misty.StartKeyPhraseRecognition(true, true, 7000, null, null);
            }
            catch (Exception ex)
            {
                _misty.SkillLogger.Log($"MistyConcierge: IN TalkToUserCallback: => Exception", ex);
            }
        }

        /// <summary>
        /// Turns ON the Audio Video Streaming from Misty.
        /// </summary>
        private async void TurnONAVStreaming()
        {
            await _misty.EnableAvStreamingServiceAsync();
            StartAvStreamingParameters avStreamingParameters = new StartAvStreamingParameters();
            avStreamingParameters.Url = "rtspd:1935";
            avStreamingParameters.Height = 480;
            avStreamingParameters.Width = 640;
            await _misty.StartAvStreamingAsync(avStreamingParameters);
            _misty.ChangeLED(255, 128, 0, null); // orange
        }

        /// <summary>
        /// Turns OFF the Audio Video Streaming from Misty when the user touches Misty's head. 
        /// </summary>
        private async void TurnOFFAVStreaming()
        {
            // Disable the AV Streaming when the skill is cancelled.
            await _misty.StopAvStreamingAsync();
            _misty.ChangeLED(0, 255, 0, null); // green
        }

        #endregion

        #region *** OnPause / OnResume / OnCancel / OnTimeout / OnResponse / Dispose ***

        /// <summary>
        /// This event handler is called when Pause is called on the skill
        /// User can save the skill status/data to be retrieved when Resume is called
        /// Infrastructure to help support this still under development, but users can implement this themselves as needed for now 
        /// </summary>
        /// <param name="parameters"></param>
        public void OnPause(object sender, IDictionary<string, object> parameters)
		{
			//In this template, Pause is not implemented by default
		}

		/// <summary>
		/// This event handler is called when Resume is called on the skill
		/// User can restore any skill status/data and continue from Paused location
		/// Infrastructure to help support this still under development, but users can implement this themselves as needed for now 
		/// </summary>
		/// <param name="parameters"></param>
		public void OnResume(object sender, IDictionary<string, object> parameters)
		{
			//TODO Put your code here and update the summary above
		}
		
		/// <summary>
		/// This event handler is called when the cancel command is issued from the robot/user
		/// You currently have a few seconds to do cleanup and robot resets before the skill is shut down... 
		/// Events will be unregistered for you 
		/// </summary>
		public void OnCancel(object sender, IDictionary<string, object> parameters)
		{
            _misty.SkillLogger.LogVerbose($"MistyConcierge : OnCancel called");
            DoCleanup();
            _misty.ChangeLED(255, 255, 0, null);  // yellow
        }

		/// <summary>
		/// This event handler is called when the skill timeouts
		/// You currently have a few seconds to do cleanup and robot resets before the skill is shut down... 
		/// Events will be unregistered for you 
		/// </summary>
		public void OnTimeout(object sender, IDictionary<string, object> parameters)
		{
            _misty.SkillLogger.LogVerbose($"MistyConcierge : OnCancel called");
            DoCleanup();
            _misty.ChangeLED(255, 255, 0, null);  // yellow
        }

		private bool _isDisposed = false;

		private void Dispose(bool disposing)
		{
			if (!_isDisposed)
			{
				if (disposing)
				{
                    _getTokenTimer?.Dispose();
                }

				// TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
				// TODO: set large fields to null.

				_isDisposed = true;
			}
		}

		// This code added to correctly implement the disposable pattern.
		public void Dispose()
		{
			// Do not change this code. Put cleanup code in Dispose(bool disposing) above.
			Dispose(true);
			// TODO: uncomment the following line if the finalizer is overridden above.
			// GC.SuppressFinalize(this);
		}
        /// <summary>
        /// Performs some cleanup on the state of the robot
        /// </summary>
        private void DoCleanup()
        {
            _misty.Stop(null);
            _misty?.StopKeyPhraseRecognition(null);
            _misty?.StopFaceRecognition(null);
        }
        #endregion
    }
}
