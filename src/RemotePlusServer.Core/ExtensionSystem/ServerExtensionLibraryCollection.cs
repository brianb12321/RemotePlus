using BetterLogger;
using RemotePlusLibrary;
using RemotePlusLibrary.Extension;
using RemotePlusLibrary.Extension.ExtensionLoader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusServer.Core.ExtensionSystem
{
    public class ServerExtensionLibraryCollection : ExtensionLibraryCollectionBase<ServerExtensionLibrary, string>
    {
        public override Dictionary<string, string> GetAllExtensions()
        {
            throw new NotImplementedException();
        }
        public void LoadExtensionsInFolder()
        {
            List<string> excludedFiles = new List<string>();
            GlobalServices.Logger.Log("Loading extensions...", LogLevel.Info);
            if (Directory.Exists("extensions"))
            {
                if (File.Exists("extensions\\excludes.txt"))
                {
                    GlobalServices.Logger.Log("Found an excludes.txt file. Reading file...", LogLevel.Info);
                    foreach (string excludedFile in File.ReadLines("extensions\\excludes.txt"))
                    {
                        GlobalServices.Logger.Log($"{excludedFile} is excluded from the extension search.", LogLevel.Info);
                        excludedFiles.Add("extensions\\" + excludedFile);
                    }
                    GlobalServices.Logger.Log("Finished reading extension exclusion file.", LogLevel.Info);
                }
                ServerInitEnvironment env = new ServerInitEnvironment(false);
                foreach (string files in Directory.GetFiles("extensions"))
                {
                    if (Path.GetExtension(files) == ".dll" && !excludedFiles.Contains(files))
                    {
                        try
                        {
                            GlobalServices.Logger.Log($"Found extension file ({Path.GetFileName(files)})", LogLevel.Info);
                            env.PreviousError = GlobalServices.Logger.ErrorCount > 0 ? true : false;
                            var lib = ServerExtensionLibrary.LoadServerLibrary(files, (m, o) => GlobalServices.Logger.Log(m, o), env);
                            Libraries.Add(lib.Name, lib);
                        }
                        catch (Exception ex)
                        {
                            GlobalServices.Logger.Log($"Could not load \"{files}\" because of a load error or initialization error. Error: {ex.Message}", LogLevel.Warning);
                        }
                        env.InitPosition++;
                    }
                }
                GlobalServices.Logger.Log($"{ServerManager.DefaultCollection.Libraries.Count} extension libraries loaded.", LogLevel.Info);
            }
            else
            {
                GlobalServices.Logger.Log("The extensions folder does not exist.", LogLevel.Info);
            }
        }
    }
}
