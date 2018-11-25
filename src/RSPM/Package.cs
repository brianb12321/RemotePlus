using BetterLogger;
using Ionic.Zip;
using RemotePlusLibrary.Core;
using RemotePlusServer.Core.ExtensionSystem;
using System;
using System.IO;

namespace RSPM
{
    public class Package
    {
        public string Path { get; }
        public ZipFile Zip { get; }
        public PackageDescription Description { get; }
        public Package(string fileName, PackageDescription desc, ZipFile zip)
        {
            Path = fileName;
            Description = desc;
            Zip = zip;
        }
        public void ExtractWithoutManifest(string location)
        {
            Zip.ExtractExistingFile = ExtractExistingFileAction.DoNotOverwrite;
            Zip.ExtractAll(location);
            File.Delete($"{location}\\package.manifest");
        }
        public void LoadPackageExtensions(string location, RemotePlusLibrary.Extension.ExtensionLoader.ExtensionLibraryCollectionBase<ServerExtensionLibrary> collection)
        {
            var env = new RemotePlusServer.Core.ExtensionSystem.ServerInitEnvironment(false);
            foreach (string extensions in Description.Extensions)
            {
                try
                {
                    collection.LoadExtension($"{location}\\{extensions}", env);
                }
                catch (Exception ex)
                {
                    GlobalServices.Logger.Log($"Unable to load extension library from package: {ex.Message}", LogLevel.Error);
                }
            }
        }
    }
}