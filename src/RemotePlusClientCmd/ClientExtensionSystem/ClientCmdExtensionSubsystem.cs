using RemotePlusLibrary.Core;
using RemotePlusLibrary.Core.EventSystem;
using RemotePlusLibrary.Core.IOC;
using RemotePlusLibrary.Extension;
using RemotePlusLibrary.SubSystem.Command;
using RemotePlusLibrary.SubSystem.Command.CommandClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusClientCmd.ClientExtensionSystem
{
    public class ClientCmdExtensionSubsystem : CommandSubsystem<IClientCmdModule>
    {
        private ICommandEnvironment _runningEnvironment;
        public ClientCmdExtensionSubsystem(IExtensionLibraryLoader loader) : base(loader)
        {
        }

        public override void Cancel()
        {
            _runningEnvironment.Cancel();
        }

        public override void Init()
        {
            base.Init();
            GlobalServices.Logger.Log("Starting proxy command subsystem.", BetterLogger.LogLevel.Info);
        }

        public override Task<CommandPipeline> RunServerCommandAsync(string command, CommandExecutionMode commandMode, IClientContext client)
        {
            _runningEnvironment = IOCContainer.GetService<ICommandEnvironment>();
            _runningEnvironment.SetOut(Console.Out);
            _runningEnvironment.SetIn(Console.In);
            _runningEnvironment.SetError(Console.Error);
            _runningEnvironment.ClearRequested += (sender, e) => Console.Clear();
            _runningEnvironment.MultilineEntry += (sender, e) =>
            {
                Console.Write(e.Prelude);
                e.ReceivedValue = Console.ReadLine();
            };
            _runningEnvironment.ResetColor += (sender, e) => Colorful.Console.ResetColor();
            _runningEnvironment.SwitchBackgroundColor += (sender, e) => Colorful.Console.BackgroundColor = e.TextColor;
            _runningEnvironment.SwitchForegroundColor += (sender, e) => Colorful.Console.ForegroundColor = e.TextColor;
            var t = _runningEnvironment.ExecuteAsync(command, CommandExecutionMode.Client);
            t.ContinueWith((blalbalba) =>
            {
                _runningEnvironment.Dispose();
                _runningEnvironment = null;
            });
            return t;
        }
    }
}