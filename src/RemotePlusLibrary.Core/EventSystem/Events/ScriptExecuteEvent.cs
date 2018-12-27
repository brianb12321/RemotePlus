using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.Core.EventSystem.Events
{
    [DataContract]
    public class ScriptExecuteEvent : TinyMessenger.TinyMessageBase
    {
        public ScriptExecuteEvent(object sender) : base(sender)
        {
        }
    }
}