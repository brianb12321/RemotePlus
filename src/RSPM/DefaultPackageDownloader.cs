using RemotePlusLibrary.ServiceArchitecture;
using RemotePlusLibrary.SubSystem.Command;
using RemotePlusServer.Core;
using System;
using System.Drawing;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using RemotePlusLibrary.Core;
using RemotePlusServer;
using static RemotePlusServer.Core.ServerManager;

namespace RSPM
{
    public class DefaultPackageDownloader : IPackageDownloader
    {
        public IClientContext ClientContext { get; set; }
        IRemotePlusService<ServerRemoteInterface> _service = null;
        public DefaultPackageDownloader(IRemotePlusService<ServerRemoteInterface> service)
        {
            _service = service;
        }
        public bool DownlaodPackage(string packageName, Uri[] sources)
        {
            var client = ClientContext.GetClient<RemoteClient>();
            bool foundPackage = false;
            WebClient downloader = new WebClient();
            downloader.DownloadProgressChanged += Downloader_DownloadProgressChanged;
            foreach (Uri addressToCheck in sources)
            {
                client.ClientCallback.TellMessageToServerConsole($"Package Get: {addressToCheck}");
                try
                {
                    client.ClientCallback.RequestInformation(new RemotePlusLibrary.RequestSystem.DefaultRequestBuilders.ProgressRequestBuilder()
                    {
                        Message = "Attempting to download package."
                    });
                    Task t = downloader.DownloadFileTaskAsync(new Uri($"{addressToCheck}/{packageName}.pkg"), $"{packageName}.pkg");
                    t.Wait();
                    foundPackage = true;
                    break;
                }
                catch (AggregateException ex)
                {
                    client.ClientCallback.DisposeCurrentRequest();
                    client.ClientCallback.TellMessageToServerConsole(new ConsoleText($"Unable to download: {ex.GetBaseException().Message}") { TextColor = Color.Red });
                }
            }
            if(foundPackage)
            {
                client.ClientCallback.TellMessageToServerConsole("Finished downloaded package.");
            }
            client.ClientCallback.DisposeCurrentRequest();
            return foundPackage;
        }

        private void Downloader_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            var client = ClientContext.GetClient<RemoteClient>();
            client.ClientCallback.UpdateRequest(new RemotePlusLibrary.RequestSystem.DefaultUpdateRequestBuilders.ProgressUpdateBuilder(e.ProgressPercentage));
        }
    }
}