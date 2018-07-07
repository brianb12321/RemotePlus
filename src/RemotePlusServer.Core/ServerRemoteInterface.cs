using BetterLogger;
using RemotePlusLibrary;
using RemotePlusLibrary.Client;
using RemotePlusLibrary.Extension;
using RemotePlusLibrary.Extension.CommandSystem;
using RemotePlusLibrary.Extension.CommandSystem.CommandClasses;
using RemotePlusLibrary.Extension.CommandSystem.CommandClasses.Parsing;
using RemotePlusLibrary.IOC;
using RemotePlusLibrary.Scripting;
using RemotePlusLibrary.Security.AccountSystem;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Speech.Synthesis;
using System.Text.RegularExpressions;
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
            GlobalServices.Logger.Log("Creating process component.", LogLevel.Debug);
            Process p = new Process();
            GlobalServices.Logger.Log($"File to execute: {Program}", LogLevel.Debug);
            p.StartInfo.FileName = Program;
            GlobalServices.Logger.Log($"File arguments: {Argument}", LogLevel.Debug);
            p.StartInfo.Arguments = Argument;
            GlobalServices.Logger.Log($"Shell execution is disabled.", LogLevel.Debug);
            p.StartInfo.UseShellExecute = false;
            GlobalServices.Logger.Log($"Error stream will be redirected.", LogLevel.Debug);
            p.StartInfo.RedirectStandardError = true;
            GlobalServices.Logger.Log($"Standord output stream will be redirected.", LogLevel.Debug);
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
                            Client.ClientCallback.TellMessageToServerConsole($"Extra data for appendText is invalid. Value: {val}", BetterLogger.LogLevel.Error, "Server Host");
                        }
                    }
                    else if (val != "true" && val != "false")
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
                        else if(val != "true" && val != "false")
                        {
                            Client.ClientCallback.TellMessageToServerConsole($"Extra data for appendText is invalid. Value: {val}", BetterLogger.LogLevel.Error, "Server Host");
                        }
                    }
                    else
                    {
                        Client.ClientCallback.TellMessageToServerConsole(e.Data + "\n");
                    }
                }
            };
            GlobalServices.Logger.Log("Starting process component.", LogLevel.Info);
            p.Start();
            GlobalServices.Logger.Log("Beginning error stream read line.", LogLevel.Debug);
            p.BeginErrorReadLine();
            GlobalServices.Logger.Log("Beginning standord output stream reade line.", LogLevel.Debug);
            p.BeginOutputReadLine();
            p.WaitForExit();
        }
        public void EncryptFile(string fileName, string password)
        {
            GlobalServices.Logger.Log($"Encrypting file. file name: {fileName}", LogLevel.Info);
            GameclubCryptoServices.CryptoService.EncryptFile(password, fileName, Path.ChangeExtension(fileName, ".ec"));
            GlobalServices.Logger.Log("File encrypted.", LogLevel.Info);
            Client.ClientCallback.TellMessage($"File encrypted. File: {fileName}", LogLevel.Info);
        }
        public void Speak(string Message, VoiceGender Gender, VoiceAge Age)
        {
            SpeechSynthesizer ss = new System.Speech.Synthesis.SpeechSynthesizer();
            ss.SelectVoiceByHints(Gender, Age);
            ss.Speak(Message);
            Client.ClientCallback.TellMessage($"Server spoke. Message: {Message}, gender: {Gender.ToString()}, age: {Age.ToString()}", LogLevel.Info);
        }
        public DialogResult ShowMessageBox(string Message, string Caption, System.Windows.Forms.MessageBoxIcon Icon, System.Windows.Forms.MessageBoxButtons Buttons)
        {
            var dr = MessageBox.Show(Message, Caption, Buttons, Icon);
            Client.ClientCallback.TellMessage($"The user responded to the message box. Response: {dr.ToString()}", LogLevel.Info);
            return dr;
        }
        public void DecryptFile(string fileName, string password)
        {
            GlobalServices.Logger.Log($"Decrypting file. file name: {fileName}", LogLevel.Info);
            GameclubCryptoServices.CryptoService.DecrypttFile(password, fileName, Path.ChangeExtension(fileName, ".uc"));
            GlobalServices.Logger.Log("File decrypted.", LogLevel.Info);
            Client.ClientCallback.TellMessage($"File decrypted. File: {fileName}", LogLevel.Info);
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
                    Client.ClientCallback.TellMessageToServerConsole($"Could not execute script file: {ex.Message}", LogLevel.Error, ScriptBuilder.SCRIPT_LOG_CONSTANT);
                }
            }
            else
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
                        var routine = new CommandRoutine(request, Execute(request, CommandExecutionMode.Client, pipe));
                        pipe.Add(pos++, routine);
                    }
                }
                catch (ParserException e)
                {
                    string parseErrorMessage = $"Unable to parse command: {e.Message}";
                    GlobalServices.Logger.Log(parseErrorMessage, LogLevel.Error, "Server Host");
                    Client.ClientCallback.TellMessageToServerConsole(new ConsoleText(parseErrorMessage) { TextColor = Color.Red });
                    return pipe;
                }
            }
            return pipe;
        }
        protected virtual CommandToken[] RunVariableReplacement(IParser p, out bool success)
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
                    Client.ClientCallback.TellMessageToServerConsole($"Variable {variablename} does not exist", LogLevel.Error, "Server Host");
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
                    Client.ClientCallback.TellMessageToServerConsole($"Variable {variablename} does not exist", LogLevel.Error, "Server Host");
                    success = false;
                }
            }
            return tokenList.ToArray();
        }
        protected virtual IEnumerable<CommandToken> RunSubRoutines(IParser p, CommandPipeline pipe, int position)
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
        protected virtual IEnumerable<CommandToken> RunSubRoutines(CommandToken[] tokens, IParser p, CommandPipeline pipe, int position)
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
        protected virtual IEnumerable<CommandToken> ParseOutQoutes(IParser p)
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
                GlobalServices.Logger.Log($"Executing server command {c.Arguments[0]}", LogLevel.Info);
                try
                {
                    var command = ServerManager.ServerRemoteService.Commands[c.Arguments[0].Value];
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
                    var sc = command(c, pipe);
                    if (scdm == StatusCodeDeliveryMethod.TellMessage)
                    {
                        ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.TellMessage($"Command {c.Arguments[0]} finished with status code {sc.ToString()}", LogLevel.Info);
                    }
                    else if (scdm == StatusCodeDeliveryMethod.TellMessageToServerConsole)
                    {
                        ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.TellMessageToServerConsole($"Command {c.Arguments[0]} finished with status code {sc.ToString()}", LogLevel.Info);
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
