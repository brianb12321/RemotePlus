using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusClient.Events
{
    public class ClientConnectedEvent : TinyMessenger.TinyMessageBase
    {
        public ClientConnectedEvent(object sender) : base(sender)
        {
        }
    }
}