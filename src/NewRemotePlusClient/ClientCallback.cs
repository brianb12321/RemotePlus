using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RemotePlusLibrary.Contracts;
using System.Windows.Forms;
using BetterLogger;
using RemotePlusLibrary;
using RemotePlusLibrary.Client;
using RemotePlusLibrary.Extension.CommandSystem;
using RemotePlusLibrary.RequestSystem;
using RemotePlusLibrary.Security.AccountSystem;
using RemotePlusLibrary.Security.Authentication;
using System.Speech.Synthesis;
using System.ServiceModel;
using System.Threading;
using GalaSoft.MvvmLight.Messaging;
using NewRemotePlusClient.Models;

namespace NewRemotePlusClient
{
    [CallbackBehavior(IncludeExceptionDetailInFaults = true,
        ConcurrencyMode = ConcurrencyMode.Multiple,
        UseSynchronizationContext = false)]
    public class ClientCallback : IRemoteClient
    {
        public void Beep(int Hertz, int Duration)
        {
            Console.Beep(Hertz, Duration);
        }

        public void ChangePrompt(Guid serverGuid, RemotePlusLibrary.Extension.CommandSystem.PromptBuilder newPrompt)
        {
            throw new NotImplementedException();
        }

        public void Disconnect(Guid serverGuid, string Reason)
        {
            IOCHelper.Client.Disconnect();
            IOCHelper.Client.Close();
        }

        public RemotePlusLibrary.Extension.CommandSystem.PromptBuilder GetCurrentPrompt()
        {
            throw new NotImplementedException();
        }

        public void PlaySound(string FileName)
        {
            throw new NotImplementedException();
        }

        public void PlaySoundLoop(string FileName)
        {
            throw new NotImplementedException();
        }

        public void PlaySoundSync(string FileName)
        {
            throw new NotImplementedException();
        }

        public ClientBuilder RegisterClient()
        {
            ClientBuilder cb = new ClientBuilder(ClientType.GUI);
            cb.FriendlyName = "New RemotePlus Client";
            return cb;
        }

        public void RegistirationComplete(Guid serverGuid)
        {
        }

        public UserCredentials RequestAuthentication(Guid serverGuid, AuthenticationRequest Request)
        {
            UserCredentials finalCred = new UserCredentials();
            Thread t = new Thread(new ThreadStart(() => finalCred = IOCHelper.LoginManager.ShowLogin()));
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
            t.Join();
            return finalCred;
            //return IOCHelper.LoginManager.ShowLogin();
        }

        public ReturnData RequestInformation(Guid serverGuid, RequestBuilder builder)
        {
            throw new NotImplementedException();
        }

        public void RunProgram(string Program, string Argument)
        {
            throw new NotImplementedException();
        }

        public void SendSignal(Guid serverGuid, SignalMessage signal)
        {
            throw new NotImplementedException();
        }

        public DialogResult ShowMessageBox(string Message, string Caption, MessageBoxIcon Icon, MessageBoxButtons Buttons)
        {
            throw new NotImplementedException();
        }

        public void Speak(string Message, VoiceGender Gender, VoiceAge Age)
        {
            SpeechSynthesizer ss = new SpeechSynthesizer();
            ss.SelectVoiceByHints(Gender, Age);
            ss.Speak(Message);
        }

        public async void TellMessage(Guid serverGuid, string Message, LogLevel o)
        {
            await System.Windows.Application.Current.Dispatcher.InvokeAsync(() => IOCHelper.MainWindow.MainServerLogger.Log(Message, o, "Server Host", $"{serverGuid}"));
        }

        public void TellMessageToServerConsole(Guid serverGuid, string Message)
        {
            Messenger.Default.Send(new LogMessage()
            {
                Message = Message
            });
        }

        public void TellMessageToServerConsole(Guid serverGuid, string Message, LogLevel level)
        {
            Messenger.Default.Send(new LogMessage()
            {
                Message = Message,
                Level = level
            });
        }

        public void TellMessageToServerConsole(Guid serverGuid, ConsoleText text)
        {
            Messenger.Default.Send(text);
        }

        public void TellMessageToServerConsole(Guid serverGuid, string Message, LogLevel level, string from)
        {
            Messenger.Default.Send(new ConsoleLogMessage()
            {
                Message = Message,
                Level = level,
                From = from,
                Extra = serverGuid.ToString()
            });
        }
    }
}
