﻿using RemotePlusLibrary.Core.IOC;
using RemotePlusLibrary.Extension.CommandSystem;
using RemotePlusLibrary.Extension.CommandSystem.CommandClasses;
using Ninject;
using System.Runtime.Serialization;
using System.Xml;
using System.IO;
using System;
using RemotePlusServer.Core;
using System.Drawing;

namespace RSPM
{
    public static class PackageCommands
    {

        [CommandHelp("Installs a package from the internet.")]
        public static CommandResponse InstallPackage(CommandRequest req, CommandPipeline pipe)
        {
            try
            {
                IPackageManager manager = IOCContainer.GetService<IPackageManager>();
                manager.LoadPackageSources();
                manager.InstallPackage(req.Arguments[1].Value);
                return new CommandResponse((int)CommandStatus.Success);
            }
            catch (Exception ex)
            {
                ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.TellMessageToServerConsole(new ConsoleText($"An error occurred during installation: {ex.Message}") { TextColor = Color.Red });
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
    }
}