using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ninject;
using RemotePlusLibrary.Core.IOC;
using RemotePlusLibrary.Extension;

namespace RemotePlusLibrary.SubSystem.Command
{
    public abstract class StandordCommandClass : ICommandModule
    {
        public Dictionary<string, CommandDelegate> Commands { get; } = new Dictionary<string, CommandDelegate>();
        public IExtensionSubsystem<ICommandModule> Host { get; }

        public bool HasCommand(string commandName)
        {
            return Commands.ContainsKey(commandName);
        }

        public abstract void InitializeServices(IServiceCollection services);

        public CommandDelegate Lookup(string commandName)
        {
            if (Commands.TryGetValue(commandName, out CommandDelegate command))
            {
                return command;
            }
            else
            {
                return null;
            }
        }
    }
}
