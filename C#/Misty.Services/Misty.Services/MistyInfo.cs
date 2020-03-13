using System;
using System.Collections.Generic;
using MistyRobotics.SDK.Messengers;

namespace Misty.Services
{
    /// <summary>
    /// This class returns a random piece of information about Misty and the Concierge application.
    /// </summary>
    public class MistyInfo
    {
        private IRobotMessenger _misty;
        /// <summary>
        /// Random number generator
        /// </summary>
        private Random _randomGenerator = new Random();
        private List<string> _listOfInfo = new List<string>();

        public MistyInfo(IRobotMessenger misty1)
        {
            _misty = misty1;
            LoadListOfInfo();
        }

        /// <summary>
        /// Callback called to return the next piece of useful information.
        /// </summary>
        /// <param name="data"></param>
        public string GetNextItem()
        {
            string infoText = "";
            try
            {
                string sentencePrefix = "Did you know... ";
                int itemNo = (int)_randomGenerator.Next(0, _listOfInfo.Count); // Max value must be count + 1 to generate a number from 0 to count - 1.
                infoText = sentencePrefix + _listOfInfo[itemNo];
                _misty.SkillLogger.LogVerbose($"MistyConcierge : IN GetNextInfo - infoText: {infoText}");
                // Remove the item from the list so it will not be told to the user again.
                _listOfInfo.RemoveAt(itemNo);
            }
            catch (Exception ex)
            {
                _misty.SkillLogger.Log($"Misty.Services: IN GetNextInfo: => Exception", ex);
            }
            return infoText;
        }

        /// <summary>
        /// Load useful information (to tell a user) into a List /  Array.
        /// </summary>
        private void LoadListOfInfo()
        {
            // List of useful information to tell the user.
            // TODO: Remove the hard coded list to a dynamically generated list.
            //
            // Misty Concierge Features
            _listOfInfo.Add("I can tell you the current time.  Just say hey misty... tell me the current time.");
            _listOfInfo.Add("I can tell you a joke.  Just say hey misty... tell me a joke.");
            _listOfInfo.Add("I will annouce the current time every 15 minutes of the hour.");
            _listOfInfo.Add("I can provide you with the quote of the day.  Just say... hey misty, tell me a quote.");
            _listOfInfo.Add("I can provide you with driving distance and time to a specific location.  Just say... hey misty, how far is it to Vancouver?");
            _listOfInfo.Add("I can provide you with the top news stories.");
            _listOfInfo.Add("I can tell you about restaurants in your area.  Just say... hey misty what restaurants are in my area?");
            _listOfInfo.Add("I can turn ON and OFF the light.  Just say... hey misty turn on the light.");
            _listOfInfo.Add("I can provide you with the current price of a stock.  Just say... hey misty what is the price of Microsoft stock?");
            _listOfInfo.Add("I can send a text message to one of your friends.  Just say... hey misty send a text message to Linda saying Hello from Misty!!");
            _listOfInfo.Add("I can tell you the current weather / temperature.  Just say... hey misty what is the current temperature?");
            _listOfInfo.Add("I can identify a person using facial recognition and then provide a personal greeting to the person.");
            _listOfInfo.Add("I can turn ON my AV streaming so you can see what I see using video client software.");
            _listOfInfo.Add("you can tell me to turn on my AV streaming and then turn it off by touching the top of my head.");
            _listOfInfo.Add("I am integrated with a Google Assistant.  Just say... Start Google Assistant... then just say Stop Google Assistant to deactivate it.");
            // Misty Tech Information
            _listOfInfo.Add("I may look cute and approachable from the outside, but on the inside, I am all business.");
            _listOfInfo.Add("I am packed with sophisticated hardware and software features that contribute to my ruggedness and extensibility as a platform.");
            _listOfInfo.Add("I can autonomously move about a room avoiding obstacles");
            _listOfInfo.Add("I can detect and recognize people with an expansive field of view");
            _listOfInfo.Add("I am not just an amazing piece of tech on my own — I am designed for easy hardware modification with my many attachment points.");
            _listOfInfo.Add("I am naturally talented. Packed with capabilities out of the box, you can start building skills immediately.");
            _listOfInfo.Add("I can be easily programmed, my tools and documentation make it easy to bring your ideas to life on Misty.Skills can be developed using the JavaScript SDK, REST, JavaScript, and Python remote command interfaces and the C sharp SDK.");
            _listOfInfo.Add("For Programmable Personality, you can bring personality to me and my skills, through my body/head movement, eyes, sounds and voice (via third party integration).");
            _listOfInfo.Add("I am packed with sophisticated hardware and software features that contribute to my ruggedness and extensibility as a platform.  I am naturally talented. Packed with capabilities out of the box, you can start building skills immediately.");
            _listOfInfo.Add("Some of my features include computer vision, Distance and Obstacle Detection, sound, movement, display and light, connectivity, and extensibility.");
            _listOfInfo.Add("My Computer Vision uses an Occipital Structure Core depth sensors for 3D maps, a 4K camera.");
            _listOfInfo.Add("For facial recognition, I use Deep-learning AI using the Qualcomm Snapdragon Neural Processing Engine.");
            _listOfInfo.Add("the hall of fame includes heroes that are bringing all sorts of cool things to life for Misty.  The types of Misty skills include:  Intergenerational Storytelling, Air Quality Monitor, Concierge, Work Assistant, Magician, Library Assistant, Fall Detector and Mobile Action, Household Companion, and a Story Teller.");
            _listOfInfo.Add("For Face Detection and Recognition, I know when there is a human in my presence and can learn who that person is.");
            _listOfInfo.Add("for Wake Word Event, I can be activated via “Hey, Misty”, enabling follow-on directions.");
            _listOfInfo.Add("For Sound Localization, I am able to identify the direction of and turn towards the human upon receiving directions after my activation.");
            _listOfInfo.Add("for Audio Recording, I can record audio for use in a skill or saving offline.");
            _listOfInfo.Add("for Audio Playback, I can play back audio files as a part of a skill.");
            _listOfInfo.Add("for Capacitive Touch, I can respond to human touch on my head and chin.");
            _listOfInfo.Add("I am One Part Hardware, One Part Software, and Packed with Technology.  I may look cute and approachable from the outside, but on the inside, I am all business.  I am naturally talented. Packed with capabilities out of the box, you can start building skills immediately.");
            _listOfInfo.Add("I am not just an amazing piece of tech on my own—I was designed for easy hardware modification with my many attachment points.");
        }

    }
}
