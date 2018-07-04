using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using RemotePlusLibrary.Discovery;
using RemotePlusLibrary;
using RemotePlusLibrary.Extension.CommandSystem;
using RemotePlusLibrary.Extension.CommandSystem.CommandClasses;
using RemotePlusLibrary.Scripting;
using RemotePlusLibrary.Security.AccountSystem;
using System.Speech.Synthesis;
using System.Windows.Forms;
using Logging;
using RemotePlusLibrary.Core;
using System.Text.RegularExpressions;
using System.Reflection;
using RemotePlusLibrary.Configuration.ServerSettings;
using RemotePlusLibrary.Client;
using RemotePlusLibrary.Contracts;
using RemotePlusLibrary.FileTransfer.BrowserClasses;
using RemotePlusLibrary.Core.Faults;
using RemotePlusLibrary.RequestSystem;
using RemotePlusLibrary.Security.Authentication;
using RemotePlusLibrary.Extension.CommandSystem.CommandClasses.Parsing;
using BetterLogger;
using RemotePlusLibrary.IOC;

namespace ProxyServer
{
    [ServiceBehavior(IncludeExceptionDetailInFaults = true,
        InstanceContextMode = InstanceContextMode.Single,
        ConcurrencyMode = ConcurrencyMode.Multiple,
        UseSynchronizationContext = false)]
    [GlobalException(typeof(GlobalErrorHandler))]
    public class ProxyServerRemoteImpl : IProxyServerRemote, IProxyRemote
    {
        public SessionClient<IRemoteWithProxy> SelectedClient = null;
        public Client<IRemoteClient> ProxyClient = null;
        public List<SessionClient<IRemoteWithProxy>> ConnectedServers { get; } = new List<SessionClient<IRemoteWithProxy>>();
        public void Beep(int Hertz, int Duration)
        {
            SelectedClient.ClientCallback.Beep(Hertz, Duration);
        }

        public void DecryptFile(string fileName, string password)
        {
            SelectedClient.ClientCallback.DecryptFile(fileName, password);
        }

        public void Disconnect()
        {
            SelectedClient.ClientCallback.Disconnect();
            ConnectedServers.Remove(SelectedClient);
        }

        public void EncryptFile(string fileName, string password)
        {
            SelectedClient.ClientCallback.EncryptFile(fileName, password);
        }

        public bool ExecuteScript(string script)
        {
            return SelectedClient.ClientCallback.ExecuteScript(script);
        }

        public string GetCommandHelpDescription(string command)
        {
            return SelectedClient.ClientCallback.GetCommandHelpDescription(command);
        }

        public string GetCommandHelpPage(string command)
        {
            return SelectedClient.ClientCallback.GetCommandHelpPage(command);
        }

        public IEnumerable<CommandDescription> GetCommands()
        {
            return SelectedClient.ClientCallback.GetCommands();
        }

        public IEnumerable<string> GetCommandsAsStrings()
        {
            return SelectedClient.ClientCallback.GetCommandsAsStrings();
        }

        public UserAccount GetLoggedInUser()
        {
            return SelectedClient.ClientCallback.GetLoggedInUser();
        }

        public IDirectory GetRemoteFiles(string path, bool useRequest)
        {
            try
            {
                return SelectedClient.ClientCallback.GetRemoteFiles(path, useRequest);
            }
            catch (FaultException<ServerFault> ex)
            {
                throw new FaultException<ProxyFault>(new ProxyFault(SelectedClient.UniqueID), ex.Message);
            }
        }

        public ScriptGlobalInformation[] GetScriptGlobals()
        {
            return SelectedClient.ClientCallback.GetScriptGlobals();
        }

        public List<string> GetServerRoleNames()
        {
            return SelectedClient.ClientCallback.GetServerRoleNames();
        }

        public Guid[] GetServers()
        {
            return ConnectedServers.Select(s => s.UniqueID).ToArray();
        }

        public ServerSettings GetServerSettings()
        {
            return SelectedClient.ClientCallback.GetServerSettings();
        }

        public void PlaySound(string FileName)
        {
            SelectedClient.ClientCallback.PlaySound(FileName);
        }

        public void PlaySoundLoop(string FileName)
        {
            SelectedClient.ClientCallback.PlaySoundLoop(FileName);
        }

        public void PlaySoundSync(string FileName)
        {
            SelectedClient.ClientCallback.PlaySoundSync(FileName);
        }

