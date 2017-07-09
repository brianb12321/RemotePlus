using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.Core.EmailService
{
    [DataContract]
    public class AdvancedEmailSettings
    {
        public bool ApplyTimeStamp { get; set; }
    }
}
