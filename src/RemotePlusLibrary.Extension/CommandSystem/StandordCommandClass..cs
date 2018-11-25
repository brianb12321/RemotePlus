using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.Extension.CommandSystem
{
    public abstract class StandordCommandClass : ICommandClass
    {
        public Dictionary<string, CommandDelegate> Commands { get; } = new Dictionary<string, CommandDelegate>();

        public abstract void AddCommands();

        public bool HasCommand(string commandName)
        {
            return Commands.ContainsKey(commandName);
        }

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
