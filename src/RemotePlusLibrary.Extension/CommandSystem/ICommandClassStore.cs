using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.Extension.CommandSystem
{
    public interface ICommandClassStore
    {
        List<ICommandClass> CommandClasses { get; }
        CommandDelegate GetCommand(string name);
        bool HasCommand(string command);
        void InitializeCommands();
        IDictionary<string, CommandDelegate> GetAllCommands();
    }
}