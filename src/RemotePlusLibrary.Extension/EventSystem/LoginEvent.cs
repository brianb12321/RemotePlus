using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.Extension.EventSystem
{
    /// <summary>
    /// Gets raised when a user attempts to login.
    /// </summary>
    public class LoginEvent : TinyMessenger.TinyMessageBase
    {
        public bool LoginSuccessful { get; private set; }
        public LoginEvent(bool loginSuccess, object sender) : base(sender)
        {
            LoginSuccessful = loginSuccess;
        }
    }
}