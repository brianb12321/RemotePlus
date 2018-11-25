using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.Extension.CommandSystem
{
    /// <summary>
    /// Notifies the command system that the implemented aggregates commands.
    /// </summary>
    public interface ICommandClass
    {
        Dictionary<string, CommandDelegate> Commands { get; }
        void AddCommands();
        CommandDelegate Lookup(string commandName);
        bool HasCommand(string commandName);
    }
}