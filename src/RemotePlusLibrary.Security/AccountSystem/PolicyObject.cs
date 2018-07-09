using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using RemotePlusLibrary.Configuration;

namespace RemotePlusLibrary.Security.AccountSystem
{
    /// <summary>
    /// Stores policy settings. Attach policy objects to roles to apply settings.
    /// </summary>
    [Serializable]
    public class PolicyObject
    {
        public SecurityPolicyFolder Policies { get; set; } = new SecurityPolicyFolder() { Name = "Root" };
        public string ObjectName { get; set; }
        public PolicyObject(string objName)
        {
            ObjectName = objName;
        }
    }
}