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
    public class RolePool : IFileConfig
    {
        [DataMember]
        public List<Role> Roles { get; set; } = new List<Role>();
        const string ROLES_CONFIG_PATH = "Configurations\\Server\\Roles.config";
        public void Load()
        {
            var rolePool = ConfigurationHelper<RolePool>.LoadConfig(ROLES_CONFIG_PATH, Core.DefaultKnownTypeManager.GetKnownTypes(null));
            Roles = rolePool.Roles;
        }

        public void Load(string fileName)
        {

        }

        public void Save()
        {
            ConfigurationHelper<RolePool>.SaveConfig(this, ROLES_CONFIG_PATH, RemotePlusLibrary.Core.DefaultKnownTypeManager.GetKnownTypes(null));
        }

        public void Save(string fileName)
        {
            throw new NotImplementedException();
        }
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