using RemotePlusLibrary.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Ninject;

namespace RemotePlusLibrary.Security.AccountSystem
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
        /// <summary>
        /// Provides a unique identification number used to identify unique accounts.
        /// </summary>
        [DataMember]
        public Guid AID { get; private set; }
        [DataMember]
        [Browsable(true)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public Role Role { get; set; }
        public UserAccount(UserCredentials _cred, string _role)
        {
            Credentials = _cred;
            JoinRole(_role);
            AID = Guid.NewGuid();
        }
        public void JoinRole(string roleName)
        {
            if(Role != null)
            {
                throw new RoleException("You must first leave the current role before joining a new role.");
            }
            var r = Role.GetRole(roleName);
            r.Members.Add(AID);
            this.Role = r;
        }
        public void LeaveRole()
        {
            Role = null;
            Role.GlobalPool.DeregisterMember(AID);
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
