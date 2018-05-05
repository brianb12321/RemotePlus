using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.Scripting.ScriptPackageEngine
{
    public static class ScriptPackageExtensions
    {
        public static bool ExecuteScript(this ScriptPackage package)
        {
            Stream s = package.PackageContents.GetEntry(package.PackageManifest.ScriptEntryPoint).Open();
            
        }
    }
}
