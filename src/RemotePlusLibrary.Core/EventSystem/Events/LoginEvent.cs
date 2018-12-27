using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.Core.EventSystem.Events
{
    /// <summary>
    /// Gets raised when a user attempts to login.
    /// </summary>
    [DataContract]
    public class LoginEvent : TinyMessenger.TinyMessageBase
    {
        [DataMember]
        public bool LoginSuccessful { get; private set; }
        public LoginEvent(bool loginSuccess, object sender) : base(sender)
        {
            LoginSuccessful = loginSuccess;
        }
    }
}