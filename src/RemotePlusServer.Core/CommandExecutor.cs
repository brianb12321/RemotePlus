using BetterLogger;
using RemotePlusLibrary;
using RemotePlusLibrary.Client;
using RemotePlusLibrary.Extension;
using RemotePlusLibrary.Extension.CommandSystem;
using RemotePlusLibrary.Extension.CommandSystem.CommandClasses;
using RemotePlusLibrary.Extension.CommandSystem.CommandClasses.Parsing;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace RemotePlusServer.Core
{
    public class CommandExecutor : ICommandExecutor
    {
        public CommandResponse Execute(CommandRequest arguments, CommandExecutionMode commandMode, CommandPipeline pipe)
        {
            bool throwFlag = false;
            StatusCodeDeliveryMethod scdm = StatusCodeDeliveryMethod.DoNotDeliver;
            try
            {
                GlobalServices.Logger.Log($"Executing server command {arguments.Arguments[0]}", LogLevel.Info);
                try
                {
                    var command = ServerManager.ServerRemoteService.Commands[arguments.Arguments[0].Value];
                    var ba = RemotePlusConsole.GetCommandBehavior(command);
                    if (ba != null)
                    {
                        if (ba.TopChainCommand && pipe.Count > 0)
                        {
                            GlobalServices.Logger.Log($"This is a top-level command.", LogLevel.Error);
                            ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.TellMessage($"This is a top-level command.", LogLevel.Error);
                            return new CommandResponse((int)CommandStatus.AccessDenied);
                        }
                        if (commandMode != ba.ExecutionType)
                        {
                            GlobalServices.Logger.Log($"The command requires you to be in {ba.ExecutionType} mode.", LogLevel.Error);
                            ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.TellMessage($"The command requires you to be in {ba.ExecutionType} mode.", LogLevel.Error);
                            return new CommandResponse((int)CommandStatus.AccessDenied);
                        }
                        if (ba.SupportClients != ClientSupportedTypes.Both && ((ServerManager.ServerRemoteService.RemoteInterface.Client.ClientType == ClientType.GUI && ba.SupportClients != ClientSupportedTypes.GUI) || (ServerManager.ServerRemoteService.RemoteInterface.Client.ClientType == ClientType.CommandLine && ba.SupportClients != ClientSupportedTypes.CommandLine)))
                        {
                            if (string.IsNullOrEmpty(ba.ClientRejectionMessage))
                            {
                                GlobalServices.Logger.Log($"Your client must be a {ba.SupportClients.ToString()} client.", LogLevel.Error);
                                ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.TellMessage($"Your client must be a {ba.SupportClients.ToString()} client.", LogLevel.Error);
                                return new CommandResponse((int)CommandStatus.UnsupportedClient);
                            }
                            else
                            {
                                GlobalServices.Logger.Log(ba.ClientRejectionMessage, LogLevel.Error);
                                ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.TellMessage(ba.ClientRejectionMessage, LogLevel.Error);
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
                    GlobalServices.Logger.Log("Found command, and executing.", LogLevel.Debug);
                    var sc = command(arguments, pipe);
                    if (scdm == StatusCodeDeliveryMethod.TellMessage)
                    {
                        ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.TellMessage($"Command {arguments.Arguments[0]} finished with status code {sc.ToString()}", LogLevel.Info);
                    }
                    else if (scdm == StatusCodeDeliveryMethod.TellMessageToServerConsole)
                    {
                        ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.TellMessageToServerConsole($"Command {arguments.Arguments[0]} finished with status code {sc.ToString()}", LogLevel.Info);
                    }
                    return sc;
                }
                catch (KeyNotFoundException)
                {
                    GlobalServices.Logger.Log("Failed to find the command.", LogLevel.Debug);
                    ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.TellMessageToServerConsole(new ConsoleText("Unknown command. Please type {help} for a list of commands") { TextColor = Color.Red });
                    return new CommandResponse((int)CommandStatus.Fail);
                }
            }
            catch (Exception ex)
            {
                if (throwFlag)
                {
                    throw;
                }
                else
                {
                    GlobalServices.Logger.Log("command failed: " + ex.Message, LogLevel.Info);
                    ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.TellMessageToServerConsole(new ConsoleText("Error whie executing command: " + ex.Message) { TextColor = Color.Red });
                    return new CommandResponse((int)CommandStatus.Fail);
                }
            }
        }
    }
}
