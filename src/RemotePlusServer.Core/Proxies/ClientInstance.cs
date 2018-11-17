using System.Collections.Generic;
using System.Speech.Synthesis;
using System.Windows.Forms;
using RemotePlusLibrary;
using RemotePlusLibrary.Contracts;
using RemotePlusLibrary.RequestSystem;
using RemotePlusLibrary.RequestSystem.DefaultRequestBuilders;
using RemotePlusLibrary.Scripting;

namespace RemotePlusServer.Core.Proxies
{
    public class ClientInstance : IBidirectionalContract
    {
        [IndexScriptObject]
        public string ClientType => ServerManager.ServerRemoteService.RemoteInterface.Client.ClientType.ToString();
        [IndexScriptObject]
        public string requestString(string prompt)
        {
            var builder = new RequestStringRequestBuilder()
            {
                Message = prompt
            };
            return ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.RequestInformation(builder).Data.ToString();
        }
        [IndexScriptObject]
        public void speak(string message, VoiceGender voiceGender, VoiceAge voiceAge)
        {
            ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.Speak(message, voiceGender, voiceAge);
        }
        [IndexScriptObject]
        public ReturnData sendRequest(RequestBuilder builder)
        {
            return ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.RequestInformation(builder);
        }
        [IndexScriptObject]
        public void postMessage(string message)
        {
            ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.SendSignal(new SignalMessage("recon_post", message));
        }
        [IndexScriptObject]
        public void PlaySound(string FileName)
        {
            ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.PlaySound(FileName);
        }
        [IndexScriptObject]
        public void PlaySoundLoop(string FileName)
        {
            ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.PlaySoundLoop(FileName);
        }
        [IndexScriptObject]
        public void PlaySoundSync(string FileName)
        {
            ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.PlaySoundSync(FileName);
        }
        [IndexScriptObject]
        public void RunProgram(string Program, string Argument, bool ignore)
        {
            ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.RunProgram(Program, Argument, ignore);
        }
        [IndexScriptObject]
        public void Beep(int Hertz, int Duration)
        {
            ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.Beep(Hertz, Duration);
        }
        [IndexScriptObject]
        public DialogResult ShowMessageBox(string Message, string Caption, MessageBoxIcon Icon, MessageBoxButtons Buttons)
        {
            return ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.ShowMessageBox(Message, Caption, Icon, Buttons);
        }
        [IndexScriptObject]
        public void Speak(string Message, VoiceGender Gender, VoiceAge Age)
        {
            ServerManager.ServerRemoteService.RemoteInterface.Client.ClientCallback.Speak(Message, Gender, Age);
        }

        public void SendSignal(SignalMessage signal)
        {
            throw new System.NotImplementedException();
        }
    }
}