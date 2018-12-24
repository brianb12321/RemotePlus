using RemotePlusLibrary.Extension.CommandSystem;
using System;
using System.Net;
using RemotePlusLibrary.Extension.CommandSystem.CommandClasses;
using RemotePlusServer.Core;
using BetterLogger;
using System.IO;
using RemotePlusLibrary.ServiceArchitecture;
using System.Drawing;
using System.Threading.Tasks;
using RemotePlusLibrary.RequestSystem.DefaultRequestBuilders;
using RemotePlusLibrary.RequestSystem.DefaultUpdateRequestBuilders;

namespace CommonWebCommands
{
    public class DownloadCommands : StandordCommandClass
    {
        IRemotePlusService<ServerRemoteInterface> _service;
        public DownloadCommands(IRemotePlusService<ServerRemoteInterface> service)
        {
            _service = service;
        }

        public override void AddCommands()
        {
            Commands.Add("downloadWeb", downloadWeb);
            Commands.Add("wget", wget);
        }

        [CommandHelp("Downloads a file from the internet and displays it in the console.")]
        public CommandResponse downloadWeb(CommandRequest args, CommandPipeline pipe)
        {
            WebClient client = new WebClient();
            try
            {
                ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.TellMessageToServerConsole(client.DownloadString(args.Arguments[1].ToString()));
                return new CommandResponse((int)CommandStatus.Success);
            }
            catch (Exception ex)
            {
                ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.TellMessageToServerConsole($"Unable to download file: {ex.Message}", BetterLogger.LogLevel.Error, "WebCommands");
                return new CommandResponse((int)CommandStatus.Fail);
            }
        }
        [CommandHelp("Downloads a file to the specified location.")]
        public CommandResponse wget(CommandRequest args, CommandPipeline pipe)
        {
            if(args.Arguments.Count <= 2)
            {
                _service.RemoteInterface.Client.ClientCallback.TellMessageToServerConsole(new ConsoleText("You must specify a destination location.") { TextColor = Color.Red });
                return new CommandResponse((int)CommandStatus.Fail);
            }
            else
            {
                try
                {
                    if (!Uri.IsWellFormedUriString(args.Arguments[1].ToString(), UriKind.Absolute))
                    {
                        _service.RemoteInterface.Client.ClientCallback.TellMessageToServerConsole(new ConsoleText("Web address is invalid.") { TextColor = Color.Red });
                        return new CommandResponse((int)CommandStatus.Fail);
                    }
                    _service.RemoteInterface.Client.ClientCallback.TellMessageToServerConsole($"Starting download for {args.Arguments[1].ToString()}");
                    _service.RemoteInterface.Client.ClientCallback.TellMessageToServerConsole("Opening connection...");
                    WebClient client = new WebClient();
                    client.DownloadProgressChanged += (sender, e) =>
                    {
                        _service.RemoteInterface.Client.ClientCallback.UpdateRequest(new ProgressUpdateBuilder(e.ProgressPercentage)
                        {
                            Text = $"{e.BytesReceived} / {e.TotalBytesToReceive} bytes received."
                        });
                    };
                    _service.RemoteInterface.Client.ClientCallback.RequestInformation(new ProgressRequestBuilder()
                    {
                        Message = "Downloading file."
                    });
                    Task t = client.DownloadFileTaskAsync(args.Arguments[1].ToString(), args.Arguments[2].ToString());
                    t.Wait();
                    _service.RemoteInterface.Client.ClientCallback.DisposeCurrentRequest();
                    _service.RemoteInterface.Client.ClientCallback.TellMessageToServerConsole("File downloaded succesfully.");
                    return new CommandResponse((int)CommandStatus.Success);
                }
                catch (AggregateException ex)
                {
                    _service.RemoteInterface.Client.ClientCallback.TellMessageToServerConsole(new ConsoleText($"Unable to download web resource. Message: {ex.GetBaseException().Message}") { TextColor = Color.Red });
                    return new CommandResponse((int)CommandStatus.Fail);
                }
            }
        }
    }
}