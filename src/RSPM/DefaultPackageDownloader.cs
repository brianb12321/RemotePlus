using RemotePlusLibrary.Extension.CommandSystem;
using RemotePlusLibrary.ServiceArchitecture;
using RemotePlusServer.Core;
using System;
using System.Drawing;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using static RemotePlusServer.Core.ServerManager;

namespace RSPM
{
    public class DefaultPackageDownloader : IPackageDownloader
    {
        IRemotePlusService<ServerRemoteInterface> _service = null;
        public DefaultPackageDownloader(IRemotePlusService<ServerRemoteInterface> service)
        {
            _service = service;
        }
        public bool DownlaodPackage(string packageName, Uri[] sources)
        {
            bool foundPackage = false;
            WebClient downloader = new WebClient();
            downloader.DownloadProgressChanged += Downloader_DownloadProgressChanged;
            foreach (Uri addressToCheck in sources)
            {
                ServerRemoteService.RemoteInterface.Client.ClientCallback.TellMessageToServerConsole($"Package Get: {addressToCheck}");
                try
                {
                    _service.RemoteInterface.Client.ClientCallback.RequestInformation(new RemotePlusLibrary.RequestSystem.DefaultRequestBuilders.ProgressRequestBuilder()
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
                    _service.RemoteInterface.Client.ClientCallback.DisposeCurrentRequest();
                    ServerRemoteService.RemoteInterface.Client.ClientCallback.TellMessageToServerConsole(new ConsoleText($"Unable to download: {ex.GetBaseException().Message}") { TextColor = Color.Red });
                }
            }
            if(foundPackage)
            {
                _service.RemoteInterface.Client.ClientCallback.TellMessageToServerConsole("Finished downloaded package.");
            }
            _service.RemoteInterface.Client.ClientCallback.DisposeCurrentRequest();
            return foundPackage;
        }

        private void Downloader_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            _service.RemoteInterface.Client.ClientCallback.UpdateRequest(new RemotePlusLibrary.RequestSystem.DefaultUpdateRequestBuilders.ProgressUpdateBuilder(e.ProgressPercentage));
        }
    }
}