using RemotePlusLibrary.Extension.CommandSystem;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static RemotePlusServer.Core.ServerManager;

namespace RSPM
{
    public class DefaultPackageDownloader : IPackageDownloader
    {
        public bool DownlaodPackage(string packageName, Uri[] sources)
        {
            bool foundPackage = false;
            WebClient downloader = new WebClient();
            foreach (Uri addressToCheck in sources)
            {
                ServerRemoteService.RemoteInterface.Client.ClientCallback.TellMessageToServerConsole($"Package Get: {addressToCheck}");
                try
                {
                    downloader.DownloadFile($"{addressToCheck}/{packageName}.pkg", $"{packageName}.pkg");
                    foundPackage = true;
                    break;
                }
                catch (Exception ex)
                {
                    ServerRemoteService.RemoteInterface.Client.ClientCallback.TellMessageToServerConsole(new ConsoleText($"Unable to download: {ex.Message}") { TextColor = Color.Red });
                }
            }
            return foundPackage;
        }
    }
}
