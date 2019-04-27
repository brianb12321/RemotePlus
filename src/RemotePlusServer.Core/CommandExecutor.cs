﻿using BetterLogger;
using RemotePlusLibrary;
using RemotePlusLibrary.Extension;
using RemotePlusLibrary.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using RemotePlusServer.Core.ExtensionSystem;
using RemotePlusLibrary.Scripting;
using RemotePlusLibrary.SubSystem.Command;
using RemotePlusLibrary.SubSystem.Command.CommandClasses.Parsing;
using RemotePlusLibrary.SubSystem.Command.CommandClasses;
using RemotePlusLibrary.SubSystem.Command.CommandClasses.Parsing.CommandElements;

namespace RemotePlusServer.Core
{
    public class CommandExecutor : ICommandExecutor
    {
        private ICommandSubsystem<IServerCommandModule> _commandSubsystem;
        private ILogFactory _logger;
        public CommandExecutor(ICommandSubsystem<IServerCommandModule> system, ILogFactory logger)
        {
            _commandSubsystem = system;
            _logger = logger;
        }

        public event EventHandler<CommandEventArgs> CommandNotFound;

        public CommandResponse Execute(CommandRequest arguments, CommandExecutionMode commandMode, CommandPipeline pipe, ICommandEnvironment currentEnvironment)
        {
            var client = currentEnvironment.ClientContext.GetClient<RemoteClient>();
            if(arguments.Arguments.Count == 0)
            {
                return new CommandResponse((int)CommandStatus.Success);
            }
            else if (arguments.Arguments[0] is ScriptCommandElement)
            {
                ((ScriptCommandElement)arguments.Arguments[0]).Execute();
                return new CommandResponse((int)CommandStatus.Success)
                {
                    ReturnData = arguments.Arguments[0].Value
                };
            }
            else if(arguments.Arguments[0].ValueType == ElementValueType.ScriptFile)
            {
                try
                {
                    object data = null;
                    var context = currentEnvironment.ExecuteScriptFile(arguments);
                    if (context.ContainsVariable("ReturnData"))
                    {
                        data = context.GetVariable<object>("ReturnData");
                    }
                    else data = context;
                    return new CommandResponse((int)CommandStatus.Success)
                    {
                        ReturnData = data
                    };
                }
                catch (Exception ex)
                {
                    currentEnvironment.WriteLineErrorWithColor($"Error executing script file: {ex.Message}", Color.Red);
                    return new CommandResponse((int)CommandStatus.Fail);
                }
            }
            else
            {
                bool throwFlag = false;
                StatusCodeDeliveryMethod scdm = StatusCodeDeliveryMethod.DoNotDeliver;
                try
                {
                    _logger.Log($"Executing server command {arguments.Arguments[0]}", LogLevel.Info);
                    if (!_commandSubsystem.HasCommand(arguments.Arguments[0].ToString()))
                    {
                        _logger.Log("Failed to find the command.", LogLevel.Debug);
                        currentEnvironment.WriteLine(new ConsoleText("Unknown command. Please type {help} for a list of commands") { TextColor = Color.Red });
                        CommandNotFound?.Invoke(this, new CommandEventArgs(arguments));
                        return new CommandResponse((int)CommandStatus.Fail);
                    }
                    var command = _commandSubsystem.GetCommand(arguments.Arguments[0].ToString());
                    var ba = _commandSubsystem.GetCommandBehavior(command);
                    if (ba != null)
                    {
                        if (ba.TopChainCommand && pipe.Count > 0)
                        {
                            _logger.Log($"This is a top-level command.", LogLevel.Error);
                            client.ClientCallback.TellMessage($"This is a top-level command.", LogLevel.Error);
                            return new CommandResponse((int)CommandStatus.AccessDenied);
                        }
                        if (commandMode != ba.ExecutionType)
                        {
                            _logger.Log($"The command requires you to be in {ba.ExecutionType} mode.", LogLevel.Error);
                            client.ClientCallback.TellMessage($"The command requires you to be in {ba.ExecutionType} mode.", LogLevel.Error);
                            return new CommandResponse((int)CommandStatus.AccessDenied);
                        }
                        if (!ba.SupportClients.HasFlag(client.ClientType))
                        {
                            if (string.IsNullOrEmpty(ba.ClientRejectionMessage))
                            {
                                _logger.Log($"Your client must be a {ba.SupportClients.ToString()} client.", LogLevel.Error);
                                client.ClientCallback.TellMessage($"Your client must be a {ba.SupportClients.ToString()} client.", LogLevel.Error);
                                return new CommandResponse((int)CommandStatus.UnsupportedClient);
                            }
                            else
                            {
                                _logger.Log(ba.ClientRejectionMessage, LogLevel.Error);
                                client.ClientCallback.TellMessage(ba.ClientRejectionMessage, LogLevel.Error);
                                return new CommandResponse((int)CommandStatus.UnsupportedClient);
                            }
                        }
                        if (ba.DoNotCatchExceptions)
                        {
                            throwFlag = true;
                        }
                        if (ba.StatusCodeDeliveryMethod != StatusCodeDeliveryMethod.DoNotDeliver)
                        {
                            scdm = ba.StatusCodeDeliveryMethod;
                        }
                    }
                    _logger.Log("Found command, and executing.", LogLevel.Debug);
                    var sc = command(arguments, pipe, currentEnvironment);
                    if (scdm == StatusCodeDeliveryMethod.TellMessage)
                    {
                        client.ClientCallback.TellMessage($"Command {arguments.Arguments[0]} finished with status code {sc.ToString()}", LogLevel.Info);
                    }
                    else if (scdm == StatusCodeDeliveryMethod.TellMessageToServerConsole)
                    {
                        client.ClientCallback.TellMessageToServerConsole($"Command {arguments.Arguments[0]} finished with status code {sc.ToString()}", LogLevel.Info);
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
                        //Dispose any requests
                        client.ClientCallback.DisposeCurrentRequest();
                        _logger.Log("command failed: " + ex.Message, LogLevel.Info);
                        currentEnvironment.WriteLine(new ConsoleText("Error while executing command: " + ex.Message) { TextColor = Color.Red });
                        return new CommandResponse((int)CommandStatus.Fail);
                    }
                }
            }
        }
    }
}