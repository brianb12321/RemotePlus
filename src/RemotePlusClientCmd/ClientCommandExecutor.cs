using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BetterLogger;
using RemotePlusClientCmd.ClientExtensionSystem;
using RemotePlusLibrary.Core;
using RemotePlusLibrary.SubSystem.Command;
using RemotePlusLibrary.SubSystem.Command.CommandClasses;
using RemotePlusLibrary.SubSystem.Command.CommandClasses.Parsing;

namespace RemotePlusClientCmd
{
    public class ClientCommandExecutor : ICommandExecutor
    {
        private ICommandSubsystem<IClientCmdModule> _commandSubsystem;

        public ClientCommandExecutor(ICommandSubsystem<IClientCmdModule> s)
        {
            _commandSubsystem = s;
        }

        public event EventHandler<CommandEventArgs> CommandNotFound;

        public CommandResponse Execute(CommandRequest request, CommandExecutionMode commandMode, CommandPipeline pipe, ICommandEnvironment env)
        {
            bool throwFlag = false;
            StatusCodeDeliveryMethod scdm = StatusCodeDeliveryMethod.DoNotDeliver;
            try
            {
                if (!_commandSubsystem.HasCommand(request.Arguments[0].ToString()))
                {
                    GlobalServices.Logger.Log("Unknown local command. Please type {#help} for a list of commands.", LogLevel.Error);
                    return new CommandResponse((int)CommandStatus.Fail);
                }
                var command = _commandSubsystem.GetCommand(request.Arguments[0].ToString());
                var ba = _commandSubsystem.GetCommandBehavior(command);
                if (ba != null)
                {
                    if (ba.TopChainCommand && pipe.Count > 0)
                    {
                        GlobalServices.Logger.Log($"This is a top-level command.", LogLevel.Error);
                        return new CommandResponse((int)CommandStatus.AccessDenied);
                    }
                    if (commandMode != ba.ExecutionType)
                    {
                        GlobalServices.Logger.Log($"The command requires you to be in {ba.ExecutionType} mode.", LogLevel.Error);
                        return new CommandResponse((int)CommandStatus.AccessDenied);
                    }
                    if (ba.DoNotCatchExceptions)
                    {
                        throwFlag = true;
                    }
                    if (ba.StatusCodeDeliveryMethod != StatusCodeDeliveryMethod.DoNotDeliver)
                    {
                        scdm = StatusCodeDeliveryMethod.TellMessageToServerConsole;
                    }
                }
                var sc = command(request, pipe, null);
                if (scdm == StatusCodeDeliveryMethod.TellMessage)
                {
                    GlobalServices.Logger.Log($"Command {request.Arguments[0]} finished with status code {sc.ToString()}", LogLevel.Info);
                }
                else if (scdm == StatusCodeDeliveryMethod.TellMessageToServerConsole)
                {
                    GlobalServices.Logger.Log($"Command {request.Arguments[0]} finished with status code {sc.ToString()}", LogLevel.Info);
                }
                return sc;
            }
            catch (Exception ex)
            {
                if (throwFlag)
                {
                    throw;
                }
                else
                {
                    GlobalServices.Logger.Log("Error whie executing local command: " + ex.Message, LogLevel.Error);
                    return new CommandResponse((int)CommandStatus.Fail);
                }
            }
        }
    }
}
