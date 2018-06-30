using Microsoft.Scripting;
using RemotePlusLibrary.Scripting.ScriptPackageEngine;
using RemotePlusServer.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusServer.Core
{
    public static class ScriptPackageExtensions
    {
        public static bool ExecuteScript(this ScriptPackage package)
        {
            Guid g = Guid.NewGuid();
            ServerCore.ServerBuilderExtensions.InitializeGlobals();
            package.PackageContents.ExtractAll(g.ToString());
            try
            {
                string tempPackagePath = $"{g}\\{package.PackageFileName}";
                using (StreamReader sr = new StreamReader($"{tempPackagePath}\\{package.PackageManifest.ScriptEntryPoint}"))
                {
                    var paths = ServerManager.ScriptBuilder.ScriptingEngine.GetSearchPaths();
                    paths.Add(tempPackagePath);
                    ServerManager.ScriptBuilder.ScriptingEngine.SetSearchPaths(paths);
                    var result = ServerManager.ScriptBuilder.ExecuteString(sr.ReadToEnd());
                    ServerManager.ScriptBuilder.InitializeEngine();
                    Directory.Delete(g.ToString(), true);
                    return result;
                }
            }
            finally
            {
                ServerManager.ScriptBuilder.InitializeEngine();
                Directory.Delete(g.ToString(), true);
            }
        }
    }
}
