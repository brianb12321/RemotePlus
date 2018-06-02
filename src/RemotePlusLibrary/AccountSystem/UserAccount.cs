using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.AccountSystem
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
        public void JoinRole(string roleName)
        {
            var r = Role.GetRole(roleName);
            r.Members.Add(this);
            this.Role = r;
        }
        public UserAccount()
        {
            Role = Role.Empty;
            Credentials = new UserCredentials("admin", "password");
            this.JoinRole("Administrators");
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
