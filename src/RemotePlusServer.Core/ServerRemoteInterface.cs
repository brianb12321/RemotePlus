﻿using BetterLogger;
using RemotePlusLibrary;
using RemotePlusLibrary.Client;
using RemotePlusLibrary.Core.IOC;
using RemotePlusLibrary.Extension.CommandSystem;
using RemotePlusLibrary.Extension.CommandSystem.CommandClasses;
using RemotePlusLibrary.Extension.CommandSystem.CommandClasses.Parsing;
using RemotePlusLibrary.Scripting;
using RemotePlusLibrary.Security.AccountSystem;
using RemotePlusLibrary.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Speech.Synthesis;
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
        public void RunProgram(string Program, string Argument, bool shellExecute, bool ignore)
        {
            GlobalServices.Logger.Log("Creating process component.", LogLevel.Debug);
            Process p = new Process();
            GlobalServices.Logger.Log($"File to execute: {Program}", LogLevel.Debug);
            p.StartInfo.FileName = Program;
            GlobalServices.Logger.Log($"File arguments: {Argument}", LogLevel.Debug);
            p.StartInfo.Arguments = Argument;
            GlobalServices.Logger.Log($"Shell execution is disabled.", LogLevel.Debug);
            p.StartInfo.UseShellExecute = shellExecute;
            if(!shellExecute)
            {
                GlobalServices.Logger.Log($"Error stream will be redirected.", LogLevel.Debug);
                p.StartInfo.RedirectStandardError = true;
                GlobalServices.Logger.Log($"Standord output stream will be redirected.", LogLevel.Debug);
                p.StartInfo.RedirectStandardOutput = true;
            }
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
            if(!shellExecute)
            {
                GlobalServices.Logger.Log("Beginning error stream read line.", LogLevel.Debug);
                p.BeginErrorReadLine();
                GlobalServices.Logger.Log("Beginning standord output stream reade line.", LogLevel.Debug);
                p.BeginOutputReadLine();
            }
            if (!ignore)
            {
                p.WaitForExit();
            }
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
        public CommandPipeline RunServerCommand(string command, CommandExecutionMode commandMode)
        {
            CommandPipeline pipe = new CommandPipeline();
            int pos = 0;
            //Makes sure that a script command won't be executed by a script, thus avoiding lots of trouble.
            if (command.StartsWith("::") && commandMode == CommandExecutionMode.Client)
            {
                //This is script command.
                try
                {
                    ServerManager.ScriptBuilder.ExecuteStringUsingSameScriptScope(command.TrimStart(':'));
                }
                catch (Exception ex)
                {
                    Client.ClientCallback.TellMessageToServerConsole($"Could not execute script file: {ex.Message}", LogLevel.Error, ScriptBuilder.SCRIPT_LOG_CONSTANT);
                }
            }
            else
            {
                try
                {
                    ICommandEnvironmnet env = IOCContainer.GetService<ICommandEnvironmnet>();
                    ILexer lexer = env.Lexer;
                    IParser parser = env.Parser;
                    //ITokenProcessor processor = env.Processor;
                    RemotePlusLibrary.Extension.CommandSystem.CommandClasses.Parsing.ICommandExecutor executor = env.Executor;
                    //processor.ConfigureProcessor(ServerManager.ServerRemoteService.Variables, executor);
                    var tokens = lexer.Lex(command);
                    var elements = parser.Parse(tokens);
                    //var newTokens = processor.RunSubRoutines(lexer, pipe, pos);
                    //Run the commands
                    var request = new CommandRequest(elements.ToArray());
                    var routine = new CommandRoutine(request, executor.Execute(request, CommandExecutionMode.Client, pipe));
                    pipe.Add(pos++, routine);
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
    }
}