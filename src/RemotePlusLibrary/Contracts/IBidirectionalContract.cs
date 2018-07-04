using System;
using System.Runtime.Serialization.Formatters;
using System.ServiceModel;
using System.Speech.Synthesis;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RemotePlusLibrary.Contracts
{
    [ServiceContract(CallbackContract = typeof(IBidirectionalContract))]
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
        void RunProgram(string Program, string Argument);
        [FaultContract(typeof(ServerFault))]
        [OperationContract()]
        void Beep(int Hertz, int Duration);
        [OperationContract()]
        [FaultContract(typeof(ServerFault))]
        DialogResult ShowMessageBox(string Message, string Caption, MessageBoxIcon Icon, MessageBoxButtons Buttons);
        [OperationContract()]
        [FaultContract(typeof(ServerFault))]
        void Speak(string Message, VoiceGender Gender, VoiceAge Age);
    }
}