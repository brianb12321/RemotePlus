using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.AccountSystem
{
    [DataContract]
    public class DefaultPolicy : SecurityPolicy
    {
        public DefaultPolicy(string shortName)
        {
            ShortName = shortName;
        }
    }
}
