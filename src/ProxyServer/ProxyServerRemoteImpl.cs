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
            ProxyManager.Logger.AddOutput($"Client [{ProxyClient.UniqueID}] disconnected from proxy server. Proxy server notifying connected servers that the client has disconnected.", OutputLevel.Info);
            foreach(Client<IRemoteWithProxy> client in ConnectedServers)
            {
                client.ClientCallback.Disconnect();
                ProxyManager.Logger.AddOutput($"Server [{client.UniqueID}] notified of client disconnection.", OutputLevel.Info);
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
                    ProxyManager.Logger.AddOutput($"Client [{ProxyClient.UniqueID}] is now in the faulted state.", OutputLevel.Error);
                    ProxyClient = null;
                };
                ProxyManager.Logger.AddOutput($"Client [{ProxyClient.UniqueID}] is now registered.", OutputLevel.Info);
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
                ProxyManager.Logger.AddOutput($"Server [{closedClient.UniqueID}] closed without proper shutdown.", OutputLevel.Info);
                ConnectedServers.Remove(closedClient);
                if (SelectedClient == closedClient)
                {
                    Task.Run(() => ProxyClient.ClientCallback.RequestInformation(ProxyManager.ProxyGuid, RequestBuilder.RequestMessageBox($"Server [{SelectedClient.UniqueID}] has disconnected without proper shutdown. Please select another server to be the active server.", "Proxy Server", MessageBoxButtons.OK, MessageBoxIcon.Information)));
                }
            };
            tempClient.ClientCallback.ServerRegistered(tempClient.UniqueID);
            ProxyManager.Logger.AddOutput($"Server [{tempClient.UniqueID}] joined the proxy cluster.", Logging.OutputLevel.Info);
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
                    ProxyClient.ClientCallback.TellMessageToServerConsole(ProxyManager.ProxyGuid, new UILogItem(OutputLevel.Info, $"Server {serverPosition} is now active.", "Proxy Server"));
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
                    ProxyClient.ClientCallback.TellMessageToServerConsole(ProxyManager.ProxyGuid, new UILogItem(OutputLevel.Info, $"Server {guid} is now active.", "Proxy Server"));
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
        public void TellMessage(Guid guid, string Message, OutputLevel o)
        {
            ProxyClient.ClientCallback.TellMessage(SelectedClient.UniqueID, Message, o);
        }

        public void TellMessage(Guid guid, UILogItem li)
        {
            ProxyClient.ClientCallback.TellMessage(SelectedClient.UniqueID, li);
        }

        public void TellMessage(Guid guid, UILogItem[] Logs)
        {
            ProxyClient.ClientCallback.TellMessage(SelectedClient.UniqueID, Logs);
        }

        public void TellMessageToServerConsole(Guid guid, UILogItem li)
        {
            ProxyClient.ClientCallback.TellMessageToServerConsole(SelectedClient.UniqueID, li);
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
            try
            {
                CommandParser parser = new CommandParser(Command);
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
                        if(routine.Output.ResponseCode == 3131)
                        {
                            RunServerCommand(Command, CommandExecutionMode.Client);
                        }
                        pipe.Add(pos++, routine);
                    }
                }
                catch (ParserException e)
                {
                    UILogItem parseMessage = new UILogItem(OutputLevel.Error, $"Unable to parse command: {e.Message}");
                    parseMessage.From = "Server Host";
                    ProxyManager.Logger.AddOutput(parseMessage.Message, parseMessage.Level, parseMessage.From);
                    ProxyClient.ClientCallback.TellMessageToServerConsole(ProxyManager.ProxyGuid, parseMessage);
                    return pipe;
                }
            }
            catch (Exception ex)
            {
                ProxyClient.ClientCallback.TellMessageToServerConsole(SelectedClient.UniqueID, new UILogItem(OutputLevel.Error, $"There was an error in the command: {ex.Message}", "Proxy Server"));
                return pipe;
            }
            return pipe;
        }
        private CommandToken[] RunVariableReplacement(CommandParser p, out bool success)
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
                    ProxyClient.ClientCallback.TellMessageToServerConsole(ProxyManager.ProxyGuid, new UILogItem(OutputLevel.Error, $"Variable {variablename} does not exist", "Proxy Server"));
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
                    ProxyClient.ClientCallback.TellMessageToServerConsole(ProxyManager.ProxyGuid, new UILogItem(OutputLevel.Error, $"Variable {variablename} does not exist", "Proxy Server"));
                    success = false;
                }
            }
            return tokenList.ToArray();
        }

        private IEnumerable<CommandToken> RunSubRoutines(CommandParser p, CommandPipeline pipe, int position)
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
        private IEnumerable<CommandToken> RunSubRoutines(CommandToken[] tokens, CommandParser p, CommandPipeline pipe, int position)
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
        private IEnumerable<CommandToken> ParseOutQoutes(CommandParser p)
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
                ProxyManager.Logger.AddOutput($"Server [{foundServer.UniqueID}] disconnected gracefully.", OutputLevel.Info);
                ConnectedServers.Remove(foundServer);
                //Notify client that the active server has disconnected gracefully.
                if(SelectedClient == foundServer)
                {
                    Task.Run(() => ProxyClient.ClientCallback.RequestInformation(ProxyManager.ProxyGuid, RequestBuilder.RequestMessageBox($"Server [{SelectedClient.UniqueID}] has disconnected gracefully. Please select another server to be the active server.", "Proxy Server", MessageBoxButtons.OK, MessageBoxIcon.Information)));
                }
            }
        }
    }
}
