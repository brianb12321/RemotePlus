using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.Configuration.ServerSettings
{
    [DataContract]
    public class SecuritySettings
    {
        [DataMember]
        public bool Enabled { get; set; }
        [DataMember]
        public string CertificateFilePath { get; set; }
        [DataMember]
        public string CertificatePassword { get; set; }
    }
}