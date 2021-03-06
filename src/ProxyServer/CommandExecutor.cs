﻿using BetterLogger;
using RemotePlusLibrary.Client;
using RemotePlusLibrary.Extension;
using RemotePlusLibrary.Extension.CommandSystem;
using RemotePlusLibrary.Extension.CommandSystem.CommandClasses;
using RemotePlusLibrary.Extension.CommandSystem.CommandClasses.Parsing;
using RemotePlusLibrary.Core;
using System;
using System.Collections.Generic;
using RemotePlusLibrary.Extension.CommandSystem.CommandClasses.Parsing.CommandElements;
using System.Drawing;

namespace ProxyServer
{
    public class CommandExecutor : ICommandExecutor
    {
        ICommandClassStore _store;
        ILogFactory _logger;
        public CommandExecutor(ICommandClassStore store, ILogFactory logger)
        {
            _store = store;
            _logger = logger;
        }

        public event EventHandler<CommandEventArgs> CommandNotFound;

        public CommandResponse Execute(CommandRequest arguments, CommandExecutionMode commandMode, CommandPipeline pipe, ICommandEnvironment currentEnvironment)
        {
            if (arguments.Arguments.Count == 0)
            {
                return new CommandResponse((int)CommandStatus.Success);
            }
            else if(arguments.Arguments[0] is ScriptCommandElement)
            {
                ((ScriptCommandElement)arguments.Arguments[0]).Execute();
                return new CommandResponse((int)CommandStatus.Success)
                {
                    ReturnData = arguments.Arguments[0].Value
                };
            }
            bool throwFlag = false;
            StatusCodeDeliveryMethod scdm = StatusCodeDeliveryMethod.DoNotDeliver;
            try
            {
                _logger.Log($"Executing server command {arguments.Arguments[0]}", LogLevel.Info);
                if(!_store.HasCommand(arguments.Arguments[0].Value.ToString()))
                {
                    _logger.Log("Failed to find the command.", LogLevel.Debug);
                    currentEnvironment.WriteLine(new ConsoleText("Unknown proxy command. Please type {proxyHelp} for a list of commands") { TextColor = Color.Red });
                    CommandNotFound?.Invoke(this, new CommandEventArgs(arguments));
                    return new CommandResponse((int)CommandStatus.Fail);
                }
                var command = _store.GetCommand(arguments.Arguments[0].Value.ToString());
                var ba = RemotePlusConsole.GetCommandBehavior(command);
                if (ba != null)
                {
                    if (ba.TopChainCommand && pipe.Count > 0)
                    {
                        _logger.Log($"This is a top-level command.", LogLevel.Error);
                        ProxyManager.ProxyService.RemoteInterface.ProxyClient.ClientCallback.TellMessage(ProxyManager.ProxyGuid, $"This is a top-level command.", LogLevel.Error);
                        return new CommandResponse((int)CommandStatus.AccessDenied);
                    }
                    if (commandMode != ba.ExecutionType)
                    {
                        _logger.Log($"The command requires you to be in {ba.ExecutionType} mode.", LogLevel.Error);
                        ProxyManager.ProxyService.RemoteInterface.ProxyClient.ClientCallback.TellMessage(ProxyManager.ProxyGuid, $"The command requires you to be in {ba.ExecutionType} mode.", LogLevel.Error);
                        return new CommandResponse((int)CommandStatus.AccessDenied);
                    }
                    if (ba.SupportClients != ClientSupportedTypes.Both && ((ProxyManager.ProxyService.RemoteInterface.ProxyClient.ClientType == ClientType.GUI && ba.SupportClients != ClientSupportedTypes.GUI) || (ProxyManager.ProxyService.RemoteInterface.ProxyClient.ClientType == ClientType.CommandLine && ba.SupportClients != ClientSupportedTypes.CommandLine)))
                    {
                        if (string.IsNullOrEmpty(ba.ClientRejectionMessage))
                        {
                            _logger.Log($"Your client must be a {ba.SupportClients.ToString()} client.", LogLevel.Error);
                            ProxyManager.ProxyService.RemoteInterface.ProxyClient.ClientCallback.TellMessage(ProxyManager.ProxyGuid, $"Your client must be a {ba.SupportClients.ToString()} client.", LogLevel.Error);
                            return new CommandResponse((int)CommandStatus.UnsupportedClient);
                        }
                        else
                        {
                            _logger.Log(ba.ClientRejectionMessage, LogLevel.Error);
                            ProxyManager.ProxyService.RemoteInterface.ProxyClient.ClientCallback.TellMessage(ProxyManager.ProxyGuid, ba.ClientRejectionMessage, LogLevel.Error);
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
                    ProxyManager.ProxyService.RemoteInterface.ProxyClient.ClientCallback.TellMessage(ProxyManager.ProxyGuid, $"Command {arguments.Arguments[0]} finished with status code {sc.ToString()}", LogLevel.Info);
                }
                else if (scdm == StatusCodeDeliveryMethod.TellMessageToServerConsole)
                {
                    ProxyManager.ProxyService.RemoteInterface.ProxyClient.ClientCallback.TellMessageToServerConsole(ProxyManager.ProxyGuid, $"Command {arguments.Arguments[0]} finished with status code {sc.ToString()}", LogLevel.Info);
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
                    _logger.Log("command failed: " + ex.Message, LogLevel.Info);
                    currentEnvironment.WriteLine(new ConsoleText("Error whie executing proxy command: " + ex.Message) { TextColor = Color.Red });
                    return new CommandResponse((int)CommandStatus.Fail);
                }
            }
        }
    }
}