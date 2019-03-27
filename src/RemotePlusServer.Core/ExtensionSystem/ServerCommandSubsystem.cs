using RemotePlusLibrary.Core;
using RemotePlusLibrary.Core.IOC;
using RemotePlusLibrary.Extension;
using RemotePlusLibrary.ServiceArchitecture;
using RemotePlusLibrary.SubSystem.Command;
using RemotePlusLibrary.SubSystem.Command.CommandClasses;
using RemotePlusServer.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusServer.Core.ExtensionSystem
{
    public class ServerCommandSubsystem : CommandSubsystem<IServerCommandModule>
    {
        IRemotePlusService<ServerRemoteInterface> _service;
        private ICommandEnvironment _runningEnvironment = null;
        public ServerCommandSubsystem(IExtensionLibraryLoader loader, IRemotePlusService<ServerRemoteInterface> s) : base(loader)
        {
            _service = s;
        }

        public override void Cancel()
        {
            _runningEnvironment?.Cancel();
        }

        public override void Init()
        {
            base.Init();
            GlobalServices.Logger.Log("Starting server command subsystem.", BetterLogger.LogLevel.Info);
        }

        public override Task<CommandPipeline> RunServerCommandAsync(string command, CommandExecutionMode commandMode)
        {
            _runningEnvironment = IOCContainer.GetService<ICommandEnvironment>();
            _runningEnvironment.CommandLogged += (sender, e) => _service.RemoteInterface.Client.ClientCallback.TellMessageToServerConsole(e.Text);
            _runningEnvironment.MultilineEntry += (sender, e) =>
            {
                string input = _service.RemoteInterface.Client.ClientCallback.RequestInformation(new RemotePlusLibrary.RequestSystem.DefaultRequestBuilders.ConsoleReadLineRequestBuilder(e.Prelude.ToString()) { LineColor = ConsoleColor.Yellow }).Data.ToString();
                e.ReceivedValue = input;
            };
            //_runningEnvironment.ProcessFinished += (sender, e) =>
            //{
               
            //};
            _runningEnvironment.SetOut(new _ClientTextWriter(_service.RemoteInterface.Client.ClientCallback));
            _runningEnvironment.SetError(new _ClientTextWriter(_service.RemoteInterface.Client.ClientCallback));
            _runningEnvironment.SetIn(new _ClientTextReader(_service.RemoteInterface.Client.ClientCallback));
            var t = _runningEnvironment.ExecuteAsync(command, commandMode);
            t.ContinueWith((blalbalba) =>
            {
                _runningEnvironment.Dispose();
                _runningEnvironment = null;
            });
            return t;
        }
    }
}
