using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.Editors.PolicyManagerClasses
{
    public class PolicyView
    {
        public string Type { get; set; }
        public AccountSystem.SecurityPolicy Policy { get; set; }
        public PolicyView(AccountSystem.SecurityPolicy p, string t)
        {
            Policy = p;
            Type = t;
        }
    }
}
