using System;
using System.Net;
using RemotePlusServer.Core;
using BetterLogger;
using System.IO;
using RemotePlusLibrary.ServiceArchitecture;
using System.Drawing;
using System.Threading.Tasks;
using RemotePlusLibrary.RequestSystem.DefaultRequestBuilders;
using RemotePlusLibrary.RequestSystem.DefaultUpdateRequestBuilders;
using Ninject;
using RemotePlusServer.Core.ExtensionSystem;
using RemotePlusLibrary.SubSystem.Command;
using RemotePlusLibrary.SubSystem.Command.CommandClasses;
using RemotePlusLibrary.Core;

namespace CommonWebCommands
{
    public class DownloadCommands : ServerCommandClass
    {
        IRemotePlusService<ServerRemoteInterface> _service;

        public override void InitializeServices(IKernel kernel)
        {
            GlobalServices.Logger.Log("Adding download commands.", LogLevel.Info);
            _service = kernel.Get<IRemotePlusService<ServerRemoteInterface>>();
            Commands.Add("downloadWeb", downloadWeb);
            Commands.Add("wget", wget);
        }

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
            if(args.Arguments.Count < 2)
            {
                currentEnvironment.WriteLine(new ConsoleText("You must specify a destination location.") { TextColor = Color.Red });
                return new CommandResponse((int)CommandStatus.Fail);
            }
            else
            {
                try
                {
                    if (!Uri.IsWellFormedUriString(args.Arguments[1].ToString(), UriKind.Absolute))
                    {
                        currentEnvironment.WriteLine(new ConsoleText("Web address is invalid.") { TextColor = Color.Red });
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
                    Task t = client.DownloadFileTaskAsync(args.Arguments[1].ToString(), args.Arguments[2].ToString());
                    t.Wait();
                    _service.RemoteInterface.Client.ClientCallback.DisposeCurrentRequest();
                    currentEnvironment.WriteLine("File downloaded succesfully.");
                    return new CommandResponse((int)CommandStatus.Success);
                }
                catch (AggregateException ex)
                {
                    _service.RemoteInterface.Client.ClientCallback.DisposeCurrentRequest();
                    currentEnvironment.WriteLine(new ConsoleText($"Unable to download web resource. Message: {ex.GetBaseException().Message}") { TextColor = Color.Red });
                    return new CommandResponse((int)CommandStatus.Fail);
                }
            }
        }
    }
}