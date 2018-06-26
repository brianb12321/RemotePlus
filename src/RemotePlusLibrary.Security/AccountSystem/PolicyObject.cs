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
    public class PolicyObject : IFileConfig
    {
        public SecurityPolicyFolder Policies { get; set; } = new SecurityPolicyFolder() { Name = "Root" };
        public string ObjectName { get; set; }
        public PolicyObject(string objName)
        {
            ObjectName = objName;
        }
        public void Load()
        {
            FileStream fsStream = new FileStream($"policyObjects\\{ObjectName}.pobj", FileMode.Open, FileAccess.Read);
            BinaryFormatter bf = new BinaryFormatter();
            PolicyObject newObj = (PolicyObject)bf.Deserialize(fsStream);
            this.Policies = newObj.Policies;
            fsStream.Flush();
            fsStream.Close();
        }

        public void Load(string fileName)
        {
            throw new NotImplementedException();
        }

        public void Save()
        {
            FileStream fsStream = new FileStream($"policyObjects\\{ObjectName}.pobj", FileMode.Create, FileAccess.ReadWrite);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fsStream, this);
            fsStream.Flush();
            fsStream.Close();
        }

        public void Save(string fileName)
        {
            FileStream fsStream = new FileStream(fileName, FileMode.Create, FileAccess.ReadWrite);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fsStream, this);
            fsStream.Flush();
            fsStream.Close();
        }
        public static PolicyObject Open(string fileName)
        {
            FileStream fsStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            BinaryFormatter bf = new BinaryFormatter();
            PolicyObject newObj = (PolicyObject)bf.Deserialize(fsStream);
            fsStream.Flush();
            fsStream.Close();
            return newObj;
        }
    }
}
