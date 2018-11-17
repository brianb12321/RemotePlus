using RemotePlusLibrary.RequestSystem.DefaultRequestBuilders.BaseBuilders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.RequestSystem.DefaultRequestBuilders
{
    [DataContract]
    public class ConsoleMenuRequestBuilder : SimpleMenuRequestBuilder
    {
        public ConsoleMenuRequestBuilder() : base("rcmd_smenu")
        {
        }
    }
}
