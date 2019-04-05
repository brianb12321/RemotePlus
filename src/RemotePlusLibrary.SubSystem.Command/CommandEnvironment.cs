using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using BetterLogger;
using RemotePlusLibrary.Core;
using RemotePlusLibrary.Core.IOC;
using RemotePlusLibrary.Scripting;
using RemotePlusLibrary.SubSystem.Command.CommandClasses;
using RemotePlusLibrary.SubSystem.Command.CommandClasses.Parsing;

namespace RemotePlusLibrary.SubSystem.Command
{
    public class CommandEnvironment : ICommandEnvironment
    {
        public event EventHandler<CommandLogEventArgs> CommandLogged;
        public event EventHandler ClearRequested;
        public event EventHandler<MultiLineEntryEventArgs> MultilineEntry;
        public event EventHandler<ConsoleColorEventArgs> SwitchForegroundColor;
        public event EventHandler<ConsoleColorEventArgs> SwitchBackgroundColor;
        public event EventHandler ResetColor;
        //public event EventHandler ProcessFinished;

        private CancellationTokenSource cts;
        public ILexer Lexer { get;private set; }

        public ICommandExecutor Executor { get; private set; }
        public IParser Parser { get; }
        public TextWriter Out { get; private set; }
        public TextWriter Error { get; private set; }
        public TextReader In { get; private set; }
        public IScriptingEngine EnvironmentEngine { get; private set; }

        public CommandEnvironment(ILexer lexer, ICommandExecutor executor, IParser parser)
        {
            Lexer = lexer;
            Parser = parser;
            Executor = executor;
        }

