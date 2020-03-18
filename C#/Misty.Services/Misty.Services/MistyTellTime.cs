using System;
using System.Threading;
using MistyRobotics.SDK.Messengers;

namespace Misty.Services
{
    /// <summary>
    /// This service returns the current time to the calling module.  In addition, it plays the time every 15 minutes of the hour.
    /// </summary>
    public class MistyTellTime
    {
        private IRobotMessenger _misty;
        private Timer _checkTimeTimer;
        private bool _alarmStarted = false;
        private MistySpeechApi _speechApi;

        public MistyTellTime(IRobotMessenger misty1, MistySpeechApi speechApi)
        {
            _misty = misty1;
            _speechApi = speechApi;
            // Set the current time event
            _checkTimeTimer = new Timer(CheckTimeCallback, null, 1000, 60000);
            _misty.SkillLogger.LogVerbose($"Misty.Service : IN MistyTellTime() - TELL TIME SERVICE HAS STARTED");
        }

        public string GetCurrentTime()
        {
            // Get the current date/time
            DateTime today = DateTime.Now;
            _misty.SkillLogger.Log($"Misty.Service : IN GetCurrentTime() - today: {today.ToString()}");

            var timeminutes = today.Minute;
            var timehours = today.Hour;
            var amOrpm = "AM";

            // Switch from 24 hour time to 12 hour
            if (timehours > 12)
            {
                timehours = timehours - 12;
                amOrpm = "PM";
            }
            string strMinutes = timeminutes.ToString();
            if (timeminutes < 10) { strMinutes = "0" + timeminutes.ToString(); }
            string currentTimeText = $"The current time is: {timehours}:{strMinutes} {amOrpm}";
            if (timeminutes == 0)
            {
                currentTimeText = $"The current time is: {timehours} oclock {amOrpm}";
            }
            return currentTimeText;
        }

        /// <summary>
        /// This callback gets the current time and plays it on Misty.
        /// </summary>
        /// <param name="state"></param>
        private async void CheckTimeCallback(object state)
        {
            if (_alarmStarted) return;
            // Get the current date/time
            DateTime today = DateTime.Now;
            //_misty.SkillLogger.Log($"Misty.Service : IN CheckTimeCallback() - today: {today.ToString()}");

            var timeseconds = today.Second;
            var timeminutes = today.Minute;
            var timehours = today.Hour;
            var amOrpm = "AM";

            // Switch from 24 hour time to 12 hour
            if (timehours > 12)
            {
                timehours = timehours - 12;
                amOrpm = "PM";
            }

            // Plays the current time to the user every 15 minutes starting on the hour e.g., 10 oclock, 10:15, 10:30 and 10:45.
            if (((timeminutes == 0) || (timeminutes == 15) || (timeminutes == 30) || (timeminutes == 45)) && (!_alarmStarted))
            {
                string strMinutes = timeminutes.ToString();
                if (timeminutes < 10) { strMinutes = "0" + timeminutes.ToString(); }
                string currentTimeText = $"The current time is: {timehours}:{strMinutes} {amOrpm}";
                if (timeminutes == 0)
                {
                    currentTimeText = $"The current time is: {timehours} oclock {amOrpm}";
                }
                bool done = false;
                do
                {
                    // If not busy processing other text then:
                    if (!_speechApi.IsBusy)
                    {
                        await _speechApi.SpeakTheTextAsync(currentTimeText);
                        done = true;
                    } else { _misty.Wait(1000); }

                } while (!done);
                _alarmStarted = true;
                _misty.SkillLogger.LogVerbose($"MistyConcierge : IN CheckTimeCallback() - currentTimeText: {currentTimeText}");
                _misty.Wait(60000);
            }
            _alarmStarted = false;
        }
    }
}
