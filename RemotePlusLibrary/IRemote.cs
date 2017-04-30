using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Windows.Forms;
using System.Speech.Synthesis;
using Logging;
using RemotePlusLibrary.Extension;
using RemotePlusLibrary.FileTransfer;

namespace RemotePlusLibrary
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract(CallbackContract = typeof(IRemoteClient))]
    public interface IRemote
    {
        [OperationContract]
        void PlaySound(string FileName);
        [OperationContract(IsOneWay = true)]
        void PlaySoundLoop(string FileName);
        [OperationContract(IsOneWay = true)]
        void PlaySoundSync(string FileName);
        [OperationContract(IsOneWay = true)]
        void RunProgram(string Program, string Argument);
        [OperationContract(IsOneWay = true)]
        void Beep(int Hertz, int Duration);
        [OperationContract(IsOneWay = true)]
        void ShowMessageBox(string Message, string Caption, MessageBoxIcon Icon, MessageBoxButtons Buttons);
        [OperationContract(IsOneWay = true)]
        void Speak(string Message, VoiceGender Gender, VoiceAge Age);
        [OperationContract(IsOneWay = true)]
        void Register(RegistirationObject Settings);
        [OperationContract]
        List<LogItem> RunServerCommand(string Command);
        [OperationContract(IsOneWay = true)]
        void UpdateServerSettings(ServerSettings Settings);
        [OperationContract]
        ServerSettings GetServerSettings();
        [OperationContract(IsOneWay = true)]
        void Restart();
        [OperationContract]
        RemoteFileInfo DownloadFile(DownloadRequest request);
        [OperationContract(IsOneWay = true)]
        void UploadFile(RemoteFileInfo request);
        [OperationContract]
        UserAccount GetLoggedInUser();
        [OperationContract]
        OperationStatus RunExtension(string ExtensionName, params object[] Args);
        [OperationContract]
        void ReplyToExtension(Func<object> Reply);
        [OperationContract(IsOneWay = true)]
        void HaultExtension();
        [OperationContract(IsOneWay = true)]
        void ResumeExtension();
        [OperationContract]
        List<ExtensionDetails> GetExtensionNames();
        [OperationContract]
        List<string> GetCommands();
    }
}
