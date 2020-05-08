
## Table of Contents

This guide provides all of the information you need to start using the Misty Concierge C# Template with your own Misty II.

- [ Misty Concierge C# Template](#misty-concierge-c#-template)
  - [Overview](#overview)
  - [Requirements](#requirements)
  - [Importing a Dialogflow Agent](#Importing-a-Dialogflow-Agent)
  - [Creating an Access Token Function](#creating-an-access-token-function)
  - [Enabling the Text-to-Speech API](#Enabling-the-text-to-speech-api)
  - [Setting up the External Apis](#setting-up-the-External-Apis)
  - [Updating your Skill Code](#updating-your-skill-code)
  - [Deploying the Concierge application](#deploying-the-concierge-application)
  - [Next Steps](#next-steps)


---

**WARRANTY DISCLAIMER.**

* General. TO THE MAXIMUM EXTENT PERMITTED BY APPLICABLE LAW, MISTY ROBOTICS PROVIDES THIS SAMPLE SOFTWARE “AS-IS” AND DISCLAIMS ALL WARRANTIES AND CONDITIONS, WHETHER EXPRESS, IMPLIED, OR STATUTORY, INCLUDING THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE, TITLE, QUIET ENJOYMENT, ACCURACY, AND NON-INFRINGEMENT OF THIRD-PARTY RIGHTS. MISTY ROBOTICS DOES NOT GUARANTEE ANY SPECIFIC RESULTS FROM THE USE OF THIS SAMPLE SOFTWARE. MISTY ROBOTICS MAKES NO WARRANTY THAT THIS SAMPLE SOFTWARE WILL BE UNINTERRUPTED, FREE OF VIRUSES OR OTHER HARMFUL CODE, TIMELY, SECURE, OR ERROR-FREE.
* Use at Your Own Risk. YOU USE THIS SAMPLE SOFTWARE AND THE PRODUCT AT YOUR OWN DISCRETION AND RISK. YOU WILL BE SOLELY RESPONSIBLE FOR (AND MISTY ROBOTICS DISCLAIMS) ANY AND ALL LOSS, LIABILITY, OR DAMAGES, INCLUDING TO ANY HOME, PERSONAL ITEMS, PRODUCT, OTHER PERIPHERALS CONNECTED TO THE PRODUCT, COMPUTER, AND MOBILE DEVICE, RESULTING FROM YOUR USE OF THIS SAMPLE SOFTWARE OR PRODUCT.

Please refer to the Misty Robotics End User License Agreement for further information and full details: https://www.mistyrobotics.com/legal/end-user-license-agreement/

---

## Misty Concierge C# Template

## Overview

The **Misty Concierge Template - C# Edition** is an open-source C# skill template for Misty II robots. It is based on the Misty Concierge Template - Javascript edition.

The Misty skills in this repository focus on *information sharing*. They integrate with Dialogflow to give Misty the ability to handle voice interactions, answer questions, and look up information. As she listens and responds, Misty maintains eye contact with the speaker, moves her head and arms, blinks her LED, plays sounds, and changes her expression to invite engagement and delight those who speak to her.

The C# Misty Concierge application has the following features and capabilities:

1.   Tell the current time
2.   Tell a joke
3.   Tell a daily inspirational quote
4.   Tell the current temperature / weather
5.   Provide features & capabilities about Misty and the Concierge application.
6.   Provide driving distance and time to a specified location
7.   Send SMS text messages to friends
8.   Provide a top news headline
9.   Perform facial recognition and play a multiple personal greetings to friends
10.  Provide the current price of a stock
11.  Provide a short list of restaurants in the area
12.  Turn ON and OFF a light
13.  Turn ON Misty’s AV streaming and touch Misty’s head to turn it OFF

Below is a video demonstrating the features and capabilities of the Misty Concierge C# Template.
- https://www.youtube.com/watch?v=MNzSJObXpAY&feature=youtu.be

---
The Misty Concierge C# Template repository includes the following resources:

* **Misty.Skill.Concierge** - A C# project containing the basic functionality for the concierge use case.  It provides the framework for sending audio files to Dialogflow and handling the Dialogflow response data. Use this template to start building custom voice interactions for your Misty II robot.
* **Misty.Services** - A C# project containing a number of external api services as well as other C# classes for specific features (e.g., Friends information for personal greeting and Twilio SMS text messages.  Use this C# project as an examples of how to use third-party services with your concierge skills.
* **Dialogflow** - Includes a .zip file with JSON data for predefined intents and entities that you can import into a Dialogflow agent.

This guide provides instructions for using the code with your Misty II. If this is your first time using the template, we recommend starting with the base template to learn how Dialogflow works with your skill code. 

### Requirements

To use the template in this repository, you need a Misty II robot running:

- Robot Version: 1.13.0.10362 or higher
- Sensory Services Version: 1.13.0 or higher
- Windows OS Version: 10.0.17763.253 or higher

with the [2020.01.07 Android patch](https://community.mistyrobotics.com/t/2020-01-07-misty-ii-android-patch-for-improved-slam-performance/2415) applied.

***Note:** Some of the commands and event types used in these templates are **beta**, and related hardware, firmware, and software is still under development. Please recognize that these skills may behave unpredictably at this time, and their range of capabilities is likely to change as Misty improves. If you have questions about getting started, or if you run in to issues with the contents of this repository, we encourage you to [post in the Misty Community Forums](https://community.mistyrobotics.com/t/application-template-misty-as-a-concierge/2414)!*

---

## Misty Concierge C# Guide

When the `Misty.Skill.Concierge` skill runs, you can activate Misty with her wake word ("Hey, Misty!") to start talking to the robot. Misty turns to face you and moves her head to maintain eye contact while you speak. When you finish talking, Misty captures your speech in an audio file and sends this recording to be analyzed by your own custom Dialogflow agent. Dialogflow returns a data object you can use in your skill to program Misty's response to different questions and statements. Read this section to learn how to:

* create a Dialogflow agent with *intents* for handling a few basic types of questions that people might ask your robot
* use Google Cloud Functions to automatically generate a token for use with Google's services each time the concierge skill runs
* enable the Google text-to-speech API for use with your skill
* code how Misty reacts to different responses from Dialogflow
* update the `MistyApiInfo.cs` skill code with your own credentials and external api keys to run the skill on your Misty II

Start by forking, cloning, or downloading this repository to your local machine. Once you have a copy of the template files to work with, read the next section to learn how to create your own Dialogflow agent. 

### Importing a Dialogflow Agent

In the Dialogflow folder, the zip file to import into Google Dialogflow is called misty-concierge-scharp.zip.  Following the instructions in the link below to import this zip file. 

- [Importing a Dialogflow Agent (from Misty Concierge Javascript Template documentation)](https://github.com/MistySampleSkills/Misty-Concierge-Template#importing-a-dialogflow-agent)


### Creating an Access Token Function

Follow the instructions in the Misty Concierge Javascript Template documentation.

- [Creating an Access Token Function](https://github.com/MistySampleSkills/Misty-Concierge-Template#creating-an-access-token-function)


### Enabling the Text-to-Speech API

Follow the instructions in the Misty Concierge Javascript Template documentation.
- [Enabling the Text-to-Speech API](https://github.com/MistySampleSkills/Misty-Concierge-Template#enabling-the-text-to-speech-api)

---

### Setting up the External APIs

The Misty Concierge application calls many external Apis to retrieve data.  In the Misty.Services project, update the `MistyApiInfo.cs` file with the following informaton:

Start by setting your current location.  This is used for driving distance and well as for the current weather.
- CurrentLocation = "Port Coquitlam BC"


#### Google Cloud Functions API:

Before we can use the Concierge application with our own Dialogflow agent, we must update the our unique Project ID and the trigger URL for our cloud function.

Update the following constants for the Google Cloud Functions API.
- CloudFunctionAuthTokenUrl
- GoogleCloudProjectId

#### Foursquare Locations API:

Follow the instructions in the Misty Concierge Javascript Template documentation.
- [Setting up Foursquare Places API](https://github.com/MistySampleSkills/Misty-Concierge-Template#setting-up-foursquare)

Update the following constants for the Foursquares API.
- FoursquareClientId
- FoursquareClientSecret
- FoursquareLocationLatLon = "49.251546,-122.7665771";
 // the latitude and longitude for the location to search around to find a venue.

#### Google Driving Matrix API:

Below is a link to the Get Started information for the Google Driving Matrix API.
https://developers.google.com/maps/documentation/distance-matrix/start 

After creating an API key then update the following constant.
- GoogleDistanceMatrixApiKey 

#### Joke API:

Below is the link to the web site providing the Joke information.  No additioanl setup is required to use this API.
"https://icanhazdadjoke.com/";

#### Daily Quote API: 

Below is the link to the web site providing the Joke information.  No additioanl setup is required to use this API.
"https://quotes.rest/"
Note: The Misty Concierge plays credit information.  You must credit They Said So when using the free version.

#### News Headlines Api

Below is a link to the Get Started information for the News API.
https://newsapi.org/docs/get-started
https://newsapi.org/ 

After creating an API key then update the following constant.
- NewsapiorgApiKey

#### Stock Quote API:

Below is a link to the Get Started information for the Stoke Quote API.
https://www.alphavantage.co/

After creating an API key then update the following constant in the MistApiInfo.cs file.
- StockApiKey

#### Twilio C# Library:

Below is a link to the documentation for the Twilio C# API.
https://www.twilio.com/docs/usage/api

After creating your twilio account and a Twilio phone number then update the following constants in the MistApiInfo.cs file.
- TwilioAccountSid
- TwilioAuthToken
- TwilioPhoneNumber

#### Weather API:

Below is the link to the Weather API site.
"http://api.weatherstack.com";

After creating an API key then update the following constants in the MistApiInfo.cs file.
- WeatherApiKey

#### Wemo Smart Plug C# Library:

Below is a link to the C# library used to turn ON and OFF the Wemo Smart Plug.
https://github.com/seanksullivan/Wemo.net

If you have a Wemo Smart Plug then update the IP Address constant value.

- WemoDeviceIpAddress = "http://10.0.0.227";

---

### Setting up the Personal Greeting skill

The Personal Greeting skill performs facial recognition and then plays a person greeting to each of your friends.

In the Misty.Services project, update the `FirendsInfo.cs` file with FriendInfo{} for each of your friend and family members.  The Cellular Phone Number is used for sending SMS text messages using the Twilio API. 

      public static List<FriendInfo> friends = new List<FriendInfo>
      {
          new FriendInfo {
              Key = "FACE_TRAINED_NAME_1", FirstName = "First Name #1",   LastName = "Last Name #1", CellPhoneNumber = "+19998887777",
              Greetings = new List<string>() { "<SPECIFY A SPECIAL GREETING FOR EACH OF YOUR FRIENDS AND FAMILY>" }
          },
          new FriendInfo {
              Key = "FACE_TRAINED_NAME_1", FirstName = "First Name #2",   LastName = "Last Name #2", CellPhoneNumber = "+19991112222",
              Greetings = new List<string>() { "Hello.  You look fantastic today.  Welcome to our home.  Please come in and relax in the living room." }
          },


## Updating your Skill Code

The code in the conciergeBaseTemplate.js skill file is organized into three discreet segments:

Code under the FOLLOW FACE / LOOK AT PERSON segment defines the behavior that allows Misty to localize to the person speaking and turn her head to look at the faces she detects.
Code under the CONCIERGE segment updates our access token, records user expressions, sends recordings to Dialogflow, and handles response data from our Dialogflow agent.
Code under the TEXT TO SPEECH segment defines the functions that allow Misty to use Google's text-to-speech service to speak her responses out loud.
To use the template with our custom Dialogflow agent, we only need to update the function that handles the response data from Dialogflow. This function gets the values for any intents and parameters that our agent identifies in a user expression, and it's where we write the conditional statements that determine how Misty responds.

Locate the function declaration for the _dialogueFlowResponse() callback function. This callback runs when the skill receives a response from the Dialogflow agent. Look for the conditional block that handles different intents:

    // Call the specific Api based on the user's intent.
    switch (intent)
    {
        case "YOUR_INTENT_NAME":
            // Get the text from the response for the Google Dialogflow request.
            textToSpeak = (string)jsonResp["queryResult"]["fulfillmentText"];
            // Do Something.
            break;
        case "YOUR_OTHER_INTENT_NAME":
            // Get the text from the response for the Google Dialogflow request.
            textToSpeak = (string)jsonResp["queryResult"]["fulfillmentText"];
            // Do Something.
            break;


## Deploying the Concierge application

Misty II's .NET SDK provides tools for building, debugging, and deploying skills with Microsoft Visual Studio and the .NET framework. Skills you build with the .NET SDK assemble into background applications that run alongside Misty's software on Windows IoT Core.

https://docs.mistyrobotics.com/misty-ii/net-sdk/overview/


Use Visual Studio 2017 to open the `Misty.Skill.Concierge.slc' Solution file.  Then follow the "Building & Deploying a .NET Skill" section in the .NET SDK Getting Started link below to deploy and run the Concierge skill:

https://docs.mistyrobotics.com/misty-ii/net-sdk/getting-started/


## Next Steps

To expand on the functionality of this template, you can use the `MistyServiceTemplate.cs` file in the `Misty.Services` project as a template file for creating a new Service to include in the project.  Then you would create additional intents in your Dialogflow agent for handling other categories of expression. Then you can update your skill code with additional logic that describes how Misty should respond to each new intent.

