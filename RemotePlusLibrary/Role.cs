using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary
{
    [DataContract]
    public class Role
    {
        public Role()
        {
            RoleName = "NewRole";
            Privilleges = new SecurityAccessRules();
        }
        public Role(string roleName, SecurityAccessRules priv)
        {
            RoleName = roleName;
            this.Privilleges = priv;
        }
        [DataMember]
        public string RoleName { get; set; }
        [DataMember]
        [Category("Security")]
        [Browsable(true)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public SecurityAccessRules Privilleges { get; set; }
        public override string ToString()
        {
            return RoleName;
        }
    }
}
