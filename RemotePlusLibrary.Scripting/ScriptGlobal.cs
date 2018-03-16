using RemotePlusLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.Scripting
{
    public class ScriptGlobal
    {
        public ScriptGlobalInformation Information { get; set; }
        public object Global { get; set; }
        public ScriptGlobal(object g)
        {
            Global = g;
        }
    }
}
