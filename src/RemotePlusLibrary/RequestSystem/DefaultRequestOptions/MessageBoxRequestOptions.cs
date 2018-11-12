using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RemotePlusLibrary.RequestSystem.DefaultRequestOptions
{
    [DataContract]
    public class MessageBoxRequestOptions
    {
        [DataMember]
        public string Message { get; set; }
        [DataMember]
        public string Caption { get; set; }
        [DataMember]
        public MessageBoxButtons Buttons { get; set; }
        [DataMember]
        public MessageBoxIcon Icons { get; set; }
    }
}
