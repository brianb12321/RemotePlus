using RemotePlusLibrary.Core.IOC;
using RemotePlusLibrary.Extension.CommandSystem;
using RemotePlusLibrary.Extension.CommandSystem.CommandClasses;
using Ninject;
using System.Runtime.Serialization;
using System.Xml;
using System.IO;
using System;
using RemotePlusServer.Core;
using System.Drawing;
using System.Collections.Generic;
using RemotePlusLibrary.ServiceArchitecture;

namespace RSPM
{
    public class PackageCommands : ICommandClass
    {
        IRemotePlusService<ServerRemoteInterface> _service;
        public PackageCommands(IRemotePlusService<ServerRemoteInterface> service)
        {
            _service = service;
        }
        public Dictionary<string, CommandDelegate> Commands { get; } = new Dictionary<string, CommandDelegate>();

        [CommandHelp("Installs a package from the internet.")]
        public CommandResponse InstallPackage(CommandRequest req, CommandPipeline pipe, ICommandEnvironment currentEnvironment)
        {
            try
            {
                IPackageManager manager = IOCContainer.GetService<IPackageManager>();
                manager.LoadPackageSources();
                manager.InstallPackage(req.Arguments[1].Value.ToString());
                return new CommandResponse((int)CommandStatus.Success);
            }
            catch (Exception ex)
            {
                _service.RemoteInterface.Client.ClientCallback.TellMessageToServerConsole(new ConsoleText($"An error occurred during installation: {ex.Message}") { TextColor = Color.Red });
                return new CommandResponse((int)CommandStatus.Fail);
            }
        }
        [CommandHelp("Generates a package manifest file for you.")]
        public CommandResponse GeneratePackageManifest(CommandRequest req, CommandPipeline pipe, ICommandEnvironment currentEnvironment)
        {
            DataContractSerializer xsSubmit = new DataContractSerializer(typeof(PackageDescription));
            var subReq = new PackageDescription();
            subReq.Description = "Sample description.";
            subReq.Name = "Sample Package";
            subReq.Extensions.Add("testExtension.dll");
            subReq.Extensions.Add("awesomeExtension.dll");
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

        public void AddCommands()
        {
            Commands.Add("Install-Package", InstallPackage);
            Commands.Add("Generate-Package-Manifest", GeneratePackageManifest);
        }

        public CommandDelegate Lookup(string commandName)
        {
            if(Commands.TryGetValue(commandName, out CommandDelegate command))
            {
                return command;
            }
            else
            {
                return null;
            }
        }

        public bool HasCommand(string commandName)
        {
            return Commands.ContainsKey(commandName);
        }
    }
}