        public void ProxyDisconnect()
        {
            GlobalServices.Logger.Log($"Client [{ProxyClient.UniqueID}] disconnected from proxy server. Proxy server notifying connected servers that the client has disconnected.", LogLevel.Info);
            foreach(Client<IRemoteWithProxy> client in ConnectedServers)
            {
                client.ClientCallback.Disconnect();
                GlobalServices.Logger.Log($"Server [{client.UniqueID}] notified of client disconnection.", LogLevel.Info);
            }
        }

        public void ProxyRegister()
        {
            var callback = OperationContext.Current.GetCallbackChannel<IRemoteClient>();
            ProxyClient = Client<IRemoteClient>.Build(callback.RegisterClient(), callback);
            if(ProxyClient.ClientType == ClientType.CommandLine)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("=================================================================================");
                sb.AppendLine($"                        Welcome to RemotePlus Proxy                             ");
                sb.AppendLine();
                sb.AppendLine($"Version: {Assembly.GetExecutingAssembly().GetName().Version}                    ");
                sb.AppendLine("YOU ARE LIABLE FOR ANY DAMAGES OR ILLEGAL ACTIVITIES USING THIS PROGRAM. MAKE    ");
                sb.AppendLine("SURE YOU ARE AUTHORIZED TO LOAD REMOTEPLUS ONTO ANY SYSTEM THAT ISN'T YOURS.     ");
                sb.AppendLine("WE AREN'T RESPONSIBLE FOR ANY DATA LOSS DUE TO A CUSTOM EXTENSION OR SCRIPT.     ");
                sb.AppendLine("If you found a bug in the RemotePlus code, please open an issue at               ");
                sb.AppendLine("http://github.com/brianb12321/RemotePlus");
                sb.AppendLine("=================================================================================");
                sb.AppendLine();
                sb.AppendLine($"There are {ConnectedServers.Count} server(s) connected to the proxy server.");
                sb.AppendLine("To view all the servers connected, enter {proxyViewServers} into the console.");
                sb.AppendLine("To switch the selected server, enter {proxySwitchServer} into the console.");
                sb.AppendLine("Any command that is not part of the proxy server will be executed on the selected server.");
                sb.AppendLine($"Proxy server GUID: {ProxyManager.ProxyGuid}");
                sb.AppendLine();
                sb.AppendLine();
                ProxyClient.ClientCallback.TellMessageToServerConsole(ProxyManager.ProxyGuid, sb.ToString());
                ProxyClient.ClientCallback.ChangePrompt(ProxyManager.ProxyGuid, new RemotePlusLibrary.Extension.CommandSystem.PromptBuilder()
                {
                    Path = "",
                    CurrentUser = "",
                    AdditionalData = "Proxy"
                });
                ProxyClient.Channel = OperationContext.Current.Channel;
                ProxyClient.Channel.Faulted += (sender, e) =>
                {
                    GlobalServices.Logger.Log($"Client [{ProxyClient.UniqueID}] is now in the faulted state.", LogLevel.Error);
                    ProxyClient = null;
                };
                GlobalServices.Logger.Log($"Client [{ProxyClient.UniqueID}] is now registered.", LogLevel.Info);
                ProxyClient.ClientCallback.RegistirationComplete(ProxyManager.ProxyGuid);
            }
        }

        public string ReadFileAsString(string fileName)
        {
            return SelectedClient.ClientCallback.ReadFileAsString(fileName);
        }

        public void Register()
        {
            var callback = OperationContext.Current.GetCallbackChannel<IRemoteWithProxy>();
            SessionClient<IRemoteWithProxy> tempClient = SessionClient<IRemoteWithProxy>.BuildSessionClient(new ClientBuilder(ClientType.Server), callback);
            tempClient.SessionId = OperationContext.Current.Channel.SessionId;
            ConnectedServers.Add(tempClient);
            tempClient.Channel = OperationContext.Current.Channel;
            if(SelectedClient == null)
            {
                SelectedClient = tempClient;
            }
            tempClient.Channel.Faulted += (sender, e) =>
            {
                var closedClient = ConnectedServers.First(s => s.Channel == tempClient.Channel);
                GlobalServices.Logger.Log($"Server [{closedClient.UniqueID}] closed without proper shutdown.", LogLevel.Info);
                ConnectedServers.Remove(closedClient);
                if (SelectedClient == closedClient)
                {
                    Task.Run(() => ProxyClient.ClientCallback.RequestInformation(ProxyManager.ProxyGuid, RequestBuilder.RequestMessageBox($"Server [{SelectedClient.UniqueID}] has disconnected without proper shutdown. Please select another server to be the active server.", "Proxy Server", MessageBoxButtons.OK, MessageBoxIcon.Information)));
                }
            };
            tempClient.ClientCallback.ServerRegistered(tempClient.UniqueID);
            GlobalServices.Logger.Log($"Server [{tempClient.UniqueID}] joined the proxy cluster.", LogLevel.Info);
        }

