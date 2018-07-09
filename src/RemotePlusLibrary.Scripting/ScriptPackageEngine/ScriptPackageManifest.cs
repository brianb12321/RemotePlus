using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.Scripting.ScriptPackageEngine
{
    [DataContract]
    public class ScriptPackageManifest
    {
        [DataMember(Name = "PackageName")]
        public string PackageName { get; set; }
        [DataMember(Name = "EntrypPoint", IsRequired = true)]
        public string ScriptEntryPoint { get; set; }
    }
    public static class ScriptPakcageManifestExtensions
    {
        public static void GenerateManifestToFile(this ScriptPackageManifest manifest, string fileName)
        {
            new Configuration.StandordDataAccess.ConfigurationHelper().SaveConfig(manifest, fileName);
        }
    }
}