using RemotePlusLibrary.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.SubSystem.Command
{
    /// <summary>
    /// Notifies the command system that the implemented aggregates commands.
    /// </summary>
    public interface ICommandModule : IExtensionModule
    {
        Dictionary<string, CommandDelegate> Commands { get; }
        CommandDelegate Lookup(string commandName);
        bool HasCommand(string commandName);
    }
}