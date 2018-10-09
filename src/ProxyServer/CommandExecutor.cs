using BetterLogger;
using RemotePlusLibrary.Client;
using RemotePlusLibrary.Extension;
using RemotePlusLibrary.Extension.CommandSystem;
using RemotePlusLibrary.Extension.CommandSystem.CommandClasses;
using RemotePlusLibrary.Extension.CommandSystem.CommandClasses.Parsing;
using RemotePlusLibrary.Core;
using System;
using System.Collections.Generic;

namespace ProxyServer
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
                    var command = ProxyManager.ProxyService.Commands[arguments.Arguments[0].Value];
                    var ba = RemotePlusConsole.GetCommandBehavior(command);
                    if (ba != null)
                    {
                        if (ba.TopChainCommand && pipe.Count > 0)
                        {
                            GlobalServices.Logger.Log($"This is a top-level command.", LogLevel.Error);
                            ProxyManager.ProxyService.RemoteInterface.ProxyClient.ClientCallback.TellMessage(ProxyManager.ProxyGuid, $"This is a top-level command.", LogLevel.Error);
                            return new CommandResponse((int)CommandStatus.AccessDenied);
                        }
                        if (commandMode != ba.ExecutionType)
                        {
                            GlobalServices.Logger.Log($"The command requires you to be in {ba.ExecutionType} mode.", LogLevel.Error);
                            ProxyManager.ProxyService.RemoteInterface.ProxyClient.ClientCallback.TellMessage(ProxyManager.ProxyGuid, $"The command requires you to be in {ba.ExecutionType} mode.", LogLevel.Error);
                            return new CommandResponse((int)CommandStatus.AccessDenied);
                        }
                        if (ba.SupportClients != ClientSupportedTypes.Both && ((ProxyManager.ProxyService.RemoteInterface.ProxyClient.ClientType == ClientType.GUI && ba.SupportClients != ClientSupportedTypes.GUI) || (ProxyManager.ProxyService.RemoteInterface.ProxyClient.ClientType == ClientType.CommandLine && ba.SupportClients != ClientSupportedTypes.CommandLine)))
                        {
                            if (string.IsNullOrEmpty(ba.ClientRejectionMessage))
                            {
                                GlobalServices.Logger.Log($"Your client must be a {ba.SupportClients.ToString()} client.", LogLevel.Error);
                                ProxyManager.ProxyService.RemoteInterface.ProxyClient.ClientCallback.TellMessage(ProxyManager.ProxyGuid, $"Your client must be a {ba.SupportClients.ToString()} client.", LogLevel.Error);
                                return new CommandResponse((int)CommandStatus.UnsupportedClient);
                            }
                            else
                            {
                                GlobalServices.Logger.Log(ba.ClientRejectionMessage, LogLevel.Error);
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
                    GlobalServices.Logger.Log("Found command, and executing.", LogLevel.Debug);
                    var sc = command(arguments, pipe);
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
                catch (KeyNotFoundException)
                {
                    GlobalServices.Logger.Log($"Failed to find the command. Passing command to selected server [{ProxyManager.ProxyService.RemoteInterface.SelectedClient.UniqueID}]", LogLevel.Debug);
                    return new CommandResponse(3131);
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
                    ProxyManager.ProxyService.RemoteInterface.ProxyClient.ClientCallback.TellMessageToServerConsole(ProxyManager.ProxyGuid, "Error whie executing command: " + ex.Message, LogLevel.Error);
                    return new CommandResponse((int)CommandStatus.Fail);
                }
            }
        }
    }
}