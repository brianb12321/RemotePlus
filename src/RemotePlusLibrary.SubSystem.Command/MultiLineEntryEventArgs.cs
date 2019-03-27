using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.SubSystem.Command
{
    public class MultiLineEntryEventArgs : EventArgs
    {
        public string ReceivedValue { get; set; }
        public char Prelude { get; private set; }
        public MultiLineEntryEventArgs(char prelude)
        {
            Prelude = prelude;
        }
    }
}