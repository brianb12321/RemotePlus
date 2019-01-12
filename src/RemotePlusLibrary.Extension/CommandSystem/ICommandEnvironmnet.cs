using RemotePlusLibrary.Extension.CommandSystem.CommandClasses;
using RemotePlusLibrary.Extension.CommandSystem.CommandClasses.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.Extension.CommandSystem
{
    /// <summary>
    /// Exposes methods and events for interacting with the command-line environment.
    /// </summary>
    public interface ICommandEnvironmnet
    {
        event EventHandler<CommandLogEventArgs> CommandLogged;
        ILexer Lexer { get; }
        IParser Parser { get; }
        ICommandExecutor Executor { get; }
        ICommandClassStore CommandClasses { get; }
        CommandPipeline Execute(string command, CommandExecutionMode mode);
    }
}