        public Task<CommandPipeline> ExecuteAsync(string command, CommandExecutionMode mode)
        {
            cts = new CancellationTokenSource();
            return Task.Factory.StartNew(() =>
            {
                //Enter multiline input mode.
                if (command.Length > 0 && command.ToCharArray()[0] == '`')
                {
                    CommandPipeline pipe = new CommandPipeline();
                    List<string> commands = new List<string>();
                    MultiLineEntryEventArgs args = new MultiLineEntryEventArgs('>');
                    while (true)
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
                    foreach (string c in commands)
                    {
                        CommandPipeline currentPipe = executePrivate(c, mode);
                        foreach (CommandRoutine routine in currentPipe)
                        {
                            pipe.Add(routine);
                        }
                    }
                    return pipe;
                }
                else
                {
                    if (command.Length >= 2 && command.Substring(0, 2) == "::")
                    {
                        try
                        {
                            IScriptingEngine engine = IOCContainer.GetService<IScriptingEngine>();
                            EnvironmentEngine = engine;
                            engine.SetOut(Out);
                            engine.SetError(Error);
                            engine.SetIn(In);
                            var executingContext = engine.GetSessionContext();
                            executingContext.AddVariable("ExecutingEnvironment", this);
                            object value = engine.ExecuteString<object>(command.Substring(2), executingContext);
                            //ProcessFinished?.Invoke(this, EventArgs.Empty);
                            return null;
                        }
                        catch (Exception ex)
                        {
                            WriteLineError(new ConsoleText($"Error while executing script: {ex.Message}") { TextColor = Color.Red });
                            return null;
                        }
                    }
                    else
                    {
                        var value = executePrivate(command, mode);
                        //ProcessFinished?.Invoke(this, EventArgs.Empty);
                        return value;
                    }
                }
            }, cts.Token);
        }
        public CommandPipeline Execute(string command, CommandExecutionMode mode)
        {
            return ExecuteAsync(command, mode).Result;
        }
        public void Cancel()
        {
            cts.Cancel();
        }
        private CommandPipeline executePrivate(string command, CommandExecutionMode mode)
        {
            CommandPipeline pipe = new CommandPipeline();
            try
            {
                ILexer lexer = Lexer;
                IParser parser = Parser;
                //ITokenProcessor processor = env.Processor;
                ICommandExecutor executor = Executor;
                //processor.ConfigureProcessor(ServerManager.ServerRemoteService.Variables, executor);
                var tokens = lexer.Lex(command);
                var (options, elements) = parser.Parse(tokens, this);
                if(options.DetachIO)
                {
                    SetIn(TextReader.Null);
                    SetOut(TextWriter.Null);
                    SetError(TextWriter.Null);
                }
                IScriptingEngine scriptEngine = IOCContainer.GetService<IScriptingEngine>();
                EnvironmentEngine = scriptEngine;
                scriptEngine.SetOut(Out);
                scriptEngine.SetError(Error);
                scriptEngine.SetIn(In);
                //var newTokens = processor.RunSubRoutines(lexer, pipe, pos);
                //Run the commands
                if (elements.Count <= 1)
                {
                    CommandRoutine routine = null;
                    CommandRequest request = new CommandRequest(elements[0].ToArray(), cts.Token);
                    routine = new CommandRoutine(request, executor.Execute(request, mode, pipe, this));
                    pipe.Add(routine);
                }
                else
                {
                    CommandResponse result = null;
                    foreach (List<ICommandElement> currentCommand in elements)
                    {
                        if (elements.IndexOf(currentCommand) == 0)
                        {
                            CommandRequest firstRequest = new CommandRequest(currentCommand.ToArray(), cts.Token);
                            CommandRoutine firstRoutine = null;
                            firstRoutine = new CommandRoutine(firstRequest, executor.Execute(firstRequest, mode, pipe, this));
                            result = firstRoutine.Output;
                            pipe.Add(firstRoutine);
                        }
                        else
                        {
                            CommandRequest request = new CommandRequest(currentCommand.ToArray(), cts.Token);
                            request.HasLastCommand = true;
                            request.LastCommand = result;
                            CommandRoutine routine = null;
                            routine = new CommandRoutine(request, executor.Execute(request, mode, pipe, this));
                            result = routine.Output;
                            pipe.Add(routine);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                string errorMessage = $"An error occurred before the executor phase: {ex.Message}";
                GlobalServices.Logger.Log(errorMessage, LogLevel.Error);
                CommandLogged?.Invoke(this, new CommandLogEventArgs(new ConsoleText(errorMessage) { TextColor = Color.Red }));
            }
            return pipe;
        }
        public void WriteLine(string text)
        {
            Out?.WriteLine(text);
        }

        public void WriteLine(ConsoleText text)
        {
            SwitchForegroundColor?.Invoke(this, new ConsoleColorEventArgs(text.TextColor));
            Out?.WriteLine(text);
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
            SwitchForegroundColor?.Invoke(this, new ConsoleColorEventArgs(text.TextColor));
            Out?.Write(text);
        }
        public string ReadLine()
        {
            return In.ReadLine();
        }
        public string ReadToEnd()
        {
            return In.ReadToEnd();
        }

        public void SetOut(TextWriter writer)
        {
            Out = writer;
        }
        public void SetError(TextWriter writer)
        {
            Error = writer;
        }
        public void SetIn(TextReader reader)
        {
            In = reader;
        }
        public void WriteLineError(string text)
        {
            Error?.WriteLine(text);
        }

        public void WriteLineError(ConsoleText text)
        {
            SwitchForegroundColor?.Invoke(this, new ConsoleColorEventArgs(text.TextColor));
            Error?.WriteLine(text);
        }

        public void WriteLineError()
        {
            Error?.WriteLine();
        }

        public void WriteError(string text)
        {
            Error?.Write(text);
        }

        public void WriteError(ConsoleText text)
        {
            SwitchForegroundColor?.Invoke(this, new ConsoleColorEventArgs(text.TextColor));
            Error?.Write(text);
        }

        public void Clear()
        {
            ClearRequested?.Invoke(this, EventArgs.Empty);
        }
        public void SetBackgroundColor(Color bgColor)
        {
            SwitchBackgroundColor?.Invoke(this, new ConsoleColorEventArgs(bgColor));
        }

        public void SetForegroundColor(Color fgColor)
        {
            SwitchForegroundColor?.Invoke(this, new ConsoleColorEventArgs(fgColor));
        }
        public void ResetAllColors()
        {
            ResetColor?.Invoke(this, EventArgs.Empty);
        }

        public object ExecuteScript(string content)
        {
            var executingContext = EnvironmentEngine.GetSessionContext();
            executingContext.AddVariable("ExecutingEnvironment", this);
            var valueObj = EnvironmentEngine.ExecuteString<object>(content, executingContext);
            return valueObj;
        }

        public IScriptExecutionContext ExecuteScriptFile(CommandRequest args)
        {
            var executingContext = EnvironmentEngine.CreateContext();
            executingContext.AddVariable("ExecutingEnvironment", this);
            executingContext.AddVariable("Arguments", args);
            var context = EnvironmentEngine.ExecuteFile(args.Arguments[0].ToString(), executingContext);
            return context;
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
                In = null;
                Error = null;
                Out = null;

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