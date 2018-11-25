using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.Core.EventSystem.Events
{
    public class ServiceFaultErrorEvent : TinyMessenger.TinyMessageBase
    {
        public Exception FaultException { get; set; }
        public ServiceFaultErrorEvent(object sender, Exception error) : base(sender)
        {
            FaultException = error;
        }
    }
}
