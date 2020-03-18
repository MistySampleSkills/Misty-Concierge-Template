# Misty-Concierge-Template

The **Misty Concierge Template** is an open-source project with skill templates for Misty II robots. As a Misty developer, you can use the code and documentation in this repository to quickly build and deploy skills that put Misty to work as a concierge in industries like hospitality, healthcare, education, eldercare, commercial real estate, retail, marketing, and even museums.

## Repository Contents

This repository provides templates for building a concierge skill using Misty's JavaScript and .NET (Beta) SDKs. It includes the following subdirectories:

* **JavaScript** - The original JavaScript implementation of the Misty Concierge Template, built with Misty's JavaScript SDK. Use the README in this directory to get started with the JavaScript version of the template.
* **C#** - A C# implementation of the Misty Concierge Template, built with Misty's .NET SDK (Beta). Use the README in this directory to get started with a C# version of the template.
* **img** - Images and screenshots used in the documentation for this repository.

The skills you build with these templates focus on *information sharing*. Each template integrates with Dialogflow and other third-party APIs to give Misty the ability to handle voice interactions, answer questions, and look up information. As she listens and responds, Misty maintains eye contact with the speaker, moves her head and arms, blinks her LED, plays sounds, and changes her expression to invite engagement and delight those who speak to her. All of this interaction is built into the template for you to customize and build on for your own use case. 

You can extend these templates to put Misty to work in a number of ways. For example:

* Misty could answer questions about local events, what's on the menu for dinner, or what the weather is like.
* She could take reservations and order tickets, or help with room service orders via further integrations with 3rd party services and point-of-sale systems.
* You could ask Misty to book cars or taxis.
* Misty could tell fun facts, tell jokes, or play games.
* You could ask Misty to page someone by sending an SMS through a service like Twilio.
* With other IoT integrations, Misty could control your smart home devices.
* Misty could be a personal assistant who can review your schedule and set reminders.

## Contribution Guidelines

If you'd like to extend the templates, work on open issues, improve the documentation, add examples built on other services, or have ideas for other ways to advance the project, please feel free to submit a pull request.

To learn more about the technologies used in this project, check out these resources:

* [Misty II JavaScript SDK docs](https://docs.mistyrobotics.com/misty-ii/javascript-sdk/javascript-skill-architecture/)
* [Misty II .NET SDK (Beta) docs](https://docs.mistyrobotics.com/misty-ii/net-sdk/overview)
* [Dialogflow docs](https://cloud.google.com/dialogflow/docs/)
* [Foursquare Places API docs](https://developer.foursquare.com/docs/api)

New to contributing to open source? Here's how you can get started with this project.

* [Fork](https://guides.github.com/activities/forking/) or [clone](https://help.github.com/en/github/creating-cloning-and-archiving-repositories/cloning-a-repository) this repository. (Forking creates a copy of the repository under your account, and cloning without forking creates a remote copy of this repository on your local machine.)
* Create a branch to work on your changes: `git checkout -b <branch-name>`
* Test your changes. When you're ready, submit a pull request [from your fork](https://help.github.com/en/github/collaborating-with-issues-and-pull-requests/creating-a-pull-request-from-a-fork) or [remote branch](https://help.github.com/en/github/collaborating-with-issues-and-pull-requests/creating-a-pull-request). It's best to submit your PR against the `develop` branch, in case there are other changes that should be made before the code is merged into the `master` branch. Include a description of your changes. If your PR relates to an open issue, be sure to reference the issue number in the description or title of your PR.
* Wait for your changes to be reviewed, and work with the community to get it approved!

If you have questions about contributing to this project, please share them in the [Misty Community Forums](https://community.mistyrobotics.com/t/misty-application-template-misty-concierge-template/2414). We're excited to see the project grow!