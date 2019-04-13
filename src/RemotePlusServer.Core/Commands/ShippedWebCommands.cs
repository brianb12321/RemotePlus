using System;
using System.Drawing;
using System.Net;
using System.Threading.Tasks;
using RemotePlusLibrary.Core.IOC;
using RemotePlusLibrary.Extension;
using RemotePlusLibrary.RequestSystem.DefaultRequestBuilders;
using RemotePlusLibrary.RequestSystem.DefaultUpdateRequestBuilders;
using RemotePlusLibrary.ServiceArchitecture;
using RemotePlusLibrary.SubSystem.Command;
using RemotePlusLibrary.SubSystem.Command.CommandClasses;
using RemotePlusServer.Core.ExtensionSystem;

namespace RemotePlusServer.Core.Commands
{
    [ExtensionModule]
    public class ShippedWebCommands : ServerCommandClass
    {
        private IRemotePlusService<ServerRemoteInterface> _service;
        [CommandHelp("Downloads a file from the internet and displays it in the console.")]
        public CommandResponse downloadWeb(CommandRequest args, CommandPipeline pipe, ICommandEnvironment currentEnvironment)
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
        [CommandHelp("Downloads a file from the internet.")]
        public CommandResponse wget(CommandRequest args, CommandPipeline pipe, ICommandEnvironment currentEnvironment)
        {
            if (args.Arguments.Count < 2)
            {
                currentEnvironment.WriteLineErrorWithColor("You must specify a destination location.", Color.Red);
                return new CommandResponse((int)CommandStatus.Fail);
            }
            else
            {
                try
                {
                    if (!Uri.IsWellFormedUriString(args.Arguments[1].ToString(), UriKind.Absolute))
                    {
                        currentEnvironment.WriteLineErrorWithColor("Web address is invalid.", Color.Red);
                        return new CommandResponse((int)CommandStatus.Fail);
                    }
                    currentEnvironment.WriteLine($"Starting download for {args.Arguments[1].ToString()}");
                    currentEnvironment.WriteLine("Opening connection...");
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
                    var sub = args.CancellationToken.Register(() =>
                    {
                        client.CancelAsync();
                    });
                    Task t = client.DownloadFileTaskAsync(args.Arguments[1].ToString(), args.Arguments[2].ToString());
                    t.Wait(args.CancellationToken);
                    _service.RemoteInterface.Client.ClientCallback.DisposeCurrentRequest();
                    currentEnvironment.WriteLine("File downloaded successfully.");
                    sub.Dispose();
                    return new CommandResponse((int)CommandStatus.Success);
                }
                catch (AggregateException ex)
                {
                    _service.RemoteInterface.Client.ClientCallback.DisposeCurrentRequest();
                    currentEnvironment.WriteLineErrorWithColor($"Unable to download web resource. Message: {ex.GetBaseException().Message}", Color.Red);
                    return new CommandResponse((int)CommandStatus.Fail);
                }
            }
        }
        public override void InitializeServices(IServiceCollection services)
        {
            _service = services.GetService<IRemotePlusService<ServerRemoteInterface>>();
            Commands.Add("downloadWeb", downloadWeb);
            Commands.Add("wget", wget);
        }
    }
}