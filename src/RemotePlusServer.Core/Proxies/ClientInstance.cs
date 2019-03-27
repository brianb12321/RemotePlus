using System.Collections.Generic;
using System.Speech.Synthesis;
using System.Windows.Forms;
using RemotePlusLibrary;
using RemotePlusLibrary.Contracts;
using RemotePlusLibrary.Extension.ResourceSystem;
using RemotePlusLibrary.RequestSystem;
using RemotePlusLibrary.RequestSystem.DefaultRequestBuilders;
using RemotePlusLibrary.Scripting;
using TinyMessenger;

namespace RemotePlusServer.Core.Proxies
{
    public class ClientInstance : IBidirectionalContract
    {
        public string ClientType => ServerManager.ServerRemoteService.RemoteInterface.Client.ClientType.ToString();
        public string requestString(string prompt)
        {
            var builder = new RequestStringRequestBuilder()
            {
                Message = prompt
            };
            return ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.RequestInformation(builder).Data.ToString();
        }
        public void speak(string message, VoiceGender voiceGender, VoiceAge voiceAge)
        {
            ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.Speak(message, voiceGender, voiceAge);
        }
        public ReturnData sendRequest(RequestBuilder builder)
        {
            return ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.RequestInformation(builder);
        }
        public void postMessage(string message)
        {
            ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.SendSignal(new SignalMessage("recon_post", message));
        }
        public void PlaySound(string FileName)
        {
            ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.PlaySound(FileName);
        }
        public void PlaySoundLoop(string FileName)
        {
            ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.PlaySoundLoop(FileName);
        }
        public void PlaySoundSync(string FileName)
        {
            ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.PlaySoundSync(FileName);
        }
        public void RunProgram(string Program, string Argument, bool shell, bool ignore)
        {
            ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.RunProgram(Program, Argument, shell, ignore);
        }
        public void Beep(int Hertz, int Duration)
        {
            ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.Beep(Hertz, Duration);
        }
        public DialogResult ShowMessageBox(string Message, string Caption, MessageBoxIcon Icon, MessageBoxButtons Buttons)
        {
            return ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.ShowMessageBox(Message, Caption, Icon, Buttons);
        }
        public void Speak(string Message, VoiceGender Gender, VoiceAge Age)
        {
            ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.Speak(Message, Gender, Age);
        }

        public void SendSignal(SignalMessage signal)
        {
            throw new System.NotImplementedException();
        }

        public Resource GetResource(string resourceIdentifier)
        {
            return ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.GetResource(resourceIdentifier);
        }

        public void PublishEvent(ITinyMessage message)
        {
            ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.PublishEvent(message);
        }

        public bool HasKnownType(string name)
        {
            return ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.HasKnownType(name);
        }
    }
}