using RemotePlusLibrary.Extension.CommandSystem.CommandClasses;
using RemotePlusLibrary.Extension.CommandSystem.CommandClasses.Parsing;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.Extension.CommandSystem
{
    /// <summary>
    /// Exposes methods and events for interacting with the command-line environment.
    /// </summary>
    public interface ICommandEnvironment : IDisposable
    {
        event EventHandler<CommandLogEventArgs> CommandLogged;
        event EventHandler<MultiLineEntryEventArgs> MultilineEntry;
        event EventHandler<ConsoleColorEventArgs> SwitchForgroundColor;
        event EventHandler<ConsoleColorEventArgs> SwitchBackgroundColor;
        TextWriter Out { get; }
        TextReader In { get; }
        ILexer Lexer { get; }
        IParser Parser { get; }
        ICommandExecutor Executor { get; }
        ICommandClassStore CommandClasses { get; }
        CommandPipeline Execute(string command, CommandExecutionMode mode);
        void WriteLine(string text);
        void WriteLine(string text, Color color);
        void WriteLine(ConsoleText text);
        void WriteLine();
        void Write(string text);
        void Write(ConsoleText text);
        void SetOut(TextWriter writer);
        void SetIn(TextReader reader);
    }
}