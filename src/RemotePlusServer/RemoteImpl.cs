using System;
using System.Collections.Generic;
using System.ServiceModel;
using RemotePlusLibrary;
using System.Windows.Forms;
using Logging;
using System.Speech.Synthesis;
using RemotePlusLibrary.Core;
using System.Diagnostics;
using RemotePlusLibrary.Extension.CommandSystem;
using System.IO;
using RemotePlusLibrary.Extension.CommandSystem.CommandClasses;
using RemotePlusServer.ExtensionSystem;
using System.Text.RegularExpressions;
using RemotePlusServer.Internal;
using System.Linq;
using RemotePlusLibrary.Scripting;
using RemotePlusLibrary.Security.AccountSystem;
using RemotePlusLibrary.Discovery;
using RemotePlusLibrary.Core.Faults;
using RemotePlusLibrary.FileTransfer.BrowserClasses;
using RemotePlusLibrary.Contracts;
using RemotePlusLibrary.Client;
using RemotePlusLibrary.Security.Authentication;
using RemotePlusLibrary.Extension.CommandSystem.CommandClasses.Parsing;
using RemotePlusLibrary.Configuration.ServerSettings;

namespace RemotePlusServer
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple,
        InstanceContextMode = InstanceContextMode.Single,
        IncludeExceptionDetailInFaults = true,
        UseSynchronizationContext = false,
        MaxItemsInObjectGraph = int.MaxValue)]
    [CallbackBehavior(IncludeExceptionDetailInFaults = true,
        ConcurrencyMode = ConcurrencyMode.Multiple,
        UseSynchronizationContext = false,
        MaxItemsInObjectGraph = int.MaxValue)]
    [GlobalException(typeof(GlobalErrorHandler))]
    public class RemoteImpl : IRemote, IRemoteWithProxy
    {
        const string OPERATION_COMPLETED = "Operation_Completed";
        public RemoteImpl()
        {

        }
        internal void Setup()
        {

        }
        public RegisterationObject Settings { get; private set; }
        public Client<RemoteClient> Client { get; set; }
        public bool Registered { get; private set; }
        public UserAccount LoggedInUser { get; private set; }
        public string CurrentPath = Environment.CurrentDirectory;
        bool CheckRegisteration(string Action)
        {
            var l = ServerManager.Logger.AddOutput($"Checking registiration for action {Action}.", OutputLevel.Info);
            if (!Registered)
            {
                ServerManager.Logger.AddOutput("The client is not registired to the server.", OutputLevel.Error);
                if (ServerManager.proxyChannelFactory.State == CommunicationState.Opened)
                {
                    ServerManager.proxyChannel.TellMessage(Guid.NewGuid(), new UILogItem(OutputLevel.Error, "You must be registered.", "Server Host"));
                    return false;
                }
                else
                {
                    OperationContext.Current.GetCallbackChannel<IRemoteClient>().Disconnect(Guid.NewGuid(), "You must be registered.");
                    return true;
                }
            }
            else
            {
                return true;
            }
        }
        public void Beep(int Hertz, int Duration)
        {
            if (CheckRegisteration("beep"))
            {
                if (LoggedInUser.Role.AttachedPolicyObject.Policies.FindSubFolder("Operations").Policies.First(p => p.ShortName == "EnableConsole").Values["Value"] != "True")
                {
                    Client.ClientCallback.TellMessage("You do not have promission to use the beep function.", OutputLevel.Info);
                }
                else
                {
                    Console.Beep(Hertz, Duration);
                    Client.ClientCallback.TellMessage($"Console beeped. Hertz: {Hertz}, Duration: {Duration}", OutputLevel.Info);
                }
            }
            // OperationContext.Current.OperationCompleted += (sender, e) => Client.ClientCallback.SendSignal(new SignalMessage(OPERATION_COMPLETED, ""));
        }

        public void PlaySound(string FileName)
        {
            if(CheckRegisteration("PlaySound"))
            {
                if (LoggedInUser.Role.AttachedPolicyObject.Policies.FindSubFolder("Operations").Policies.First(p => p.ShortName == "EnableConsole").Values["Value"] != "True")
                {
                    Client.ClientCallback.TellMessage("You do not have promission to use the PlaySound function.", OutputLevel.Info);
                }
                else
                {
                    System.Media.SoundPlayer sp = new System.Media.SoundPlayer(FileName);
                    sp.Play();
                }
            }
        }

        public void PlaySoundLoop(string FileName)
        {
            if (CheckRegisteration("PlaySoundLoop"))
            {
                if (LoggedInUser.Role.AttachedPolicyObject.Policies.FindSubFolder("Operations").Policies.First(p => p.ShortName == "EnableConsole").Values["Value"] != "True")
                {
                    Client.ClientCallback.TellMessage("You do not have promission to use the CanPlaySoundLoop function.", OutputLevel.Info);
                }
                else
                {
                    System.Media.SoundPlayer sp = new System.Media.SoundPlayer(FileName);
                    sp.PlayLooping();
                }
            }
            // OperationContext.Current.OperationCompleted += (sender, e) => Client.ClientCallback.SendSignal(new SignalMessage(OPERATION_COMPLETED, ""));
        }

        public void PlaySoundSync(string FileName)
        {
            if (CheckRegisteration("PlaySoundSync"))
            {
                if (LoggedInUser.Role.AttachedPolicyObject.Policies.FindSubFolder("Operations").Policies.First(p => p.ShortName == "EnableConsole").Values["Value"] != "True")
                {
                    Client.ClientCallback.TellMessage("You do not have promission to use the CanPlaySoundSync function.", OutputLevel.Info);
                }
                else
                {
                    System.Media.SoundPlayer sp = new System.Media.SoundPlayer(FileName);
                    sp.PlaySync();
                }
            }
            // OperationContext.Current.OperationCompleted += (sender, e) => Client.ClientCallback.SendSignal(new SignalMessage(OPERATION_COMPLETED, ""));
        }

        public void Register(RegisterationObject Settings)
        {
            const string REG_FAILED = "Registiration failed. The most likely cause is invalid credentials or this account has been blocked. Make sure that the provided credentials are correct and also make sure the account was not blocked. If you still receive this error message, please check the server logs for more details.";
            ServerManager.Logger.AddOutput("A new client is awaiting registiration.", OutputLevel.Info);
            ServerManager.Logger.AddOutput("Instanitiating callback object.", OutputLevel.Debug);
            ServerManager.Logger.AddOutput("Getting ClientBuilder from client.", OutputLevel.Debug);

            if (ServerManager.DefaultSettings.DiscoverySettings.DiscoveryBehavior == RemotePlusLibrary.Configuration.ServerSettings.ProxyConnectionMode.Connect)
            {
                Client = Client<RemoteClient>.Build(ServerManager.proxyChannel.RegisterClient(), new RemoteClient(null, true));
            }
            else
            {
                var callback = OperationContext.Current.GetCallbackChannel<IRemoteClient>();
                Client = Client<RemoteClient>.Build(callback.RegisterClient(), new RemoteClient(callback, false));
            }
            ServerManager.Logger.AddOutput("Received registiration object from client.", OutputLevel.Info);
            this.Settings = Settings;
            var l = ServerManager.Logger.AddOutput("Processing registiration object.", OutputLevel.Debug);
            Client.ClientCallback.TellMessage(new UILogItem(l.Level, l.Message, l.From));
            if (Settings.LoginRightAway)
            {
                var account = LogIn(Settings.Credentials);
                if (account == null)
                {
                    ServerManager.Logger.AddOutput($"Client {Client.FriendlyName} [{Client.UniqueID.ToString()}] disconnected. Failed to register to the server. Authentication failed.", OutputLevel.Info);
                    throw new FaultException(REG_FAILED + $" Provded username: {Settings.Credentials.Username}");
                }
                else
                {
                    LoggedInUser = account;
                    UpdateAccountPolicy();
                    RegisterComplete();
                }
            }
            else
            {
                var l3 = ServerManager.Logger.AddOutput("Awaiting credentials from the client.", OutputLevel.Info);
                Client.ClientCallback.TellMessage(new UILogItem(l3.Level, l3.Message, l3.From));
                UserCredentials upp = Client.ClientCallback.RequestAuthentication(new AuthenticationRequest(AuthenticationSeverity.Normal) { Reason = "The server requires credentials to register." });
                var account = LogIn(upp);
                if (account == null)
                {
                    ServerManager.Logger.AddOutput($"Client {Client.FriendlyName} [{Client.UniqueID.ToString()}] disconnected. Failed to register to the server. Authentication failed.", OutputLevel.Info);
                    throw new FaultException(REG_FAILED + $" Provded username: {upp.Username}");
                }
                else
                {
                    LoggedInUser = account;
                    UpdateAccountPolicy();
                    RegisterComplete();
                }
            }
            _HookManager.RunHooks(ServerLibraryBuilder.LOGIN_HOOK, new RemotePlusLibrary.Extension.HookSystem.HookArguments(ServerLibraryBuilder.LOGIN_HOOK));
            if (Client.ClientType == ClientType.CommandLine && ServerManager.proxyChannelFactory == null)
            {
                Client.ClientCallback.ChangePrompt(new RemotePlusLibrary.Extension.CommandSystem.PromptBuilder()
                {
                    Path = CurrentPath,
                    AdditionalData = "Current Path",
                    CurrentUser = LoggedInUser.Credentials.Username
                });
            }
            // OperationContext.Current.OperationCompleted += (sender, e) => Client.ClientCallback.SendSignal(new SignalMessage(OPERATION_COMPLETED, ""));
        }

        private void UpdateAccountPolicy()
        {
            LoggedInUser.Role.BuildPolicyObject();
        }

        private void RegisterComplete()
        {
            ServerManager.Logger.AddOutput($"Client \"{Client.FriendlyName}\" [{Client.UniqueID}] Type: {Client.ClientType} registired.", Logging.OutputLevel.Info);
            Registered = true;
            Client.ClientCallback.TellMessage("Registiration complete.", Logging.OutputLevel.Info);
            Client.ClientCallback.RegistirationComplete();
        }

        public void RunProgram(string Program, string Argument)
        {
            if (CheckRegisteration("RunProgram"))
            {
                if (LoggedInUser.Role.AttachedPolicyObject.Policies.FindSubFolder("Operations").Policies.First(p => p.ShortName == "EnableConsole").Values["Value"] != "True")
                {
                    Client.ClientCallback.TellMessage("You do not have promission to use the CanRunProgram function.", OutputLevel.Info);
                }
                else
                {
                    ServerManager.Logger.AddOutput("Creating process component.", OutputLevel.Debug);
                    Process p = new Process();
                    ServerManager.Logger.AddOutput($"File to execute: {Program}", OutputLevel.Debug);
                    p.StartInfo.FileName = Program;
                    ServerManager.Logger.AddOutput($"File arguments: {Argument}", OutputLevel.Debug);
                    p.StartInfo.Arguments = Argument;
                    ServerManager.Logger.AddOutput($"Shell execution is disabled.", OutputLevel.Debug);
                    p.StartInfo.UseShellExecute = false;
                    ServerManager.Logger.AddOutput($"Error stream will be redirected.", OutputLevel.Debug);
                    p.StartInfo.RedirectStandardError = true;
                    ServerManager.Logger.AddOutput($"Standord stream will be redirected.", OutputLevel.Debug);
                    p.StartInfo.RedirectStandardOutput = true;
                    p.ErrorDataReceived += (sender, e) =>
                    {
                        if (e.Data != null)
                        {
                            if (Client.ExtraData.TryGetValue("ps_appendNewLine", out string val))
                            {
                                if (val == "false")
                                {
                                    Client.ClientCallback.TellMessageToServerConsole(e.Data);
                                }
                                else
                                {
                                    Client.ClientCallback.TellMessageToServerConsole(new UILogItem(OutputLevel.Error, $"Extra data for appendText is invalid. Value: {val}", "Server Host"));
                                }
                            }
                            else
                            {
                                Client.ClientCallback.TellMessageToServerConsole(e.Data + "\n");
                            }
                        }
                    };
                    p.OutputDataReceived += (sender, e) =>
                    {
                        if (e.Data != null)
                        {
                            if (Client.ExtraData.TryGetValue("ps_appendNewLine", out string val))
                            {
                                if (val == "false")
                                {
                                    Client.ClientCallback.TellMessageToServerConsole(e.Data);
                                }
                                else
                                {
                                    Client.ClientCallback.TellMessageToServerConsole(new UILogItem(OutputLevel.Error, $"Extra data for appendText is invalid. Value: {val}", "Server Host"));
                                }
                            }
                            else
                            {
                                Client.ClientCallback.TellMessageToServerConsole(e.Data + "\n");
                            }
                        }
                    };
                    ServerManager.Logger.AddOutput("Starting process component.", OutputLevel.Info);
                    p.Start();
                    ServerManager.Logger.AddOutput("Beginning error stream read line.", OutputLevel.Debug);
                    p.BeginErrorReadLine();
                    ServerManager.Logger.AddOutput("Beginning standord stream reade line.", OutputLevel.Debug);
                    p.BeginOutputReadLine();
                }
            }
            // OperationContext.Current.OperationCompleted += (sender, e) => Client.ClientCallback.SendSignal(new SignalMessage(OPERATION_COMPLETED, ""));
        }

        public DialogResult ShowMessageBox(string Message, string Caption, System.Windows.Forms.MessageBoxIcon Icon, System.Windows.Forms.MessageBoxButtons Buttons)
        {
            if (CheckRegisteration("ShowMessageBox"))
            {
                if (LoggedInUser.Role.AttachedPolicyObject.Policies.FindSubFolder("Operations").Policies.First(p => p.ShortName == "EnableConsole").Values["Value"] != "True")
                {
                    Client.ClientCallback.TellMessage("You do not have promission to use the CanShowMessageBox function.", OutputLevel.Info);
                    return DialogResult.Abort;
                }
                else
                {
                    var dr = MessageBox.Show(Message, Caption, Buttons, Icon);
                    Client.ClientCallback.TellMessage(new UILogItem(OutputLevel.Info, $"The user responded to the message box. Response: {dr.ToString()}", "Server Host"));
                    return dr;
                }
            }
            else
            {
                return DialogResult.Abort;
            }
            // OperationContext.Current.OperationCompleted += (sender, e) => Client.ClientCallback.SendSignal(new SignalMessage(OPERATION_COMPLETED, ""));
        }

        public void Speak(string Message, VoiceGender Gender, VoiceAge Age)
        {
            if (CheckRegisteration("Speak"))
            {
                if (LoggedInUser.Role.AttachedPolicyObject.Policies.FindSubFolder("Operations").Policies.First(p => p.ShortName == "EnableConsole").Values["Value"] != "True")
                {
                    Client.ClientCallback.TellMessage("You do not have promission to use the Speak function.", OutputLevel.Info);
                }
                else
                {
                    System.Speech.Synthesis.SpeechSynthesizer ss = new System.Speech.Synthesis.SpeechSynthesizer();
                    ss.SelectVoiceByHints(Gender, Age);
                    ss.Speak(Message);
                    Client.ClientCallback.TellMessage($"Server spoke. Message: {Message}, gender: {Gender.ToString()}, age: {Age.ToString()}", OutputLevel.Info);
                }
            }
            // OperationContext.Current.OperationCompleted += (sender, e) => Client.ClientCallback.SendSignal(new SignalMessage(OPERATION_COMPLETED, ""));
        }

        public CommandPipeline RunServerCommand(string Command, CommandExecutionMode commandMode)
        {
            if (CheckRegisteration("RunServerCommand"))
            {
                CommandPipeline pipe = new CommandPipeline();
                int pos = 0;
                if (LoggedInUser.Role.AttachedPolicyObject.Policies.FindSubFolder("Operations").Policies.First(p => p.ShortName == "EnableConsole").Values["Value"] != "True")
                {
                    Client.ClientCallback.TellMessage("You do not have promission to use the Console function.", OutputLevel.Info);
                    return null;
                }
                else
                {
                    //This is script command.
                    if (Command.StartsWith("::"))
                    {
                        try
                        {
                            ServerManager.ScriptBuilder.ExecuteStringUsingSameScriptScope(Command.TrimStart(':'));
                        }
                        catch (Exception ex)
                        {
                            Client.ClientCallback.TellMessageToServerConsole(new UILogItem(OutputLevel.Error, $"Could not execute script file: {ex.Message}", ScriptBuilder.SCRIPT_LOG_CONSTANT));
                        }
                    }
                    else
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
                                var routine = new CommandRoutine(request, ServerManager.Execute(request, CommandExecutionMode.Client, pipe));
                                pipe.Add(pos++, routine);
                            }
                        }
                        catch (ParserException e)
                        {
                            UILogItem parseMessage = new UILogItem(OutputLevel.Error, $"Unable to parse command: {e.Message}");
                            parseMessage.From = "Server Host";
                            ServerManager.Logger.AddOutput(parseMessage.Message, parseMessage.Level, parseMessage.From);
                            Client.ClientCallback.TellMessageToServerConsole(parseMessage);
                            return pipe;
                        }
                    }
                }
                // OperationContext.Current.OperationCompleted += (sender, e) => Client.ClientCallback.SendSignal(new SignalMessage(OPERATION_COMPLETED, ""));
                return pipe;
            }
            else
            {
                return null;
            }
        }

        private CommandToken[] RunVariableReplacement(CommandParser p, out bool success)
        {
            success = true;
            List<CommandToken> tokenList = new List<CommandToken>();
            var variableTokens = p.GetVariables();
            foreach (CommandToken variableToken in variableTokens)
            {
                var variablename = variableToken.OriginalValue.Remove(0, 1);
                if (ServerManager.DefaultService.Variables.ContainsKey(variablename))
                {
                    var variableValue = ServerManager.DefaultService.Variables[variablename];

                    variableToken.Value = variableValue;
                    success = true;
                    tokenList.Add(variableToken);
                }
                else
                {
                    Client.ClientCallback.TellMessageToServerConsole(new UILogItem(OutputLevel.Error, $"Variable {variablename} does not exist", "Server Host"));
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
                if (ServerManager.DefaultService.Variables.ContainsKey(variablename))
                {
                    var variableValue = ServerManager.DefaultService.Variables[variablename];

                    variableToken.Value = variableValue;
                    success = true;
                    tokenList.Add(variableToken);
                }
                else
                {
                    Client.ClientCallback.TellMessageToServerConsole(new UILogItem(OutputLevel.Error, $"Variable {variablename} does not exist", "Server Host"));
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
                    var routine = new CommandRoutine(request, ServerManager.Execute(request, CommandExecutionMode.Client, pipe));
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
                    var routine = new CommandRoutine(request, ServerManager.Execute(request, CommandExecutionMode.Client, pipe));
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

        public void UpdateServerSettings(ServerSettings Settings)
        {
            if (CheckRegisteration("UpdateServerSettings"))
            {
                if (LoggedInUser.Role.AttachedPolicyObject.Policies.FindSubFolder("Operations").Policies.First(p => p.ShortName == "EnableConsole").Values["Value"] != "True")
                {
                    Client.ClientCallback.TellMessage(new UILogItem(OutputLevel.Error, "You do not have permission to change server settings.", "Server Host"));
                }
                else
                {
                    ServerManager.Logger.AddOutput("Updating server settings.", OutputLevel.Info);
                    ServerManager.DefaultSettings = Settings;
                    Client.ClientCallback.TellMessage("Saving settings.", OutputLevel.Info);
                    ServerManager.DefaultSettings.Save();
                    Client.ClientCallback.TellMessage("Settings saved.", OutputLevel.Info);
                    ServerManager.Logger.AddOutput("Settings saved.", OutputLevel.Info);
                }
            }
            // OperationContext.Current.OperationCompleted += (sender, e) => Client.ClientCallback.SendSignal(new SignalMessage(OPERATION_COMPLETED, ""));
        }

        public ServerSettings GetServerSettings()
        {
            // OperationContext.Current.OperationCompleted += (sender, e) => Client.ClientCallback.SendSignal(new SignalMessage(OPERATION_COMPLETED, ""));
            if (CheckRegisteration("GetServerSettings"))
            {
                if (LoggedInUser.Role.AttachedPolicyObject.Policies.FindSubFolder("Operations").Policies.First(p => p.ShortName == "GetServerSettings").Values["Value"] != "True")
                {
                    Client.ClientCallback.TellMessage(new UILogItem(OutputLevel.Error, "You do not have permission to change server settings.", "Server Host"));
                    return null;
                }
                else
                {
                    ServerManager.Logger.AddOutput("Retreiving server settings.", OutputLevel.Info);
                    return ServerManager.DefaultSettings;
                }
            }
            else
            {
                return null;
            }
        }

        public void Restart()
        {
            Application.Restart();
        }
        public UserAccount GetLoggedInUser()
        {
            // OperationContext.Current.OperationCompleted += (sender, e) => Client.ClientCallback.SendSignal(new SignalMessage(OPERATION_COMPLETED, ""));
            if (CheckRegisteration("GetLoggedInUser"))
            {
                return LoggedInUser;
            }
            else
            {
                return null;
            }
        }

        public IEnumerable<CommandDescription> GetCommands()
        {
            if (CheckRegisteration("GetCommands"))
            {
                // OperationContext.Current.OperationCompleted += (sender, e) => Client.ClientCallback.SendSignal(new SignalMessage(OPERATION_COMPLETED, ""));
                List<CommandDescription> rc = new List<CommandDescription>();
                ServerManager.Logger.AddOutput("Requesting commands list.", OutputLevel.Info);
                Client.ClientCallback.TellMessage("Returning commands list.", OutputLevel.Info);
                foreach (KeyValuePair<string, CommandDelegate> currentCommand in ServerManager.DefaultService.Commands)
                {
                    rc.Add(new CommandDescription() { Help = RemotePlusConsole.GetCommandHelp(currentCommand.Value), Behavior = RemotePlusConsole.GetCommandBehavior(currentCommand.Value), HelpPage = RemotePlusConsole.GetCommandHelpPage(currentCommand.Value), CommandName = currentCommand.Key });
                }
                return rc;
            }
            else
            {
                return null;
            }
        }
        public IEnumerable<string> GetCommandsAsStrings()
        {
            // OperationContext.Current.OperationCompleted += (sender, e) => Client.ClientCallback.SendSignal(new SignalMessage(OPERATION_COMPLETED, ""));
            if (CheckRegisteration("GetCommandsAsStrings"))
            {
                Client.ClientCallback.SendSignal(new SignalMessage("operation_completed", ""));
                return ServerManager.DefaultService.Commands.Keys;
            }
            else
            {
                return null;
            }
        }

        public void SwitchUser()
        {
            // OperationContext.Current.OperationCompleted += (sender, e) => Client.ClientCallback.SendSignal(new SignalMessage(OPERATION_COMPLETED, ""));
            LogOff();
            ServerManager.Logger.AddOutput("Logging in.", OutputLevel.Info ,"Server Host");
            Client.ClientCallback.TellMessage(new UILogItem(OutputLevel.Info, "Logging in.", "Server Host"));
            var cred = Client.ClientCallback.RequestAuthentication(new AuthenticationRequest(AuthenticationSeverity.Normal) { Reason = "Please provide a username and password to switch to." });
            LogIn(cred);
        }
        private void LogOff()
        {
            Registered = false;
            string username = LoggedInUser.Credentials.Username;
            LoggedInUser = null;
            ServerManager.Logger.AddOutput($"User {username} logged off.", OutputLevel.Info, "Server Host");
            Client.ClientCallback.TellMessage(new UILogItem(OutputLevel.Info, $"user {username} logged off.", "Server Host"));
        }
        private UserAccount LogIn(UserCredentials cred)
        {
            // OperationContext.Current.OperationCompleted += (sender, e) => Client.ClientCallback.SendSignal(new SignalMessage(OPERATION_COMPLETED, ""));
            if (cred == null)
            {
                ServerManager.Logger.AddOutput("The user did not pass in any credentials. Authentication failed.", OutputLevel.Info);
                Client.ClientCallback.TellMessage("Can't you at least provide a username and password?", OutputLevel.Info);
                Client.ClientCallback.Disconnect("Authentication failed.");
                return null;
            }
            else
            {
                var l4 = ServerManager.Logger.AddOutput("Authenticating your user credentials.", OutputLevel.Info);
                Client.ClientCallback.TellMessage(new UILogItem(l4.Level, l4.Message, l4.From));
                var tryUser = AccountManager.AttemptLogin(cred);
                if (tryUser != null)
                {
                    return tryUser;
                }
                else
                {
                    Client.ClientCallback.TellMessage("Registiration failed. Authentication failed.", OutputLevel.Info);
                    return null;
                }
            }
        }

        public void Disconnect()
        {
            ServerManager.Logger.AddOutput($"Client \"{Client.FriendlyName ?? "\"\""}\" [{Client.UniqueID}] disconectted.", OutputLevel.Info);
        }

        public void EncryptFile(string fileName, string password)
        {
            // OperationContext.Current.OperationCompleted += (sender, e) => Client.ClientCallback.SendSignal(new SignalMessage(OPERATION_COMPLETED, ""));
            ServerManager.Logger.AddOutput($"Encrypting file. file name: {fileName}", OutputLevel.Info);
            GameclubCryptoServices.CryptoService.EncryptFile(password, fileName, Path.ChangeExtension(fileName, ".ec"));
            ServerManager.Logger.AddOutput("File encrypted.", OutputLevel.Info);
            Client.ClientCallback.TellMessage(new UILogItem(OutputLevel.Info, $"File encrypted. File: {fileName}", "Server Host"));
        }

        public void DecryptFile(string fileName, string password)
        {
            // OperationContext.Current.OperationCompleted += (sender, e) => Client.ClientCallback.SendSignal(new SignalMessage(OPERATION_COMPLETED, ""));
            ServerManager.Logger.AddOutput($"Decrypting file. file name: {fileName}", OutputLevel.Info);
            GameclubCryptoServices.CryptoService.DecrypttFile(password, fileName, Path.ChangeExtension(fileName, ".uc"));
            ServerManager.Logger.AddOutput("File decrypted.", OutputLevel.Info);
            Client.ClientCallback.TellMessage(new UILogItem(OutputLevel.Info, $"File decrypted. File: {fileName}", "Server Host"));
        }

        public string GetCommandHelpPage(string command)
        {
            // OperationContext.Current.OperationCompleted += (sender, e) => Client.ClientCallback.SendSignal(new SignalMessage(OPERATION_COMPLETED, ""));
            return RemotePlusConsole.ShowHelpPage(ServerManager.DefaultService.Commands, command);
        }

        public string GetCommandHelpDescription(string command)
        {
            // OperationContext.Current.OperationCompleted += (sender, e) => Client.ClientCallback.SendSignal(new SignalMessage(OPERATION_COMPLETED, ""));
            return RemotePlusConsole.ShowCommandHelpDescription(ServerManager.DefaultService.Commands, command);
        }
        public IDirectory GetRemoteFiles(string path, bool usingRequest)
        {
            // OperationContext.Current.OperationCompleted += (sender, e) => Client.ClientCallback.SendSignal(new SignalMessage(OPERATION_COMPLETED, ""));
            DirectoryInfo subDir = new DirectoryInfo(path);
            if (subDir.Parent == null)
            {
                DriveInfo driveInfo = new DriveInfo(subDir.FullName);
                if(driveInfo.IsReady)
                {
                    RemoteDrive drive = new RemoteDrive(driveInfo.Name, driveInfo.VolumeLabel);
                    //Get files
                    foreach (FileInfo files in driveInfo.RootDirectory.EnumerateFiles())
                    {
                        drive.Files.Add(new RemoteFile(files.FullName, files.CreationTime, files.LastAccessTime));
                    }
                    //Get Folders
                    foreach (DirectoryInfo folders in driveInfo.RootDirectory.EnumerateDirectories())
                    {
                        drive.Directories.Add(new RemoteDirectory(folders.FullName, folders.LastAccessTime));
                    }
                    return drive;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                try
                {
                    RemoteDirectory r = new RemoteDirectory(subDir.FullName, subDir.LastAccessTime);
                    //Get files
                    foreach (FileInfo files in subDir.EnumerateFiles())
                    {
                        r.Files.Add(new RemoteFile(files.FullName, files.CreationTime, files.LastAccessTime));
                    }
                    //Get Folders
                    foreach (DirectoryInfo folders in subDir.EnumerateDirectories())
                    {
                        r.Directories.Add(new RemoteDirectory(folders.FullName, folders.LastAccessTime));
                    }
                    return r;
                }
                catch (UnauthorizedAccessException ex)
                {
                    throw new FaultException<ServerFault>(new ServerFault(), ex.Message);
                }
            }
        }

        public bool ExecuteScript(string script)
        {
            try
            {
                ServerManager.Logger.AddOutput("Running script file.", OutputLevel.Info, "Server Host");
                bool success = ServerManager.ScriptBuilder.ExecuteString(script);
                return success;
            }
            catch (Exception ex)
            {
                Client.ClientCallback.TellMessageToServerConsole(new UILogItem(OutputLevel.Error, $"Could not execute script file: {ex.Message}", ScriptBuilder.SCRIPT_LOG_CONSTANT));
                return false;
            }
        }

        public ScriptGlobalInformation[] GetScriptGlobals()
        {
            var list = ServerManager.ScriptBuilder.GetGlobals();
            return list.Select(l => l.Information).ToArray();
        }

        public string ReadFileAsString(string fileName)
        {
            return File.ReadAllText(fileName);
        }

        public List<string> GetServerRoleNames()
        {
            List<string> l = new List<string>();
            foreach(Role r in Role.GlobalPool.Roles)
            {
                l.Add(r.RoleName);
            }
            return l;
        }

        public void ServerRegistered(Guid serverGuid)
        {
            ServerManager.ServerGuid = serverGuid;
        }
    }
}