using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using RemotePlusLibrary.Security.AccountSystem;

namespace RemotePlusLibrary
{
    [DataContract]
    public class RegisterationObject
    {
        public RegisterationObject()
        {
            Credentials = new UserCredentials("username", "password");
        }
        [DataMember]
        public bool VerboseError { get; set; }
        [DataMember]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public UserCredentials Credentials { get; set; }
        [DataMember]
        public bool LoginRightAway { get; set; }
    }
}
