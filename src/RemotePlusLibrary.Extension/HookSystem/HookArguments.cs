using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.Extension.HookSystem
{
    /// <summary>
    /// Provides additional information used in a server hook;
    /// </summary>
    public class HookArguments
    {
        public string HookName { get; set; }
        public Dictionary<string, string> HookData { get; set; }
        public HookArguments(string name)
        {
            HookData = new Dictionary<string, string>();
            HookName = name;
        }
    }
}
