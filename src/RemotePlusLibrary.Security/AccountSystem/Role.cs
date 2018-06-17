using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.Security.AccountSystem
{
    [DataContract]
    [Serializable]
    public class Role
    {
        #region Members
        public static Role Empty = new Role();
        [Browsable(false)]
        [DataMember]
        public List<Guid> Members { get; private set; } = new List<Guid>();
        public static RolePool GlobalPool { get; set; } = new RolePool();
        [DataMember]
        public string RoleName { get; set; }
        [DataMember]
        public string PolicyObjectName { get; set; }
        [IgnoreDataMember]
        [Browsable(false)]
        public PolicyObject AttachedPolicyObject { get; set; }
        [IgnoreDataMember]
        public static string[] RoleNames { get; set; }
        #endregion
        private Role()
        {
            RoleName = "NewRole";
            PolicyObjectName = "Admin";
        }
        private Role(string roleName, string priv)
        {
            RoleName = roleName;
            this.PolicyObjectName = priv;
        }

        public void BuildPolicyObject()
        {
            PolicyObject po = new PolicyObject(PolicyObjectName);
            po.Load();
            AttachedPolicyObject = po;
        }

        public static Role GetRole(string roleName)
        {
            try
            {
                var role = GlobalPool.Roles.First(t => t.RoleName.ToLower() == roleName.ToLower());
                role.BuildPolicyObject();
                return role;
            }
            catch (ArgumentNullException)
            {
                throw new RoleException("Role does not exist.");
            }
        }
        public static void InitializeRolePool()
        {
            GlobalPool = new RolePool();
        }
        public static Role CreateRole(string roleName)
        {
            return new Role(roleName, "Admin");
        }

        public override string ToString()
        {
            return RoleName;
        }
    }
}
