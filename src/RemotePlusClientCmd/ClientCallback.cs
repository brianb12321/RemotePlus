﻿using System;
using RemotePlusLibrary;
using System.ServiceModel;
using RemotePlusClient.CommonUI;
using System.Windows.Forms;
using System.Speech.Synthesis;
using System.Media;
using System.Diagnostics;
using RemotePlusLibrary.Security.AccountSystem;
using System.Threading;
using RemotePlusLibrary.Contracts;
using RemotePlusLibrary.RequestSystem;
using RemotePlusLibrary.Security.Authentication;
using RemotePlusLibrary.Core;
using BetterLogger;
using RemotePlusLibrary.Extension.ResourceSystem;
using TinyMessenger;
using RemotePlusLibrary.SubSystem.Command;
using System.Drawing;

namespace RemotePlusClientCmd
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Multiple,
        IncludeExceptionDetailInFaults = true,
        UseSynchronizationContext = false)]
    [ServiceKnownType("GetKnownTypes", typeof(DefaultKnownTypeManager))]
    class ClientCallback : IRemoteClient
    {
        public void Beep(int Hertz, int Duration)
        {
            Console.Beep(Hertz, Duration);
        }

        public void ChangePrompt(Guid guid, RemotePlusLibrary.SubSystem.Command.PromptBuilder newPrompt)
        {
            ClientCmdManager.prompt = newPrompt;
        }

        public void Disconnect(Guid guid, string Reason)
        {
            //ClientCmdManager.WaitFlag = true;
            ClientCmdManager.Remote.Close();
            GlobalServices.Logger.Log($"Server [{guid}] disconnected. " + Reason, LogLevel.Info, "CLient");
            //ClientCmdManager.WaitFlag = false;
        }

        public RemotePlusLibrary.SubSystem.Command.PromptBuilder GetCurrentPrompt()
        {
            return ClientCmdManager.prompt;
        }

        public Resource GetResource(string resourceIdentifier)
        {
            throw new NotImplementedException();
        }

        public void PlaySound(string FileName)
        {
            SoundPlayer player = new SoundPlayer(FileName);
            player.Play();
        }

        public void PlaySoundLoop(string FileName)
        {
            SoundPlayer player = new SoundPlayer(FileName);
            player.PlaySync();
        }

        public void PlaySoundSync(string FileName)
        {
            SoundPlayer player = new SoundPlayer(FileName);
            player.PlayLooping();
        }

        public ClientBuilder RegisterClient()
        {
            ClientBuilder cb = new ClientBuilder(ClientType.CommandLine);
            cb.FriendlyName = "RemotePlus Client Command Line";
            cb.ExtraData.Add("ps_appendNewLine", "false");
            return cb;
        }

        public void RegistirationComplete(Guid guid)
        {
            ClientCmdManager.WaitFlag = false;
        }

        public UserCredentials RequestAuthentication(Guid guid, AuthenticationRequest Request)
        {
            //ClientCmdManager.WaitFlag = true;
            Console.WriteLine($"The server [{guid}] requires authentication. Reason: {Request.Reason}");
            Console.Write("Enter Username: ");
            string username = Console.ReadLine();
            Console.Write("Enter Password: ");
            string password = Console.ReadLine();
            //ClientCmdManager.WaitFlag = false;
            return new UserCredentials(username, password);
        }

        public ReturnData RequestInformation(Guid guid, RequestBuilder builder)
        {
            ReturnData data = null;
            Thread t = new Thread((p) => data = RequestStore.Show(guid, (RequestBuilder)p));
            t.SetApartmentState(ApartmentState.STA);
            t.Start(builder);
            t.Join();
            return data;
        }
        public void UpdateRequest(Guid serverGuid, UpdateRequestBuilder message)
        {
            RequestStore.Update(serverGuid, message);
        }
        public void DisposeCurrentRequest(Guid serverGuid)
        {
            RequestStore.DisposeCurrentRequest();
        }
        public void RunProgram(string Program, string Argument, bool shell, bool ignore)
        {
            var p = Process.Start(Program, Argument);
            if(!ignore)
            {
                p.WaitForExit();
            }
        }

        public void SendSignal(SignalMessage message)
        {
            //ClientCmdManager.WaitFlag = true;
        }

        public DialogResult ShowMessageBox(string Message, string Caption, MessageBoxIcon Icon, MessageBoxButtons Buttons)
        {
            return MessageBox.Show(Message, Caption, Buttons, Icon);
        }

        public void Speak(string Message, VoiceGender Gender, VoiceAge Age)
        {
            SpeechSynthesizer ss = new SpeechSynthesizer();
            ss.SelectVoiceByHints(Gender, Age);
            ss.Speak(Message);
        }

        public void TellMessage(Guid guid, string Message, LogLevel o)
        {
            //ClientCmdManager.WaitFlag = true;
            GlobalServices.Logger.Log(Message, o, "Server Host", guid.ToString());
            //ClientCmdManager.WaitFlag = false;
        }

        public void WriteToClientConsole(Guid guid, string Message)
        {
            //ClientCmdManager.WaitFlag = true;
            Console.WriteLine(Message);
            //ClientCmdManager.WaitFlag = false;
        }

        public void WriteToClientConsole(Guid guid, ConsoleText text)
        {
            if(text.TextColor != Color.Empty) Colorful.Console.ForegroundColor = text.TextColor;
            if(text.BackColor != Color.Empty) Colorful.Console.BackgroundColor = text.BackColor;
            Colorful.Console.WriteLine(text.Text);
            Colorful.Console.ResetColor();
        }

        public void WriteToClientConsole(Guid serverGuid, string Message, LogLevel level)
        {
            GlobalServices.Logger.Log(Message, level, "Server Console", serverGuid.ToString());
        }

        public void WriteToClientConsole(Guid serverGuid, string Message, LogLevel level, string from)
        {
            GlobalServices.Logger.Log(Message, level, $"Server Console {from}", serverGuid.ToString());
        }

        public void WriteToClientConsoleNoNewLine(Guid serverGuid, string Message)
        {
            Console.Write(Message);
        }

        public void PublishEvent(ITinyMessage message)
        {
            GlobalServices.EventBus.PublishPrivate(message);
        }

        public bool HasKnownType(string name)
        {
            return DefaultKnownTypeManager.HasName(name);
        }

        public void ClearClientConsole(Guid guid)
        {
            Console.Clear();
        }

        public void SetClientConsoleBackgroundColor(Guid guid, Color bgColor)
        {
            Colorful.Console.BackgroundColor = bgColor;
        }

        public void SetClientConsoleForegroundColor(Guid guid, Color fgColor)
        {
            Colorful.Console.ForegroundColor = fgColor;
        }

        public void ResetClientConsoleColor(Guid guid)
        {
            Colorful.Console.ResetColor();
        }
    }
}