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
        CommandPipeline RunServerCommand(string Command, CommandExecutionMode commandMode);
        [OperationContract(IsOneWay = true)]
        void UpdateServerSettings(ServerSettings Settings);
        [OperationContract]
        ServerSettings GetServerSettings();
        [OperationContract(IsOneWay = true)]
        void Restart();
        [OperationContract]
        UserAccount GetLoggedInUser();
        [OperationContract]
        ExtensionReturn RunExtension(string ExtensionName, ExtensionExecutionContext Context, string[] args);
        [OperationContract]
        List<ExtensionDetails> GetExtensionNames();
        [OperationContract]
        IEnumerable<string> GetCommandsAsStrings();
        [OperationContract]
        IEnumerable<CommandDescription> GetCommands();
        [OperationContract]
        ServerExtensionCollectionProgrammer GetCollectionProgrammer();
        [OperationContract]
        ServerExtensionLibraryProgrammer GetServerLibraryProgrammer(string LibraryName);
        [OperationContract(Name = "GetServerExtensionProgrammer")]
        ServerExtensionProgrammer GetServerExtensionProgrammer(string ExtensionName);
        [OperationContract(Name = "GetServerExtensionProgrammerWithLibraryName")]
        ServerExtensionProgrammer GetServerExtensionProgrammer(string LibraryName, string ExtensionName);
        [OperationContract(IsOneWay = true)]
        void ProgramServerEstensionCollection(ServerExtensionCollectionProgrammer collectProgrammer);
        [OperationContract(IsOneWay = true)]
        void ProgramServerExtesnionLibrary(ServerExtensionLibraryProgrammer libProgrammer);
        [OperationContract(IsOneWay = true)]
        void ProgramServerExtension(string LibraryName, ServerExtensionProgrammer seProgrammer);
        [OperationContract(IsOneWay = true)]
        void SwitchUser();
        [OperationContract(IsOneWay = true)]
        void Disconnect();
        [OperationContract(IsOneWay = true)]
        void EncryptFile(string fileName, string password);
        [OperationContract(IsOneWay = true)]
        void DecryptFile(string fileName, string password);
        [OperationContract]
        string GetCommandHelpPage(string command);
        [OperationContract]
        string GetCommandHelpDescription(string command);
        [OperationContract]
        RemoteDirectory GetRemoteFiles(bool useRequest);
        [OperationContract]
        EmailSettings GetServerEmailSettings();
        [OperationContract]
        void UpdateServerEmailSettings(EmailSettings emailSetting);
        [OperationContract]
        bool SendEmail(string To, string Subject, string Message);
    }
}
