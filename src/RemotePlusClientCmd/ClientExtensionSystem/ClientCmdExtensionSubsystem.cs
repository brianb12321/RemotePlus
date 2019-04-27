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
        public ClientCmdExtensionSubsystem(IExtensionLibraryLoader loader) : base(loader)
        {
        }

        public override void Cancel()
        {
            throw new NotImplementedException();
        }

        public override void Init()
        {
            base.Init();
            GlobalServices.Logger.Log("Starting proxy command subsystem.", BetterLogger.LogLevel.Info);
        }

        public override Task<CommandPipeline> RunServerCommandAsync(string command, CommandExecutionMode commandMode, IClientContext client)
        {
            throw new NotImplementedException();
        }
    }
}