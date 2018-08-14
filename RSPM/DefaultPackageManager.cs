using RemotePlusLibrary.Extension.CommandSystem;
using RemotePlusServer.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static RemotePlusServer.Core.ServerManager;

namespace RSPM
{
    public class DefaultPackageManager : IPackageManager
    {
        public List<Uri> Sources { get; private set; } = new List<Uri>();

        public void InstallPackage(string packageName)
        {
            ServerRemoteService.RemoteInterface.Client.ClientCallback.TellMessageToServerConsole($"Beginning installation of {packageName}");
            var package = downloadPackage(packageName);
            if(package != null)
            {
                ServerRemoteService.RemoteInterface.Client.ClientCallback.TellMessageToServerConsole("Extracting package.");
                package.Zip.ExtractAll("extensions");
                ServerRemoteService.RemoteInterface.Client.ClientCallback.TellMessageToServerConsole("Finished extracting package. Deleting downloaded package.");
                package.Zip.Dispose();
                File.Delete($"{packageName}.pkg");
                ServerRemoteService.RemoteInterface.Client.ClientCallback.TellMessageToServerConsole("Finished installing package.");
            }
        }

        private Package downloadPackage(string packageName)
        {
            WebClient downloader = new WebClient();
            foreach (Uri addressToCheck in Sources)
            {
                ServerRemoteService.RemoteInterface.Client.ClientCallback.TellMessageToServerConsole($"Package Get: {addressToCheck}");
                try
                {
                    downloader.DownloadFile($"{addressToCheck}/{packageName}.pkg", $"{packageName}.pkg");
                    IPackageReader reader = new DefaultPackageReader();
                    ServerRemoteService.RemoteInterface.Client.ClientCallback.TellMessageToServerConsole("Reading package file (pkg)");
                    return reader.BuildPackage($"{packageName}.pkg");
                }
                catch (Exception ex)
                {
                    ServerRemoteService.RemoteInterface.Client.ClientCallback.TellMessageToServerConsole(new ConsoleText($"Unable download: {ex.Message}") { TextColor = Color.Red });
                }
            }
            return null;
        }

        public void LoadPackageSources()
        {
            ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.TellMessageToServerConsole("Reading sources.");
            try
            {
                StreamReader fileReader = new StreamReader("Configurations\\Server\\sources.list");
                while(!fileReader.EndOfStream)
                {
                    string url = fileReader.ReadLine();
                    if(Uri.TryCreate(url, UriKind.Absolute, out Uri parsedUri))
                    {
                        ServerRemoteService.RemoteInterface.Client.ClientCallback.TellMessageToServerConsole($"Source: {parsedUri.ToString()}");
                        Sources.Add(parsedUri);
                    }
                }
                fileReader.Close();
            }
            catch (FileNotFoundException)
            {
                ServerRemoteService.RemoteInterface.Client.ClientCallback.TellMessageToServerConsole("The sources list does not exist. Creating new list.");
                File.Create("Configurations\\Server\\sources.list");
            }
        }
    }
}