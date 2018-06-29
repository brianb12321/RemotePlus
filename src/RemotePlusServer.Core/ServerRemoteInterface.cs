using Logging;
using RemotePlusLibrary;
using RemotePlusLibrary.Client;
using RemotePlusLibrary.Contracts;
using RemotePlusLibrary.Extension;
using RemotePlusLibrary.Extension.CommandSystem;
using RemotePlusLibrary.Extension.CommandSystem.CommandClasses;
using RemotePlusLibrary.Extension.CommandSystem.CommandClasses.Parsing;
using RemotePlusLibrary.Scripting;
using RemotePlusLibrary.Security.AccountSystem;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Speech.Synthesis;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RemotePlusServer.Core
{
    public class ServerRemoteInterface
    {
        public RegisterationObject Settings { get; set; }
        public Client<RemoteClient> Client { get; set; }
        public bool Registered { get; set; }
        public UserAccount LoggedInUser { get; set; }
        public string CurrentPath = Environment.CurrentDirectory;
        public ServerRemoteInterface()
        {
            
        }
        public void Beep(int Hertz, int Duration)
        {
            Console.Beep(Hertz, Duration);
        }
        public void RunProgram(string Program, string Argument)
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
        public void EncryptFile(string fileName, string password)
        {
            ServerManager.Logger.AddOutput($"Encrypting file. file name: {fileName}", OutputLevel.Info);
            GameclubCryptoServices.CryptoService.EncryptFile(password, fileName, Path.ChangeExtension(fileName, ".ec"));
            ServerManager.Logger.AddOutput("File encrypted.", OutputLevel.Info);
            Client.ClientCallback.TellMessage(new UILogItem(OutputLevel.Info, $"File encrypted. File: {fileName}", "Server Host"));
        }
        public void Speak(string Message, VoiceGender Gender, VoiceAge Age)
        {
            SpeechSynthesizer ss = new System.Speech.Synthesis.SpeechSynthesizer();
            ss.SelectVoiceByHints(Gender, Age);
            ss.Speak(Message);
            Client.ClientCallback.TellMessage($"Server spoke. Message: {Message}, gender: {Gender.ToString()}, age: {Age.ToString()}", OutputLevel.Info);
        }
        public DialogResult ShowMessageBox(string Message, string Caption, System.Windows.Forms.MessageBoxIcon Icon, System.Windows.Forms.MessageBoxButtons Buttons)
        {
            var dr = MessageBox.Show(Message, Caption, Buttons, Icon);
            Client.ClientCallback.TellMessage(new UILogItem(OutputLevel.Info, $"The user responded to the message box. Response: {dr.ToString()}", "Server Host"));
            return dr;
        }
        public void DecryptFile(string fileName, string password)
        {
            ServerManager.Logger.AddOutput($"Decrypting file. file name: {fileName}", OutputLevel.Info);
            GameclubCryptoServices.CryptoService.DecrypttFile(password, fileName, Path.ChangeExtension(fileName, ".uc"));
            ServerManager.Logger.AddOutput("File decrypted.", OutputLevel.Info);
            Client.ClientCallback.TellMessage(new UILogItem(OutputLevel.Info, $"File decrypted. File: {fileName}", "Server Host"));
        }
        public CommandPipeline RunServerCommand(string Command, CommandExecutionMode commandMode)
        {
            CommandPipeline pipe = new CommandPipeline();
            int pos = 0;
            if (Command.StartsWith("::"))
            {
                //This is script command.
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
                        var routine = new CommandRoutine(request, Execute(request, CommandExecutionMode.Client, pipe));
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
            return pipe;
        }
        protected virtual CommandToken[] RunVariableReplacement(CommandParser p, out bool success)
        {
            success = true;
            List<CommandToken> tokenList = new List<CommandToken>();
            var variableTokens = p.GetVariables();
            foreach (CommandToken variableToken in variableTokens)
            {
                var variablename = variableToken.OriginalValue.Remove(0, 1);
                if (ServerManager.ServerRemoteService.Variables.ContainsKey(variablename))
                {
                    var variableValue = ServerManager.ServerRemoteService.Variables[variablename];

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
        protected virtual CommandToken[] RunVariableReplacement(CommandToken[] tokens, out bool success)
        {
            success = true;
            List<CommandToken> tokenList = new List<CommandToken>();
            foreach (CommandToken variableToken in tokens)
            {
                var variablename = variableToken.OriginalValue.Remove(0, 1);
                if (ServerManager.ServerRemoteService.Variables.ContainsKey(variablename))
                {
                    var variableValue = ServerManager.ServerRemoteService.Variables[variablename];

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
        protected virtual IEnumerable<CommandToken> RunSubRoutines(CommandParser p, CommandPipeline pipe, int position)
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
                    var routine = new CommandRoutine(request, Execute(request, CommandExecutionMode.Client, pipe));
                    routineToken.Value = routine.Output.CustomStatusMessage;
                    position++;
                }
                yield return routineToken;
            }
        }
        protected virtual IEnumerable<CommandToken> RunSubRoutines(CommandToken[] tokens, CommandParser p, CommandPipeline pipe, int position)
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
                    var routine = new CommandRoutine(request, Execute(request, CommandExecutionMode.Client, pipe));
                    routineToken.Value = routine.Output.CustomStatusMessage;
                    position++;
                }
                yield return routineToken;
            }
        }
        protected virtual IEnumerable<CommandToken> ParseOutQoutes(CommandToken[] tokens)
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
        protected virtual IEnumerable<CommandToken> ParseOutQoutes(CommandParser p)
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
        public static CommandResponse Execute(CommandRequest c, CommandExecutionMode commandMode, CommandPipeline pipe)
        {
            bool throwFlag = false;
            StatusCodeDeliveryMethod scdm = StatusCodeDeliveryMethod.DoNotDeliver;
            try
            {
                ServerManager.Logger.AddOutput($"Executing server command {c.Arguments[0]}", OutputLevel.Info);
                try
                {
                    var command = ServerManager.ServerRemoteService.Commands[c.Arguments[0].Value];
                    var ba = RemotePlusConsole.GetCommandBehavior(command);
                    if (ba != null)
                    {
                        if (ba.TopChainCommand && pipe.Count > 0)
                        {
                            ServerManager.Logger.AddOutput($"This is a top-level command.", OutputLevel.Error);
                            ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.TellMessage($"This is a top-level command.", OutputLevel.Error);
                            return new CommandResponse((int)CommandStatus.AccessDenied);
                        }
                        if (commandMode != ba.ExecutionType)
                        {
                            ServerManager.Logger.AddOutput($"The command requires you to be in {ba.ExecutionType} mode.", OutputLevel.Error);
                            ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.TellMessage($"The command requires you to be in {ba.ExecutionType} mode.", OutputLevel.Error);
                            return new CommandResponse((int)CommandStatus.AccessDenied);
                        }
                        if (ba.SupportClients != ClientSupportedTypes.Both && ((ServerManager.ServerRemoteService.RemoteInterface.Client.ClientType == ClientType.GUI && ba.SupportClients != ClientSupportedTypes.GUI) || (ServerManager.ServerRemoteService.RemoteInterface.Client.ClientType == ClientType.CommandLine && ba.SupportClients != ClientSupportedTypes.CommandLine)))
                        {
                            if (string.IsNullOrEmpty(ba.ClientRejectionMessage))
                            {
                                ServerManager.Logger.AddOutput($"Your client must be a {ba.SupportClients.ToString()} client.", OutputLevel.Error);
                                ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.TellMessage($"Your client must be a {ba.SupportClients.ToString()} client.", OutputLevel.Error);
                                return new CommandResponse((int)CommandStatus.UnsupportedClient);
                            }
                            else
                            {
                                ServerManager.Logger.AddOutput(ba.ClientRejectionMessage, OutputLevel.Error);
                                ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.TellMessage(ba.ClientRejectionMessage, OutputLevel.Error);
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
                    ServerManager.Logger.AddOutput("Found command, and executing.", OutputLevel.Debug);
                    var sc = command(c, pipe);
                    if (scdm == StatusCodeDeliveryMethod.TellMessage)
                    {
                        ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.TellMessage($"Command {c.Arguments[0]} finished with status code {sc.ToString()}", OutputLevel.Info);
                    }
                    else if (scdm == StatusCodeDeliveryMethod.TellMessageToServerConsole)
                    {
                        ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.TellMessageToServerConsole(new UILogItem(OutputLevel.Info, $"Command {c.Arguments[0]} finished with status code {sc.ToString()}"));
                    }
                    return sc;
                }
                catch (KeyNotFoundException)
                {
                    ServerManager.Logger.AddOutput("Failed to find the command.", OutputLevel.Debug);
                    ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.TellMessageToServerConsole(new UILogItem(OutputLevel.Error, "Unknown command. Please type {help} for a list of commands", "Server Host"));
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
                    ServerManager.Logger.AddOutput("command failed: " + ex.Message, OutputLevel.Info);
                    ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.TellMessageToServerConsole(new UILogItem(OutputLevel.Error, "Error whie executing command: " + ex.Message, "Server Host"));
                    return new CommandResponse((int)CommandStatus.Fail);
                }
            }
        }
    }
}
