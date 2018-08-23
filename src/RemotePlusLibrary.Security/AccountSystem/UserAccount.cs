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
        public UserAccount(UserCredentials _cred)
        {
            Credentials = _cred;
            AID = Guid.NewGuid();
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
