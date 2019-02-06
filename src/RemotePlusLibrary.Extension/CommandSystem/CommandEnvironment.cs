using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BetterLogger;
using RemotePlusLibrary.Core;
using RemotePlusLibrary.Extension.CommandSystem;
using RemotePlusLibrary.Extension.CommandSystem.CommandClasses;
using RemotePlusLibrary.Extension.CommandSystem.CommandClasses.Parsing;
using RemotePlusLibrary.Extension.ResourceSystem.ResourceTypes;
using RemotePlusLibrary.Scripting;

namespace RemotePlusLibrary.Extension.CommandSystem
{
    public class CommandEnvironment : ICommandEnvironment
    {
        public event EventHandler<CommandLogEventArgs> CommandLogged;
        public event EventHandler<MultiLineEntryEventArgs> MultilineEntry;
        public event EventHandler<ConsoleColorEventArgs> SwitchForgroundColor;
        public event EventHandler<ConsoleColorEventArgs> SwitchBackgroundColor;

        public ILexer Lexer { get;private set; }

        public ICommandExecutor Executor { get; private set; }
        public ICommandClassStore CommandClasses { get; }
        public IParser Parser { get; }
        public TextWriter Out { get; private set; }
        public TextReader In { get; private set; }

        public CommandEnvironment(ILexer lexer, ICommandExecutor executor, ICommandClassStore commandClasses, IParser parser)
        {
            Lexer = lexer;
            Parser = parser;
            Executor = executor;
            CommandClasses = commandClasses;
        }

        public CommandPipeline Execute(string command, CommandExecutionMode mode)
        {
            //Enter multiline input mode.
            if(command.Length > 0 && command.ToCharArray()[0] == '`')
            {
                CommandPipeline pipe = new CommandPipeline();
                List<string> commands = new List<string>();
                MultiLineEntryEventArgs args = new MultiLineEntryEventArgs('>');
                while(true)
                {
                    MultilineEntry?.Invoke(this, args);
                    if (args.ReceivedValue.Length > 0 && args.ReceivedValue.ToCharArray()[0] == '`')
                    {
                        break;
                    }
                    else
                    {
                        commands.Add(args.ReceivedValue);
                    }
                }
                foreach(string c in commands)
                {
                    CommandPipeline currentPipe = executePrivate(c, mode);
                    foreach(CommandRoutine routine in currentPipe)
                    {
                        pipe.Add(routine);
                    }
                }
                return pipe;
            }
            else
            {
                return executePrivate(command, mode);
            }
        }
        private CommandPipeline executePrivate(string command, CommandExecutionMode mode)
        {
            CommandPipeline pipe = new CommandPipeline();
            try
            {
                ILexer lexer = Lexer;
                IParser parser = Parser;
                //ITokenProcessor processor = env.Processor;
                RemotePlusLibrary.Extension.CommandSystem.CommandClasses.Parsing.ICommandExecutor executor = Executor;
                //processor.ConfigureProcessor(ServerManager.ServerRemoteService.Variables, executor);
                var tokens = lexer.Lex(command);
                var elements = parser.Parse(tokens, this);
                //var newTokens = processor.RunSubRoutines(lexer, pipe, pos);
                //Run the commands
                if (elements.Count <= 1)
                {
                    CommandRequest request = new CommandRequest(elements[0].ToArray());
                    var routine = new CommandRoutine(request, executor.Execute(request, mode, pipe, this));
                    pipe.Add(routine);
                }
                else
                {
                    CommandResponse result = null;
                    foreach (List<ICommandElement> currentCommand in elements)
                    {
                        if (elements.IndexOf(currentCommand) == 0)
                        {
                            CommandRequest firstRequest = new CommandRequest(currentCommand.ToArray());
                            var firstRoutine = new CommandRoutine(firstRequest, executor.Execute(firstRequest, mode, pipe, this));
                            result = firstRoutine.Output;
                            pipe.Add(firstRoutine);
                        }
                        else
                        {
                            CommandRequest request = new CommandRequest(currentCommand.ToArray());
                            request.HasLastCommand = true;
                            request.LastCommand = result;
                            var routine = new CommandRoutine(request, executor.Execute(request, mode, pipe, this));
                            result = routine.Output;
                            pipe.Add(routine);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                string scriptErrorMessage = $"An error occurred before the executor phase: {ex.Message}";
                GlobalServices.Logger.Log(scriptErrorMessage, LogLevel.Error, ScriptBuilder.SCRIPT_LOG_CONSTANT);
                CommandLogged?.Invoke(this, new CommandLogEventArgs(new ConsoleText(scriptErrorMessage) { TextColor = Color.Red }));
            }
            return pipe;
        }

        public void WriteLine(string text)
        {
            Out?.WriteLine(text);
        }

        public void WriteLine(ConsoleText text)
        {
            SwitchForgroundColor?.Invoke(this, new ConsoleColorEventArgs(text.TextColor));
            Out?.WriteLine(text);
        }

        public void WriteLine(string text, Color color)
        {
            WriteLine(new ConsoleText(text) { TextColor = color });
        }
        public void WriteLine()
        {
            Out?.WriteLine();
        }
        public void Write(string text)
        {
            Out?.Write(text);
        }

        public void Write(ConsoleText text)
        {
            SwitchForgroundColor?.Invoke(this, new ConsoleColorEventArgs(text.TextColor));
            Out?.Write(text);
        }

        public void SetOut(TextWriter writer)
        {
            Out = writer;
        }
        public void SetIn(TextReader reader)
        {
            In = reader;
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    In?.Close();
                    Out?.Close();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~CommandEnvironment() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}