using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using RemotePlusLibrary.Configuration;

namespace RemotePlusLibrary.Security.AccountSystem
{
    /// <summary>
    /// Contains all the roles available on the server.
    /// </summary>
    [DataContract]
    public class RolePool
    {
        [DataMember]
        public List<Role> Roles { get; set; } = new List<Role>();
        public const string ROLES_CONFIG_PATH = "Configurations\\Server\\Roles.config";
        public void DeregisterMember(Guid AID)
        {
            Roles.ForEach(r =>
            {
                var members = r.Members.Where(r2 => r2 == AID);
                foreach(var m in members)
                {
                    r.Members.Remove(m);
                }
            });
        }
    }
}