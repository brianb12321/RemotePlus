using ProxyServer.Internal;
using RemotePlusLibrary.Core;
using RemotePlusLibrary.Core.IOC;
using RemotePlusLibrary.Extension;
using RemotePlusLibrary.RequestSystem.DefaultRequestBuilders;
using RemotePlusLibrary.ServiceArchitecture;
using RemotePlusLibrary.SubSystem.Command;
using RemotePlusLibrary.SubSystem.Command.CommandClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProxyServer.ExtensionSystem
{
    public class ProxyCommandSubsystem : CommandSubsystem<IProxyCommandModule>
    {
        IRemotePlusService<ProxyServerRemoteImpl> _service;
        ICommandEnvironment _runningEnvironment = null;
        public ProxyCommandSubsystem(IRemotePlusService<ProxyServerRemoteImpl> service, IExtensionLibraryLoader loader) : base(loader)
        {
            _service = service;
        }

        public override void Cancel()
        {
            if(_runningEnvironment != null)
            {
                _runningEnvironment?.Cancel();
            }
            else
            {
                _service.RemoteInterface.CancelServerCommand();
            }
        }

        public override void Init()
        {
            base.Init();
            GlobalServices.Logger.Log("Starting proxy command subsystem.", BetterLogger.LogLevel.Info);
        }

        public override Task<CommandPipeline> RunServerCommandAsync(string command, CommandExecutionMode commandMode)
        {
            //Sends the command directly to the selected server.
            if (command.StartsWith("=>"))
            {
                return _service.RemoteInterface.RunServerCommandAsync(command.Remove(0, 2), commandMode);
            }
            else
            {
                _runningEnvironment = IOCContainer.GetService<ICommandEnvironment>();
                _runningEnvironment.CommandLogged += (sender, e) => _service.RemoteInterface.ProxyClient.ClientCallback.WriteToClientConsole(ProxyManager.ProxyGuid, e.Text);
                _runningEnvironment.MultilineEntry += (sender, e) =>
                {
                    string input = _service.RemoteInterface.ProxyClient.ClientCallback.RequestInformation(ProxyManager.ProxyGuid, new ConsoleReadLineRequestBuilder(e.Prelude.ToString()) { LineColor = ConsoleColor.Yellow }).Data.ToString();
                    e.ReceivedValue = input;
                };
                _runningEnvironment.ClearRequested += (sender, e) => _service.RemoteInterface.ProxyClient.ClientCallback.ClearClientConsole(ProxyManager.ProxyGuid);
                _runningEnvironment.SwitchBackgroundColor += (sender, e) => _service.RemoteInterface.ProxyClient.ClientCallback.SetClientConsoleBackgroundColor(ProxyManager.ProxyGuid, e.TextColor);
                _runningEnvironment.SwitchForegroundColor += (sender, e) => _service.RemoteInterface.ProxyClient.ClientCallback.SetClientConsoleForegroundColor(ProxyManager.ProxyGuid, e.TextColor);
                _runningEnvironment.ResetColor += (sender, e) => _service.RemoteInterface.ProxyClient.ClientCallback.ResetClientConsoleColor(ProxyManager.ProxyGuid);
                _runningEnvironment.SetOut(new _ClientTextWriter(ProxyManager.ProxyGuid));
                _runningEnvironment.SetError(new _ClientTextWriter(ProxyManager.ProxyGuid));
                _runningEnvironment.SetIn(new _ClientTextReader(ProxyManager.ProxyGuid));
                var pipe = _runningEnvironment.ExecuteAsync(command, commandMode);
                pipe.ContinueWith((blalbalba) =>
                {
                    _runningEnvironment.Dispose();
                    _runningEnvironment = null;
                });
                return pipe;
            }
        }
    }
}