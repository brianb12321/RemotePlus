using BetterLogger;
using RemotePlusClient.CommonUI.Connection;
using RemotePlusClient.ViewModels;
using RemotePlusLibrary;
using RemotePlusLibrary.Client;
using RemotePlusLibrary.Contracts;
using RemotePlusLibrary.Core;
using RemotePlusLibrary.Extension.ResourceSystem;
using RemotePlusLibrary.RequestSystem;
using RemotePlusLibrary.Security.AccountSystem;
using RemotePlusLibrary.Security.Authentication;
using RemotePlusLibrary.SubSystem.Command;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using TinyMessenger;

namespace RemotePlusClient
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Multiple,
        IncludeExceptionDetailInFaults = true,
        UseSynchronizationContext = false)]
    public class ClientCallback : IRemoteClient
    {
        IConnectionManager _connManager = null;
        IWindowManager _winManager = null;
        IDialogManager _dialogManager = null;
        ILogFactory _logger = null;
        public ClientCallback(IConnectionManager cm, IWindowManager wm, ILogFactory lf, IDialogManager df)
        {
            _connManager = cm;
            _winManager = wm;
            _logger = lf;
            _dialogManager = df;
        }
        public void Beep(int Hertz, int Duration)
        {
            Console.Beep(Hertz, Duration);
        }

        public void ChangePrompt(Guid serverGuid, PromptBuilder newPrompt)
        {
            
        }

        public void Disconnect(Guid serverGuid, string Reason)
        {
            _logger.Log($"Server disconnected: {Reason}", LogLevel.Info);
            _connManager.Close();
        }

        public void DisposeCurrentRequest(Guid serverGuid)
        {
            Task.Factory.StartNew(() => RequestStore.DisposeCurrentRequest());
        }

        public PromptBuilder GetCurrentPrompt()
        {
            return null;
        }

        public Resource GetResource(string resourceIdentifier)
        {
            return null;
        }

        public bool HasKnownType(string name)
        {
            return DefaultKnownTypeManager.HasName(name);
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

        public void PublishEvent(ITinyMessage message)
        {
            throw new NotImplementedException();
        }

        public ClientBuilder RegisterClient()
        {
            ClientBuilder cb = new ClientBuilder(ClientType.GUI);
            cb.FriendlyName = "RemotePlus Client";
            return cb;
        }

        public void RegistirationComplete(Guid serverGuid)
        {
            _logger.Log("Registiration complete!!!", LogLevel.Info);
        }

        public UserCredentials RequestAuthentication(Guid serverGuid, AuthenticationRequest Request)
        {
            var result = _dialogManager.Show<AuthenticationViewModel>(Request);
            return result.ViewModel.Credentails;
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

        public void Speak(string Message, System.Speech.Synthesis.VoiceGender Gender, System.Speech.Synthesis.VoiceAge Age)
        {
            throw new NotImplementedException();
        }

        public void TellMessage(Guid serverGuid, string Message, LogLevel o)
        {
            _logger.Log(Message, o, $"Server Host ({serverGuid})");
        }

        public void WriteToClientConsole(Guid serverGuid, string Message)
        {
            _winManager.GetAllByID<ConsoleViewModel>(serverGuid).ToList().ForEach(t => t.ViewModel.AppendLine(Message));
        }

        public void WriteToClientConsole(Guid serverGuid, string Message, LogLevel level)
        {
            _winManager.GetAllByID<ConsoleViewModel>(serverGuid).ToList().ForEach(t => t.ViewModel.AppendLine(Message));
        }

        public void WriteToClientConsole(Guid serverGuid, ConsoleText text)
        {
            _winManager.GetAllByID<ConsoleViewModel>(serverGuid).ToList().ForEach(t => t.ViewModel.AppendLine(text));
        }

        public void WriteToClientConsole(Guid serverGuid, string Message, LogLevel level, string from)
        {
            _winManager.GetAllByID<ConsoleViewModel>(serverGuid).ToList().ForEach(t => t.ViewModel.AppendLine(Message));
        }

        public void WriteToClientConsoleNoNewLine(Guid serverGuid, string Message)
        {
            _winManager.GetAllByID<ConsoleViewModel>(serverGuid).ToList().ForEach(t => t.ViewModel.Append(Message));
        }

        public void SetClientConsoleBackgroundColor(Guid serverGuid, Color bgColor)
        {
            _winManager.GetAllByID<ConsoleViewModel>(serverGuid).ToList().ForEach(t => t.ViewModel.SetBackgroundColor(bgColor));
        }

        public void SetClientConsoleForegroundColor(Guid serverGuid, Color fgColor)
        {
            _winManager.GetAllByID<ConsoleViewModel>(serverGuid).ToList().ForEach(t => t.ViewModel.SetForegroundColor(fgColor));
        }

        public void ResetClientConsoleColor(Guid serverGuid)
        {
            _winManager.GetAllByID<ConsoleViewModel>(serverGuid).ToList().ForEach(t => t.ViewModel.ResetColors());
        }

        public void ClearClientConsole(Guid serverGuid)
        {
            _winManager.GetAllByID<ConsoleViewModel>(serverGuid).ToList().ForEach(t => t.ViewModel.ResetText());
        }
        public void UpdateRequest(Guid serverGuid, UpdateRequestBuilder message)
        {
            Task.Factory.StartNew(() => RequestStore.Update(serverGuid, message));
        }
    }
}