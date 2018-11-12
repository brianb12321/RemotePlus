using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.Core.EventSystem.Events
{
    public class ScriptExecuteEvent : TinyMessenger.TinyMessageBase
    {
        public ScriptExecuteEvent(object sender) : base(sender)
        {
        }
    }
}