        public void Register(RegisterationObject Settings)
        {
            SelectedClient.ClientCallback.Register(Settings);
        }

        public void Restart()
        {
            SelectedClient.ClientCallback.Restart();
        }

        public void RunProgram(string Program, string Argument)
        {
            SelectedClient.ClientCallback.RunProgram(Program, Argument);
        }

        public CommandPipeline RunServerCommand(string Command, CommandExecutionMode commandMode)
        {
            return SelectedClient.ClientCallback.RunServerCommand(Command, commandMode);
        }

        public void SelectServer(int serverPosition)
        {
            try
            {
                SelectedClient = ConnectedServers[serverPosition];
                if (ProxyClient.ClientType == ClientType.CommandLine)
                {
                    ProxyClient.ClientCallback.TellMessageToServerConsole(ProxyManager.ProxyGuid, $"Server {serverPosition} is now active.", LogLevel.Info, "Proxy Server");
                    ProxyClient.ClientCallback.ChangePrompt(ProxyManager.ProxyGuid, new RemotePlusLibrary.Extension.CommandSystem.PromptBuilder()
                    {
                        AdditionalData = $"Server {serverPosition}"
                    });
                }
            }
            catch (ArgumentOutOfRangeException)
            {
                ProxyClient.ClientCallback.TellMessageToServerConsole(ProxyManager.ProxyGuid, "The requested server is not connected.");
            }
        }

        public void SelectServer(Guid guid)
        {
            SelectedClient = ConnectedServers.First(s => s.UniqueID == guid);
            if (SelectedClient == null)
            {
                ProxyClient.ClientCallback.TellMessageToServerConsole(ProxyManager.ProxyGuid, "The requested server is not connected.");
            }
            else
            {
                if (ProxyClient.ClientType == ClientType.CommandLine)
                {
                    SelectedClient.ClientCallback.Register(new RegisterationObject());
                    ProxyClient.ClientCallback.TellMessageToServerConsole(ProxyManager.ProxyGuid, $"Server {guid} is now active.", LogLevel.Info, "Proxy Server");
                    ProxyClient.ClientCallback.ChangePrompt(ProxyManager.ProxyGuid, new RemotePlusLibrary.Extension.CommandSystem.PromptBuilder()
                    {
                        AdditionalData = $"Server {guid}"
                    });
                }
            }
        }

        public DialogResult ShowMessageBox(string Message, string Caption, MessageBoxIcon Icon, MessageBoxButtons Buttons)
        {
            return SelectedClient.ClientCallback.ShowMessageBox(Message, Caption, Icon, Buttons);
        }

        public void Speak(string Message, VoiceGender Gender, VoiceAge Age)
        {
            SelectedClient.ClientCallback.Speak(Message, Gender, Age);
        }

        public void SwitchUser()
        {
            SelectedClient.ClientCallback.SwitchUser();
        }

        public void UpdateServerSettings(ServerSettings Settings)
        {
            SelectedClient.ClientCallback.UpdateServerSettings(Settings);
        }


        public void Disconnect(Guid guid, string Reason)
        {
            ProxyClient.ClientCallback.Disconnect(SelectedClient.UniqueID, Reason);
        }
        public RemotePlusLibrary.Extension.CommandSystem.PromptBuilder GetCurrentPrompt()
        {
            return ProxyClient.ClientCallback.GetCurrentPrompt();
        }
        public ClientBuilder RegisterClient()
        {
            return ProxyClient.ClientCallback.RegisterClient();
        }

        public void RegistirationComplete(Guid guid)
        {
            ProxyClient.ClientCallback.RegistirationComplete(SelectedClient.UniqueID);
        }
        public UserCredentials RequestAuthentication(Guid guid, AuthenticationRequest Request)
        {
            return ProxyClient.ClientCallback.RequestAuthentication(SelectedClient.UniqueID, Request);
        }

