using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary
{
    [Serializable]
    [DataContract]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class UserAccount
    {
        [DataMember]
        [Browsable(true)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public UserCredentials Credentials { get; set; }
        [DataMember]
        [Browsable(true)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public Role Role { get; set; }
        public UserAccount(UserCredentials _cred, Role _role)
        {
            Credentials = _cred;
            Role = _role;
        }
        public UserAccount()
        {
            Credentials = new UserCredentials("NewUser", "");
            Role = new Role("newRole", new SecurityAccessRules());
        }
        public bool Verify(UserCredentials TestCred)
        {
            if(TestCred.Username == Credentials.Username && (TestCred.Password == Credentials.Password))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
