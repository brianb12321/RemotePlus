using BetterLogger;
using RemotePlusLibrary;
using RemotePlusLibrary.Core.IOC;
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
using RemotePlusServer.Internal;
using System.Threading.Tasks;
using RemotePlusLibrary.Extension.ResourceSystem;
using RemotePlusServer.Core.ExtensionSystem;
using RemotePlusLibrary.SubSystem.Command.CommandClasses;
using RemotePlusLibrary.SubSystem.Command;
using System.Threading;

namespace RemotePlusServer.Core
{
    public class ServerRemoteInterface
    {
        
        public ServerRemoteInterface()
        {
            
        }
        public Guid GetSelectedServerGuid()
        {
            return ServerManager.ServerGuid;
        }
        public void Beep(int Hertz, int Duration)
        {
            Console.Beep(Hertz, Duration);
        }
        public void RunProgram(IClientContext context, string Program, string Argument, bool shellExecute, bool ignore)
        {
            var Client = context.GetClient<RemoteClient>();
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
        public void EncryptFile(IClientContext context, string fileName, string password)
        {
            GlobalServices.Logger.Log($"Encrypting file. file name: {fileName}", LogLevel.Info);
            GameclubCryptoServices.CryptoService.EncryptFile(password, fileName, Path.ChangeExtension(fileName, ".ec"));
            GlobalServices.Logger.Log("File encrypted.", LogLevel.Info);
            context.GetClient<RemoteClient>().ClientCallback.TellMessage($"File encrypted. File: {fileName}", LogLevel.Info);
        }
        public void Speak(IClientContext context, string message, VoiceGender gender, VoiceAge age)
        {
            SpeechSynthesizer ss = new System.Speech.Synthesis.SpeechSynthesizer();
            ss.SelectVoiceByHints(gender, age);
            ss.Speak(message);
            context.GetClient<RemoteClient>().ClientCallback.TellMessage($"Server spoke. Message: {message}, gender: {gender.ToString()}, age: {age.ToString()}", LogLevel.Info);
        }
        public DialogResult ShowMessageBox(IClientContext context, string message, string caption, System.Windows.Forms.MessageBoxIcon Icon, System.Windows.Forms.MessageBoxButtons Buttons)
        {
            var dr = MessageBox.Show(message, caption, Buttons, Icon);
            context.GetClient<RemoteClient>().ClientCallback.TellMessage($"The user responded to the message box. Response: {dr.ToString()}", LogLevel.Info);
            return dr;
        }
        public void DecryptFile(IClientContext context, string fileName, string password)
        {
            GlobalServices.Logger.Log($"Decrypting file. file name: {fileName}", LogLevel.Info);
            GameclubCryptoServices.CryptoService.DecrypttFile(password, fileName, Path.ChangeExtension(fileName, ".uc"));
            GlobalServices.Logger.Log("File decrypted.", LogLevel.Info);
            context.GetClient<RemoteClient>().ClientCallback.TellMessage($"File decrypted. File: {fileName}", LogLevel.Info);
        }
        public CommandPipeline RunServerCommand(IClientContext context, string command, CommandExecutionMode commandMode)
        {
            return IOCContainer.GetService<ICommandSubsystem<IServerCommandModule>>().RunServerCommand(command, commandMode, context);
        }
        public Task<CommandPipeline> RunServerCommandAsync(IClientContext context, string command, CommandExecutionMode commandMode)
        {
            return IOCContainer.GetService<ICommandSubsystem<IServerCommandModule>>().RunServerCommandAsync(command, commandMode, context);
        }
        public void CancelServerCommand()
        {
            IOCContainer.GetService<ICommandSubsystem<IServerCommandModule>>().Cancel();
        }
    }
}