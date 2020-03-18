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
                Key = "FRIEND_NAME1", FirstName = "First Name 1",   LastName = "Last Name 1", CellPhoneNumber = "+19998887777",
                Greetings = new List<string>() {
                    "Hello <Friend Name>, you look amazing today.  Please come in and relax in the living room.",
                    "<Friend Name>, <add a 2nd greeting message here>.",
                    "<Friend Name>, <add a 3rd greeting message here>."
                }
            },
            new FriendInfo {
                Key = "FRIEND_NAME2", FirstName = "First Name 2",   LastName = "Last Name 2", CellPhoneNumber = "+19998887777",
                Greetings = new List<string>() {
                    "Hello <Friend Name>, you look amazing today.  Please come in and relax in the living room."
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
