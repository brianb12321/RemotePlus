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
using RemotePlusLibrary.Core;
using RemotePlusLibrary.Extension.Programmer;
using System.IO;
using RemotePlusLibrary.Extension.CommandSystem;
using RemotePlusLibrary.Core.EmailService;
using RemotePlusLibrary.Extension.CommandSystem.CommandClasses;

namespace RemotePlusLibrary
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    /// <summary>
    /// The operations that the client can perform on the server.
    /// </summary>
    [ServiceContract(CallbackContract = typeof(IRemoteClient))]
    [ServiceKnownType("GetKnownTypes", typeof(DefaultKnownTypeManager))]
    public interface IRemote
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
        [OperationContract()]
        [FaultContract(typeof(ServerFault))]
        void Register(RegistirationObject Settings);
        [OperationContract]
        [FaultContract(typeof(ServerFault))]
        CommandPipeline RunServerCommand(string Command, CommandExecutionMode commandMode);
        [OperationContract()]
        [FaultContract(typeof(ServerFault))]
        void UpdateServerSettings(ServerSettings Settings);
        [OperationContract]
        [FaultContract(typeof(ServerFault))]
        ServerSettings GetServerSettings();
        [OperationContract()]
        [FaultContract(typeof(ServerFault))]
        void Restart();
        [OperationContract]
        [FaultContract(typeof(ServerFault))]
        UserAccount GetLoggedInUser();
        [OperationContract]
        [FaultContract(typeof(ServerFault))]
        ExtensionReturn RunExtension(string ExtensionName, ExtensionExecutionContext Context, string[] args);
        [FaultContract(typeof(ServerFault))]
        [OperationContract]
        List<ExtensionDetails> GetExtensionNames();
        [OperationContract]
        [FaultContract(typeof(ServerFault))]
        IEnumerable<string> GetCommandsAsStrings();
        [OperationContract]
        [FaultContract(typeof(ServerFault))]
        IEnumerable<CommandDescription> GetCommands();
        [OperationContract]
        [FaultContract(typeof(ServerFault))]
        ServerExtensionCollectionProgrammer GetCollectionProgrammer();
        [OperationContract]
        [FaultContract(typeof(ServerFault))]
        ServerExtensionLibraryProgrammer GetServerLibraryProgrammer(string LibraryName);
        [OperationContract(Name = "GetServerExtensionProgrammer")]
        [FaultContract(typeof(ServerFault))]
        ServerExtensionProgrammer GetServerExtensionProgrammer(string ExtensionName);
        [OperationContract(Name = "GetServerExtensionProgrammerWithLibraryName")]
        [FaultContract(typeof(ServerFault))]
        ServerExtensionProgrammer GetServerExtensionProgrammer(string LibraryName, string ExtensionName);
        [OperationContract()]
        [FaultContract(typeof(ServerFault))]
        void ProgramServerEstensionCollection(ServerExtensionCollectionProgrammer collectProgrammer);
        [OperationContract()]
        [FaultContract(typeof(ServerFault))]
        void ProgramServerExtesnionLibrary(ServerExtensionLibraryProgrammer libProgrammer);
        [OperationContract()]
        [FaultContract(typeof(ServerFault))]
        void ProgramServerExtension(string LibraryName, ServerExtensionProgrammer seProgrammer);
        [OperationContract()]
        [FaultContract(typeof(ServerFault))]
        void SwitchUser();
        [OperationContract()]
        [FaultContract(typeof(ServerFault))]
        void Disconnect();
        [OperationContract()]
        [FaultContract(typeof(ServerFault))]
        void EncryptFile(string fileName, string password);
        [FaultContract(typeof(ServerFault))]
        [OperationContract()]
        void DecryptFile(string fileName, string password);
        [FaultContract(typeof(ServerFault))]
        [OperationContract]
        string GetCommandHelpPage(string command);
        [FaultContract(typeof(ServerFault))]
        [OperationContract]
        string GetCommandHelpDescription(string command);
        [FaultContract(typeof(ServerFault))]
        [OperationContract]
        [ServiceKnownType(typeof(RemoteDrive))]
        [ServiceKnownType(typeof(RemoteDirectory))]
        IDirectory GetRemoteFiles(string path, bool useRequest);
        [FaultContract(typeof(ServerFault))]
        [OperationContract]
        EmailSettings GetServerEmailSettings();
        [OperationContract]
        [FaultContract(typeof(ServerFault))]
        void UpdateServerEmailSettings(EmailSettings emailSetting);
        [OperationContract]
        [FaultContract(typeof(ServerFault))]
        bool SendEmail(string To, string Subject, string Message);
        [OperationContract]
        [FaultContract(typeof(ServerFault))]
        bool ExecuteScript(string script);
    }
}