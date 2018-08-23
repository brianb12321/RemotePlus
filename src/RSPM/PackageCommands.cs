using RemotePlusLibrary.Core.IOC;
using RemotePlusLibrary.Extension.CommandSystem;
using RemotePlusLibrary.Extension.CommandSystem.CommandClasses;
using Ninject;
using System.Runtime.Serialization;
using System.Xml;
using System.IO;

namespace RSPM
{
    public static class PackageCommands
    {

        [CommandHelp("Installs a package from the internet.")]
        public static CommandResponse InstallPackage(CommandRequest req, CommandPipeline pipe)
        {
            try
            {
                IPackageManager manager = IOCContainer.Provider.Get<IPackageManager>(new Ninject.Parameters.ConstructorArgument("downloader", IOCContainer.Provider.Get<IPackageDownloader>()));
                manager.LoadPackageSources();
                manager.InstallPackage(req.Arguments[1].Value);
                return new CommandResponse((int)CommandStatus.Success);
            }
            catch
            {
                return new CommandResponse((int)CommandStatus.Fail);
            }
        }
        [CommandHelp("Generates a package manifest file for you.")]
        public static CommandResponse GeneratePackageManifest(CommandRequest req, CommandPipeline pipe)
        {
            DataContractSerializer xsSubmit = new DataContractSerializer(typeof(PackageDescription));
            var subReq = new PackageDescription();
            subReq.Description = "Sample description.";
            subReq.Name = "Sample Package";
            XmlWriterSettings settings = new XmlWriterSettings()
            {
                Indent = true,
                NamespaceHandling = NamespaceHandling.OmitDuplicates
            };
            using (var sww = new StringWriter())
            using (XmlWriter writer = XmlWriter.Create("package.manifest", settings))
            {
                xsSubmit.WriteObject(writer, subReq);
            }
            return new CommandResponse((int)CommandStatus.Success);
        }
    }
}