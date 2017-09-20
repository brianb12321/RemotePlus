using System;
using System.Runtime.Serialization;

namespace RemotePlusLibrary
{
    [Serializable]
    [DataContract]
    public class UserCredentials
    {
        [DataMember]
        public string Username { get; set; }
        [DataMember]
        public string Password { get; set; }
        public UserCredentials(string username, string password)
        {
            Username = username;
            Password = password;
        }
        public UserCredentials()
        {
            Username = "";
            Password = "";
        }
    }
}