using System;
using System.Collections.Generic;

namespace Misty.Services
{
    /// <summary>
    /// This class contains information about Misty's friends.  It stored based information about Misty's friends (e.g., first name, last name, cell phone number, etc.) 
    /// and personal greetings for each friend.
    /// </summary>
    public class MistyFriends
    {
        public class FriendInfo
        {
            public string Key { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string CellPhoneNumber { get; set; }
            public List<string> Greetings { get; set; }
            public string FullName => $"{FirstName} {LastName}";
        }

        public static List<FriendInfo> friends = new List<FriendInfo>
        {
            new FriendInfo {
                Key = "UNKNOWN_PERSON", FirstName = "unknown_person",   LastName = "unknown", CellPhoneNumber = "",
                Greetings = new List<string>() { "Hello.  Welcome to our home.  Please come in and relax in the living room." }
            },
            new FriendInfo {
                Key = "DWAYNE", FirstName = "Dwayne",   LastName = "Lorette", CellPhoneNumber = "+15149232451",
                Greetings = new List<string>() {
                    "Hello Duane, you look amazing today.  Please come in and relax in the living room."
                }
            },
            new FriendInfo {
                Key = "DUANE", FirstName = "Duane",   LastName = "Lorette", CellPhoneNumber = "+15149232451",
                Greetings = new List<string>() {
                    "Hello Duane, you look amazing today.  Please come in and relax in the living room."
                }
            },
            new FriendInfo {
                Key = "LINDA", FirstName = "Linda",   LastName = "Speerbrecker", CellPhoneNumber = "+17788399949",
                Greetings = new List<string>() {
                    "Hello Linda, you look amazing today.  Please come in and relax in the living room.  What would you like to drink?",
                    "Linda ... Did you just come back from Arizona and Palm Springs?  I have great memories of those places.",
                    "Linda ... are you looking forward to going on your hawaiian cruise?  I like cruising around the islands better than seeing only one of the islands.",
                }
            },
            new FriendInfo {
                Key = "WES", FirstName = "Wes",   LastName = "Speerbrecker", CellPhoneNumber = "+16049164558",
                Greetings = new List<string>() {
                    "Hello Wes, you look very handsome today.  Please come in and relax in the living room.",
                    "Wes ... would you like a special coffee or a spiced rum to drink?",
                    "Wes ... you look fantastic today.  Have you been working out on your Bowflex equipment?",
                }
            },
            new FriendInfo {
                Key = "NANCY", FirstName = "Nancy",   LastName = "Pyke", CellPhoneNumber = "",
                Greetings = new List<string>() {
                    "Hello Nancy, you look fabulous today.  Please come in and relax in the living room.  Would you like some tea or milk chocolate?"
                }
            },
            new FriendInfo {
                Key = "GERRY", FirstName = "Gerry",   LastName = "Pyke", CellPhoneNumber = "",
                Greetings = new List<string>() {
                    "Hello Gerry, you look fantastic today.  Please come in and relax in the living room.  Would you like a Pepsi Zero or Coke Max?  I mean Pepsi Max and Coke Zero - ha ha"
                }
            },
            new FriendInfo {
                Key = "JOHN", FirstName = "John",   LastName = "Van Gelder", CellPhoneNumber = "",
                Greetings = new List<string>() {
                    "Hello John, it is great to see you. Please come in and relax in the living room.  Would you like a coffee?"
                }
            },
            new FriendInfo {
                Key = "DAN", FirstName = "Dan",   LastName = "Turner", CellPhoneNumber = "+17788676910",
                Greetings = new List<string>() {
                    $"Hello Dan ... are we going to eat dinner on time tonight or is it going to be on DAN time?",
                    $"Dan ... you look great tonight!  Would you like a spiced rum or special coffee to drink?",
                    $"Dan ... I heard you like seafood -- have you tried crabbing at the barnet marine park?"
                },
            },

            new FriendInfo {
                Key = "SHERRI", FirstName = "Sherri",   LastName = "King", CellPhoneNumber = "",
                Greetings = new List<string>() {
                    "Hello Sherri, you look amazing today.  Please come in and relax in the living room.  Would you like some wine or a special coffee?"
                }
            },
            new FriendInfo {
                Key = "RANDY", FirstName = "Randy", LastName = "Lockyear", CellPhoneNumber = "",
                Greetings = new List<string>() {
                    "Hello Randy, you are looking great today.  Please come in and relax in the living room.  Would you like a beer or some skittles?"
                }
            },
            new FriendInfo {
                Key = "CARTER", FirstName = "Carter", LastName = "", CellPhoneNumber = "",
                Greetings = new List<string>() {
                    "Hello Carter-- my research tells me that you love nerf guns --- which is your favorite nerf gun?  I am working on my nerf gun skills and will challenge you to a battle some day.",
                    "Carter -- have you been playing fortinight or nhl 2020 or did your knight hawks hockey team play this weekend?",
                    "Carter -- is it true you have a crush on Reena, Sophia, and Jolene.  Wow that is quite the roster.",
                    "Carter -- I heard your favority movie is Spiderman -- Far From Home -- that is a great movie!",
                    "Carter Ames Holt -- is it true that you are going to Mexico with your mom and Jeff at spring break?",
                    "Carter -- which of your dogs do you like the best - Rafael, Frankie or Braxton?",
                }
            },
            new FriendInfo {
                Key = "ALEXIS", FirstName = "Alexa", LastName = "", CellPhoneNumber = "",
                Greetings = new List<string>() {
                    "Hello Alexis, you are looking fantastic today. Have you been jogging with your father recently in preparation for the Sun Run?",
                    "Alexis -- how are Rafael and Frankie doing today?",
                    "Alexis -- I will try to talk like you now ---- Will we be having some broc or other veg with our dinner?",
                }
            },
            new FriendInfo {
                Key = "DALLAS", FirstName = "Dallas", LastName = "", CellPhoneNumber = "",
                Greetings = new List<string>() {
                    "Hello Dallas --- your hair looks fantastic today.  How much product did that take?",
                    "Dallas --- can you do an al pachino impression for us?  I have heard it is halarious!",
                    "Dallas --- can we fire up the karaoke machine tonight?",
                }
            },
            new FriendInfo {
                Key = "GRETA", FirstName = "Greta", LastName = "Speerbrecker", CellPhoneNumber = "",
                Greetings = new List<string>() {
                    "Hello Greta, you are looking wonderful today.  Please come in and relax in the living room.  Would you like a coffee or a glass of wine?",
                    "Greta --- How you got any more painting for Linda and Wes's collection?",
                    "Greta --- how do you like your new furniture?  I have seen the photos and the furnitiure looks fabulous.  ",
                    "Greta --- how is miss tiby the cat doing?  Has she found any more mice in the kitchen recently.  ",
                }
            },
            new FriendInfo {
                Key = "RAY", FirstName = "Ray", LastName = "Speerbrecker", CellPhoneNumber = "+12508378933",
                Greetings = new List<string>() {
                    "Hello Ray, you are looking great today.  Please come in and relax in the living room.  Would you like an Odouls beer or cup of coffee?",
                    "Ray --- have you been driving your side by side off road vehicle?  it is a great vehicle to drive into the forest for picking blueberries.",
                    "Ray --- have you been driving your Tesla car recently?  May autopilot system is almost as good as your Tesla car.",
                }
            },
            new FriendInfo {
                Key = "HANY", FirstName = "Haney", LastName = "Hansen", CellPhoneNumber = "",
                Greetings = new List<string>() {
                    "Hello Uncle Hanny --- Rumour has it that you recently had a birthday.  Happy 90th Birtday - you look fantastic!",
                    "Uncle Hanny --- I heard that you love butter tarts.  They are one of my favourite treats.  Do you know when Gerry is going to make more butter tarts for us?",
                    "Uncle Hanny --- Have you been watching the vancouver canucks playing hockey recently?  They kinda suck but they did win on Friday night."
                }
            },
            new FriendInfo {
                Key = "TRACEY", FirstName = "Tracy", LastName = "Fasciana", CellPhoneNumber = "",
                Greetings = new List<string>() {
                    "Hello Tracy --- you are looking great today.  Have you been working on your hobby recently?  I think your miniture kitchen is awesome.",
                    "Tracy --- thank you for inviting us to your home today.  Your house looks amazing!",
                    "Tracy --- it looks like you have many cats and birds in your house.  I am sure that Guy wants you to get more cats.  He might be lonely - ha ha ha.",
                }
            },
            new FriendInfo {
                Key = "", FirstName = "", LastName = "", CellPhoneNumber = "",
                Greetings = new List<string>() {
                    "Hello xxx, you are looking great today.  Please come in and relax in the living room.  Would you like a beer or some skittles?"
                }
            },
            new FriendInfo {
                Key = "", FirstName = "", LastName = "", CellPhoneNumber = "",
                Greetings = new List<string>() {
                    "Hello xxx, you are looking great today.  Please come in and relax in the living room.  Would you like a beer or some skittles?"
                }
            },
        };

        /// <summary>
        /// Looks up the person in the list of Misty's friends based on the label key from Misty's facial recognition service.
        /// </summary>
        /// <param name="FriendKey"></param>
        /// <returns></returns>
        public static FriendInfo LookupFriendInfo(string FriendKey)
        {
            return friends.Find(item => item.Key == FriendKey);
        }
    }
}