        public ReturnData RequestInformation(Guid guid, RequestBuilder builder)
        {
            return ProxyClient.ClientCallback.RequestInformation(SelectedClient.UniqueID, builder);
        }
        public void TellMessage(Guid guid, string Message, LogLevel o)
        {
            ProxyClient.ClientCallback.TellMessage(SelectedClient.UniqueID, Message, o);
        }

        public void TellMessageToServerConsole(Guid guid, string Message)
        {
            ProxyClient.ClientCallback.TellMessageToServerConsole(SelectedClient.UniqueID, Message);
        }

        public void TellMessageToServerConsole(Guid guid, ConsoleText text)
        {
            ProxyClient.ClientCallback.TellMessageToServerConsole(SelectedClient.UniqueID, text);
        }

        public void SendSignal(Guid guid, SignalMessage signal)
        {
            ProxyClient.ClientCallback.SendSignal(SelectedClient.UniqueID, signal);
        }

        public void ChangePrompt(Guid guid, RemotePlusLibrary.Extension.CommandSystem.PromptBuilder newPrompt)
        {
            ProxyClient.ClientCallback.ChangePrompt(SelectedClient.UniqueID, newPrompt);
        }

        public Guid GetSelectedServerGuid()
        {
            return SelectedClient.UniqueID;
        }
        #region Command Methods
        public CommandPipeline ExecuteProxyCommand(string Command, CommandExecutionMode mode)
        {
            CommandPipeline pipe = new CommandPipeline();
            int pos = 0;
            if (Command.StartsWith("::"))
            {
                try
                {
                    ProxyManager.ScriptBuilder.ExecuteStringUsingSameScriptScope(Command.TrimStart(':'));
                }
                catch (Exception ex)
                {
                    ProxyClient.ClientCallback.TellMessageToServerConsole(ProxyManager.ProxyGuid, $"Could not execute script file: {ex.Message}", LogLevel.Error, ScriptBuilder.SCRIPT_LOG_CONSTANT);
                }
            }
            //Sends a script command to the selected server.
            else if(Command.StartsWith("=>::"))
            {
                try
                {
                    return SelectedClient.ClientCallback.RunServerCommand(Command.Remove(0, 3), CommandExecutionMode.Client);
                }
                catch (Exception ex)
                {
                    ProxyClient.ClientCallback.TellMessageToServerConsole(SelectedClient.UniqueID, $"Could not execute script file: {ex.Message}", LogLevel.Error, ScriptBuilder.SCRIPT_LOG_CONSTANT);
                }
            }
            else
            {
                try
                {
                    IParser parser = new CommandParser(Command);
                    try
                    {
                        var tokens = parser.Parse(true);
                        var newTokens = RunSubRoutines(parser, pipe, pos);
                        foreach (CommandToken token in newTokens)
                        {
                            foreach (List<CommandToken> allTokens in parser.ParsedTokens)
                            {
                                var index = allTokens.IndexOf(token);
                                if (index != -1)
                                {
                                    parser.ParsedTokens[parser.ParsedTokens.IndexOf(allTokens)][index] = token;
                                }
                            }
                        }
                        var newVariableTokens = RunVariableReplacement(parser, out bool success);
                        if (success != true)
                        {
                            return pipe;
                        }
                        foreach (CommandToken token in newVariableTokens)
                        {
                            foreach (List<CommandToken> allTokens in parser.ParsedTokens)
                            {
                                var index = allTokens.IndexOf(token);
                                if (index != -1)
                                {
                                    parser.ParsedTokens[parser.ParsedTokens.IndexOf(allTokens)][index] = token;
                                }
                            }
                        }
                        var newQouteTokens = ParseOutQoutes(parser);
                        foreach (CommandToken token in newQouteTokens)
                        {
                            foreach (List<CommandToken> allTokens in parser.ParsedTokens)
                            {
                                var index = allTokens.IndexOf(token);
                                if (index != -1)
                                {
                                    parser.ParsedTokens[parser.ParsedTokens.IndexOf(allTokens)][index] = token;
                                }
                            }
                        }
                        //Run the commands
                        foreach (List<CommandToken> commands in parser.ParsedTokens)
                        {
                            var request = new CommandRequest(commands.ToArray());
                            var routine = new CommandRoutine(request, ProxyManager.Execute(request, mode, pipe));
                            if (routine.Output.ResponseCode == 3131)
                            {
                                RunServerCommand(Command, CommandExecutionMode.Client);
                            }
                            pipe.Add(pos++, routine);
                        }
                    }
                    catch (ParserException e)
                    {
                        string parseErrorMessage = $"Unable to parse command: {e.Message}";
                        GlobalServices.Logger.Log(parseErrorMessage, LogLevel.Error, "Server Host");
                        ProxyClient.ClientCallback.TellMessageToServerConsole(ProxyManager.ProxyGuid, parseErrorMessage, LogLevel.Error);
                        return pipe;
                    }
                }
                catch (Exception ex)
                {
                    ProxyClient.ClientCallback.TellMessageToServerConsole(SelectedClient.UniqueID, $"There was an error in the command: {ex.Message}", LogLevel.Error, "Proxy Server");
                    return pipe;
                }
            }
            return pipe;
        }
        private CommandToken[] RunVariableReplacement(IParser p, out bool success)
        {
            success = true;
            List<CommandToken> tokenList = new List<CommandToken>();
            var variableTokens = p.GetVariables();
            foreach (CommandToken variableToken in variableTokens)
            {
                var variablename = variableToken.OriginalValue.Remove(0, 1);
                if (ProxyManager.ProxyService.Variables.ContainsKey(variablename))
                {
                    var variableValue = ProxyManager.ProxyService.Variables[variablename];

                    variableToken.Value = variableValue;
                    success = true;
                    tokenList.Add(variableToken);
                }
                else
                {
                    ProxyClient.ClientCallback.TellMessageToServerConsole(ProxyManager.ProxyGuid, $"Variable {variablename} does not exist", LogLevel.Error, "Proxy Server");
                    success = false;
                }
            }
            return tokenList.ToArray();
        }
        private CommandToken[] RunVariableReplacement(CommandToken[] tokens, out bool success)
        {
            success = true;
            List<CommandToken> tokenList = new List<CommandToken>();
            foreach (CommandToken variableToken in tokens)
            {
                var variablename = variableToken.OriginalValue.Remove(0, 1);
                if (ProxyManager.ProxyService.Variables.ContainsKey(variablename))
                {
                    var variableValue = ProxyManager.ProxyService.Variables[variablename];

                    variableToken.Value = variableValue;
                    success = true;
                    tokenList.Add(variableToken);
                }
                else
                {
                    ProxyClient.ClientCallback.TellMessageToServerConsole(ProxyManager.ProxyGuid, $"Variable {variablename} does not exist", LogLevel.Error, "Proxy Server");
                    success = false;
                }
            }
            return tokenList.ToArray();
        }
        private IEnumerable<CommandToken> RunSubRoutines(IParser p, CommandPipeline pipe, int position)
        {
            foreach (CommandToken routineToken in p.GetSubRoutines())
            {
                var commandToExecute = Regex.Match(routineToken.OriginalValue, CommandToken.SUBROUTINE_PATTERN).Groups[1].Value;
                var parsedCommand = p.Parse(commandToExecute, false);
                var newTokens = RunSubRoutines(p.GetSubRoutines(parsedCommand), p, pipe, position);
                foreach (CommandToken token in newTokens)
                {
                    foreach (List<CommandToken> allTokens in parsedCommand)
                    {
                        var index = allTokens.IndexOf(token);
                        if (index != -1)
                        {
                            parsedCommand[parsedCommand.IndexOf(allTokens)][index] = token;
                        }
                    }
                }
                var newVariableTokens = RunVariableReplacement(p.GetVariables(parsedCommand), out bool success);
                if (success != true)
                {
                    yield return routineToken;
                }
                foreach (CommandToken token in newVariableTokens)
                {
                    foreach (List<CommandToken> allTokens in parsedCommand)
                    {
                        var index = allTokens.IndexOf(token);
                        if (index != -1)
                        {
                            parsedCommand[parsedCommand.IndexOf(allTokens)][index] = token;
                        }
                    }
                }
                var newQouteTokens = ParseOutQoutes(p.GetQoutedToken(parsedCommand));
                foreach (CommandToken token in newQouteTokens)
                {
                    foreach (List<CommandToken> allTokens in parsedCommand)
                    {
                        var index = allTokens.IndexOf(token);
                        if (index != -1)
                        {
                            parsedCommand[parsedCommand.IndexOf(allTokens)][index] = token;
                        }
                    }
                }
                foreach (List<CommandToken> allCommands in parsedCommand)
                {
                    var request = new CommandRequest(allCommands.ToArray());
                    var routine = new CommandRoutine(request, ProxyManager.Execute(request, CommandExecutionMode.Client, pipe));
                    routineToken.Value = routine.Output.CustomStatusMessage;
                    position++;
                }
                yield return routineToken;
            }
        }
        private IEnumerable<CommandToken> RunSubRoutines(CommandToken[] tokens, IParser p, CommandPipeline pipe, int position)
        {
            foreach (CommandToken routineToken in tokens)
            {
                var commandToExecute = Regex.Match(routineToken.OriginalValue, CommandToken.SUBROUTINE_PATTERN).Groups[1].Value;
                var parsedCommand = p.Parse(commandToExecute, false);

                var newVariableTokens = RunVariableReplacement(p.GetVariables(parsedCommand), out bool success);
                if (success != true)
                {
                    yield return routineToken;
                }
                foreach (CommandToken token in newVariableTokens)
                {
                    foreach (List<CommandToken> allTokens in parsedCommand)
                    {
                        var index = allTokens.IndexOf(token);
                        if (index != -1)
                        {
                            parsedCommand[parsedCommand.IndexOf(allTokens)][index] = token;
                        }
                    }
                }
                var newQouteTokens = ParseOutQoutes(p.GetQoutedToken(parsedCommand));
                foreach (CommandToken token in newQouteTokens)
                {
                    foreach (List<CommandToken> allTokens in parsedCommand)
                    {
                        var index = allTokens.IndexOf(token);
                        if (index != -1)
                        {
                            parsedCommand[parsedCommand.IndexOf(allTokens)][index] = token;
                        }
                    }
                }
                foreach (List<CommandToken> allCommands in parsedCommand)
                {
                    var request = new CommandRequest(allCommands.ToArray());
                    var routine = new CommandRoutine(request, ProxyManager.Execute(request, CommandExecutionMode.Client, pipe));
                    routineToken.Value = routine.Output.CustomStatusMessage;
                    position++;
                }
                yield return routineToken;
            }
        }
        private IEnumerable<CommandToken> ParseOutQoutes(CommandToken[] tokens)
        {
            List<CommandToken> tokenList = new List<CommandToken>();
            foreach (CommandToken qouteToken in tokens)
            {
                var qouteName = Regex.Match(qouteToken.OriginalValue, CommandToken.QOUTE_PATTERN).Groups[1].Value;
                qouteToken.Value = qouteName.Replace('^', '&');
                tokenList.Add(qouteToken);
            }
            return tokenList.ToArray();
        }
        private IEnumerable<CommandToken> ParseOutQoutes(IParser p)
        {
            List<CommandToken> tokenList = new List<CommandToken>();
            var qouteTokens = p.GetQoutedToken();
            foreach (CommandToken qouteToken in qouteTokens)
            {
                var qouteName = Regex.Match(qouteToken.OriginalValue, CommandToken.QOUTE_PATTERN).Groups[1].Value;
                qouteToken.Value = qouteName.Replace('^', '&');
                tokenList.Add(qouteToken);
            }
            return tokenList.ToArray();
        }
        #endregion
        public void Leave(Guid serverGuid)
        {
            var foundServer = ConnectedServers.First(s => s.UniqueID == serverGuid);
            if(foundServer != null)
            {
                GlobalServices.Logger.Log($"Server [{foundServer.UniqueID}] disconnected gracefully.", LogLevel.Info);
                ConnectedServers.Remove(foundServer);
                //Notify client that the active server has disconnected gracefully.
                if(SelectedClient == foundServer)
                {
                    Task.Run(() => ProxyClient.ClientCallback.RequestInformation(ProxyManager.ProxyGuid, RequestBuilder.RequestMessageBox($"Server [{SelectedClient.UniqueID}] has disconnected gracefully. Please select another server to be the active server.", "Proxy Server", MessageBoxButtons.OK, MessageBoxIcon.Information)));
                }
            }
        }

        public void TellMessageToServerConsole(Guid serverGuid, string Message, LogLevel level)
        {
            ProxyClient.ClientCallback.TellMessageToServerConsole(serverGuid, Message, level);
        }

        public void TellMessageToServerConsole(Guid serverGuid, string Message, LogLevel level, string from)
        {
            ProxyClient.ClientCallback.TellMessageToServerConsole(serverGuid, Message, level, from);
        }
    }
}
