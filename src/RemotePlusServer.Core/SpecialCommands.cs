using RemotePlusLibrary.Extension.CommandSystem;
using RemotePlusLibrary.Extension.CommandSystem.CommandClasses;
using RemotePlusLibrary.ServiceArchitecture;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusServer.Core
{
    public class SpecialCommands : StandordCommandClass
    {
        private bool parseCondition(CommandRequest args, List<Predicate<CommandResponse>> _predicates, int startIndex)
        {
            switch (args.Arguments[startIndex].ToString())
            {
                //Check if result equals predicate
                case "resultString=":
                    _predicates.Add((s) => args.Arguments[startIndex + 1].ToString().Equals(s.ReturnData.ToString()));
                    return true;
                case "resultCode=":
                    _predicates.Add((code) => args.Arguments[startIndex + 1].ToString().Equals(code.ResponseCode.ToString()));
                    return true;
                case "isNull":
                    _predicates.Add((returnData) => returnData.ReturnData == null);
                    return true;
                default:
                    return false;
            }
        }
        [CommandBehavior(IndexCommandInHelp = false)]
        public CommandResponse _if(CommandRequest args, CommandPipeline pipe, ICommandEnvironment currentEnvironment)
        {
            if(!args.HasLastCommand)
            {
                currentEnvironment.WriteLine(new ConsoleText("If can only be used when in a pipeline.") { TextColor = Color.Red });
                return new CommandResponse((int)CommandStatus.Fail);
            }
            bool negateCondition = false;
            string thenStatement = string.Empty;
            string elseStatement = string.Empty;
            int COMMAND_LENGTH = args.Arguments.Count - 1;
            int checkIndex = 1;
            int PREDICATE = checkIndex + 1;
            if (COMMAND_LENGTH < checkIndex)
            {
                currentEnvironment.WriteLine(new ConsoleText("You must provide a condition.") { TextColor = Color.Red });
                return new CommandResponse((int)CommandStatus.Fail);
            }
            if (args.Arguments[1].ToString().Equals("not"))
            {
                negateCondition = true;
                checkIndex = 2;
            }
            if(COMMAND_LENGTH < PREDICATE)
            {
                currentEnvironment.WriteLine(new ConsoleText("You must provide a predicate.") { TextColor = Color.Red });
                return new CommandResponse((int)CommandStatus.Fail);
            }
            List<Predicate<CommandResponse>> _predicates = new List<Predicate<CommandResponse>>();
            if(!parseCondition(args, _predicates, checkIndex))
            {
                currentEnvironment.WriteLine(new ConsoleText("The condition you provided does not exist.") { TextColor = Color.Red });
                return new CommandResponse((int)CommandStatus.Fail);
            }
            bool success = false;
            for (int i = 0; i < _predicates.Count; i++)
            {
                var predicateValue = _predicates[i].Invoke(args.LastCommand);
                if (!negateCondition)
                {
                    if (!predicateValue)
                    {
                        success = false;
                        break;
                    }
                    else
                    {
                        success = true;
                    }
                }
                else
                {
                    if (predicateValue)
                    {
                        success = false;
                        break;
                    }
                    else
                    {
                        success = true;
                    }
                }
            }
            if (COMMAND_LENGTH < checkIndex + 2)
            {
                return new CommandResponse((int)CommandStatus.Success)
                {
                    ReturnData = success
                };
            }
            else
            {
                if (args.Arguments[checkIndex + 2].ToString() == "then")
                {
                    if (COMMAND_LENGTH < checkIndex + 3)
                    {
                        currentEnvironment.WriteLine(new ConsoleText("Please provide an action.") { TextColor = Color.Red });
                        return new CommandResponse((int)CommandStatus.Fail);
                    }
                    else
                    {
                        thenStatement = args.Arguments[checkIndex + 3].ToString();
                    }
                    if (COMMAND_LENGTH >= checkIndex + 4 && args.Arguments[checkIndex + 4].ToString() == "else")
                    {
                        if (COMMAND_LENGTH < checkIndex + 5)
                        {
                            currentEnvironment.WriteLine(new ConsoleText("Please provide an else action.") { TextColor = Color.Red });
                            return new CommandResponse((int)CommandStatus.Fail);
                        }
                        else
                        {
                            elseStatement = args.Arguments[checkIndex + 5].ToString();
                        }
                    }
                }
                if (success)
                {
                    var pipeline = currentEnvironment.Execute(thenStatement, CommandExecutionMode.Client);
                    var lastCommand = pipeline.GetLatest();
                    return new CommandResponse(lastCommand.Output.ResponseCode)
                    {
                        Metadata = lastCommand.Output.Metadata,
                        ReturnData = lastCommand.Output.ReturnData
                    };
                }
                else
                {
                    var pipeline = currentEnvironment.Execute(elseStatement, CommandExecutionMode.Client);
                    var lastCommand = pipeline.GetLatest();
                    return new CommandResponse(lastCommand.Output.ResponseCode)
                    {
                        Metadata = lastCommand.Output.Metadata,
                        ReturnData = lastCommand.Output.ReturnData
                    };
                }
            }
        }
        public override void AddCommands()
        {
            Commands.Add("if", _if);
        }
    }
}