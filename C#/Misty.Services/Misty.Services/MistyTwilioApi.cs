using MistyRobotics.SDK.Messengers;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace Misty.Services
{
    /// <summary>
    /// This service call the Twilio .NET library to send text messages to a phone number.
    /// </summary>
    public class MistyTwilioApi
    {
        private IRobotMessenger _misty;

        public MistyTwilioApi(IRobotMessenger misty1, string twilioAccountSid, string twilioAuthToken)
        {
            _misty = misty1;
            TwilioClient.Init(twilioAccountSid, twilioAuthToken);
        }

        /// <summary>
        /// Call the Twilio api to send an SMS text message to the phone number.
        /// </summary>
        /// <param name="toPhoneNumber">the phone number to send the text message to</param>
        /// <param name="smsMessageText">the text message to be sent</param>
        public void SendSMSMessage(string toPhoneNumber, string smsMessageText)
        {
            var message = MessageResource.Create(
                    body: smsMessageText,
                    from: new Twilio.Types.PhoneNumber(MistyApiInfo.TwilioPhoneNumber),
                    to: new Twilio.Types.PhoneNumber(toPhoneNumber)
            );

            _misty.SkillLogger.LogVerbose($"Misty.Services: IN SendSMSMessage() -- message.Body: " + message.Body);
        }
    }
}
