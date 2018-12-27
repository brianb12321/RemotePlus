using RemotePlusLibrary.Core;
using RemotePlusLibrary.Extension.ResourceSystem;
using System;
using System.Runtime.Serialization.Formatters;
using System.ServiceModel;
using System.Speech.Synthesis;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RemotePlusLibrary.Contracts
{
    [ServiceContract(CallbackContract = typeof(IBidirectionalContract))]
    [ServiceKnownType("GetKnownTypes", typeof(DefaultKnownTypeManager))]
    public interface IBidirectionalContract
    {
        [OperationContract]
        [FaultContract(typeof(ServerFault))]
        void PlaySound(string FileName);
        [OperationContract()]
        [FaultContract(typeof(ServerFault))]
        void PlaySoundLoop(string FileName);
        [OperationContract()]
        [FaultContract(typeof(ServerFault))]
        void PlaySoundSync(string FileName);
        [OperationContract()]
        [FaultContract(typeof(ServerFault))]
        void RunProgram(string Program, string Argument, bool shell, bool ignore);
        [FaultContract(typeof(ServerFault))]
        [OperationContract()]
        void Beep(int Hertz, int Duration);
        [OperationContract()]
        [FaultContract(typeof(ServerFault))]
        DialogResult ShowMessageBox(string Message, string Caption, MessageBoxIcon Icon, MessageBoxButtons Buttons);
        [OperationContract()]
        [FaultContract(typeof(ServerFault))]
        void Speak(string Message, VoiceGender Gender, VoiceAge Age);
        [OperationContract]
        [FaultContract(typeof(ServerFault))]
        void SendSignal(SignalMessage signal);
        [OperationContract]
        [FaultContract(typeof(ServerFault))]
        Resource GetResource(string resourceIdentifier);
        [OperationContract]
        [FaultContract(typeof(ServerFault))]
        void PublishEvent(TinyMessenger.ITinyMessage message);
        [OperationContract]
        [FaultContract(typeof(ServerFault))]
        bool HasKnownType(string name);
    }
}