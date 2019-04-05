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
using RemotePlusLibrary.RequestSystem;
using RemotePlusLibrary.Security.AccountSystem;
using RemotePlusLibrary.Security.Authentication;
using System.Speech.Synthesis;
using System.ServiceModel;
using System.Threading;
using GalaSoft.MvvmLight.Messaging;
using NewRemotePlusClient.Models;
using RemotePlusLibrary.Extension.ResourceSystem;
using TinyMessenger;
using RemotePlusLibrary.Core;
using RemotePlusLibrary.SubSystem.Command;

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

        public void ChangePrompt(Guid serverGuid, RemotePlusLibrary.SubSystem.Command.PromptBuilder newPrompt)
        {
            
        }

        public void Disconnect(Guid serverGuid, string Reason)
        {
            IOCHelper.Client.Disconnect();
            IOCHelper.Client.Close();
        }

        public RemotePlusLibrary.SubSystem.Command.PromptBuilder GetCurrentPrompt()
        {
            return null;
        }

        public Resource GetResource(string resourceIdentifier)
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
            ReturnData data = null;
            Thread t = new Thread(() => data = RequestStore.Show(serverGuid, builder));
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
            t.Join();
            return data;
        }
        public void DisposeCurrentRequest(Guid serverGuid)
        {
            RequestStore.DisposeCurrentRequest();
        }

        public void RunProgram(string Program, string Argument, bool shell, bool ignore)
        {
            throw new NotImplementedException();
        }

        public void SendSignal(SignalMessage signal)
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

        public void WriteToClientConsole(Guid serverGuid, string Message)
        {
            Messenger.Default.Send(new ConsoleText(Message));
        }

        public void WriteToClientConsole(Guid serverGuid, string Message, LogLevel level)
        {
            Messenger.Default.Send(new ConsoleLogMessage()
            {
                Message = Message,
                Level = level,
                From = "Server Console",
                Extra = serverGuid.ToString()
            });
        }

        public void WriteToClientConsole(Guid serverGuid, ConsoleText text)
        {
            Messenger.Default.Send(text);
        }

        public void WriteToClientConsole(Guid serverGuid, string Message, LogLevel level, string from)
        {
            Messenger.Default.Send(new ConsoleLogMessage()
            {
                Message = Message,
                Level = level,
                From = from,
                Extra = serverGuid.ToString()
            });
        }

        public void TellMessageToServerConsoleNoNewLine(Guid serverGuid, string Message)
        {
            
        }

        public void UpdateRequest(Guid serverGuid, UpdateRequestBuilder message)
        {
            throw new NotImplementedException();
        }

        public void PublishEvent(ITinyMessage message)
        {
            GlobalServices.EventBus.PublishPrivate(message);
        }

        public bool HasKnownType(string name)
        {
            return DefaultKnownTypeManager.HasName(name);
        }
    }
}