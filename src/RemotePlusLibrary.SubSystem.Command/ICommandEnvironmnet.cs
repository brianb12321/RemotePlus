using RemotePlusLibrary.Scripting;
using RemotePlusLibrary.SubSystem.Command.CommandClasses;
using RemotePlusLibrary.SubSystem.Command.CommandClasses.Parsing;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.SubSystem.Command
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
        //event EventHandler ProcessFinished;
        IScriptingEngine EnvironmentEngine { get; }
        TextWriter Out { get; }
        TextWriter Error { get; }
        TextReader In { get; }
        ILexer Lexer { get; }
        IParser Parser { get; }
        ICommandExecutor Executor { get; }
        CommandPipeline Execute(string command, CommandExecutionMode mode);
        Task<CommandPipeline> ExecuteAsync(string command, CommandExecutionMode mode);
        void Cancel();
        object ExecuteScript(string content);
        IScriptExecutionContext ExecuteScriptFile(CommandRequest args);
        void WriteLine(string text);
        void WriteLine(string text, Color color);
        void WriteLine(ConsoleText text);
        void WriteLine();
        void Write(string text);
        void Write(ConsoleText text);
        void WriteLineError(string text);
        void WriteLineError(string text, Color color);
        void WriteLineError(ConsoleText text);
        void WriteLineError();
        void WriteError(string text);
        void WriteError(ConsoleText text);
        string ReadLine();
        string ReadToEnd();
        void SetOut(TextWriter writer);
        void SetError(TextWriter writer);
        void SetIn(TextReader reader);
    }
}