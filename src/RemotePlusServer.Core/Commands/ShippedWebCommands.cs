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
            var client = currentEnvironment.ClientContext.GetClient<RemoteClient>();
            WebClient wClient = new WebClient();
            try
            {
                client.ClientCallback.TellMessageToServerConsole(wClient.DownloadString(args.Arguments[1].ToString()));
                return new CommandResponse((int)CommandStatus.Success);
            }
            catch (Exception ex)
            {
                client.ClientCallback.TellMessageToServerConsole($"Unable to download file: {ex.Message}", BetterLogger.LogLevel.Error, "WebCommands");
                return new CommandResponse((int)CommandStatus.Fail);
            }
        }
        [CommandHelp("Downloads a file from the internet.")]
        public CommandResponse wget(CommandRequest args, CommandPipeline pipe, ICommandEnvironment currentEnvironment)
        {
            var client = currentEnvironment.ClientContext.GetClient<RemoteClient>();
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
                    WebClient wClient = new WebClient();
                    wClient.DownloadProgressChanged += (sender, e) =>
                    {
                        client.ClientCallback.UpdateRequest(new ProgressUpdateBuilder(e.ProgressPercentage)
                        {
                            Text = $"{e.BytesReceived} / {e.TotalBytesToReceive} bytes received."
                        });
                    };
                    client.ClientCallback.RequestInformation(new ProgressRequestBuilder()
                    {
                        Message = "Downloading file."
                    });
                    var sub = args.CancellationToken.Register(() =>
                    {
                        wClient.CancelAsync();
                    });
                    Task t = wClient.DownloadFileTaskAsync(args.Arguments[1].ToString(), args.Arguments[2].ToString());
                    t.Wait(args.CancellationToken);
                    client.ClientCallback.DisposeCurrentRequest();
                    currentEnvironment.WriteLine("File downloaded successfully.");
                    sub.Dispose();
                    return new CommandResponse((int)CommandStatus.Success);
                }
                catch (AggregateException ex)
                {
                    client.ClientCallback.DisposeCurrentRequest();
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