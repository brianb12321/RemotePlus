using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BetterLogger;
using RemotePlusLibrary.Core;
using RemotePlusLibrary.Extension.CommandSystem;
using RemotePlusLibrary.Extension.CommandSystem.CommandClasses;
using RemotePlusLibrary.Extension.CommandSystem.CommandClasses.Parsing;
using RemotePlusLibrary.Scripting;

namespace RemotePlusLibrary.Extension.CommandSystem
{
    public class CommandEnvironment : ICommandEnvironmnet
    {
        public event EventHandler<CommandLogEventArgs> CommandLogged;
        public ILexer Lexer { get;private set; }

        public ICommandExecutor Executor { get; private set; }
        public ICommandClassStore CommandClasses { get; }
        public IParser Parser { get; }

        public CommandEnvironment(ILexer lexer, ICommandExecutor executor, ICommandClassStore commandClasses, IParser parser)
        {
            Lexer = lexer;
            Parser = parser;
            Executor = executor;
            CommandClasses = commandClasses;
        }

        public CommandPipeline Execute(string command, CommandExecutionMode mode)
        {
            CommandPipeline pipe = new CommandPipeline();
            int pos = 0;
            try
            {
                ILexer lexer = Lexer;
                IParser parser = Parser;
                //ITokenProcessor processor = env.Processor;
                RemotePlusLibrary.Extension.CommandSystem.CommandClasses.Parsing.ICommandExecutor executor = Executor;
                //processor.ConfigureProcessor(ServerManager.ServerRemoteService.Variables, executor);
                var tokens = lexer.Lex(command);
                var elements = parser.Parse(tokens);
                //var newTokens = processor.RunSubRoutines(lexer, pipe, pos);
                //Run the commands
                if (elements.Count <= 1)
                {
                    CommandRequest request = new CommandRequest(elements[0].ToArray());
                    var routine = new CommandRoutine(request, executor.Execute(request, mode, pipe));
                    pipe.Add(pos++, routine);
                }
                else
                {
                    CommandResponse result = null;
                    foreach (List<ICommandElement> currentCommand in elements)
                    {
                        if (elements.IndexOf(currentCommand) == 0)
                        {
                            CommandRequest firstRequest = new CommandRequest(currentCommand.ToArray());
                            var firstRoutine = new CommandRoutine(firstRequest, executor.Execute(firstRequest, mode, pipe));
                            result = firstRoutine.Output;
                            pipe.Add(pos++, firstRoutine);
                        }
                        else
                        {
                            CommandRequest request = new CommandRequest(currentCommand.ToArray());
                            request.HasLastCommand = true;
                            request.LastCommand = result;
                            var routine = new CommandRoutine(request, executor.Execute(request, mode, pipe));
                            result = routine.Output;
                            pipe.Add(pos++, routine);
                        }
                    }
                }

            }
            catch (ScriptException ex)
            {
                string scriptErrorMessage = $"{ex.Message}";
                GlobalServices.Logger.Log(scriptErrorMessage, LogLevel.Error, ScriptBuilder.SCRIPT_LOG_CONSTANT);
                CommandLogged?.Invoke(this, new CommandLogEventArgs(new ConsoleText(scriptErrorMessage) { TextColor = Color.Red }));
            }
            catch (ParserException e)
            {
                string parseErrorMessage = $"Unable to parse command: {e.Message}";
                GlobalServices.Logger.Log(parseErrorMessage, LogLevel.Error, "Server Host");
                CommandLogged?.Invoke(this, new CommandLogEventArgs(new ConsoleText(parseErrorMessage) { TextColor = Color.Red }));
                return pipe;
            }
            return pipe;
        }
    }
}