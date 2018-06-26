using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RemotePlusLibrary.Security.AccountSystem;

namespace PolicyEditor
{
    public class PolicyView
    {
        public string Type { get; set; }
        public SecurityPolicy Policy { get; set; }
        public PolicyView(SecurityPolicy p, string t)
        {
            Policy = p;
            Type = t;
        }
    }
}
