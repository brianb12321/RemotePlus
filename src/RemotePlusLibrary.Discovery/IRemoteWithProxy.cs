using RemotePlusLibrary.Client;
using RemotePlusLibrary.Configuration.ServerSettings;
using RemotePlusLibrary.Contracts;
using RemotePlusLibrary.Core;
using RemotePlusLibrary.Core.Faults;
using RemotePlusLibrary.Extension.CommandSystem;
using RemotePlusLibrary.Extension.CommandSystem.CommandClasses;
using RemotePlusLibrary.FileTransfer.BrowserClasses;
using RemotePlusLibrary.Scripting;
using RemotePlusLibrary.Security.AccountSystem;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace RemotePlusLibrary.Discovery
{
    [ServiceContract(CallbackContract = typeof(IRemoteClient))]
    [ServiceKnownType("GetKnownTypes", typeof(DefaultKnownTypeManager))]
    public interface IRemoteWithProxy : IClient, IBidirectionalContract
    {
        [OperationContract]
        void ServerRegistered(Guid serverGuid);
        [OperationContract()]
        [FaultContract(typeof(ServerFault))]
        void Register(RegisterationObject Settings);
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
        IEnumerable<string> GetCommandsAsStrings();
        [OperationContract]
        [FaultContract(typeof(ServerFault))]
        IEnumerable<CommandDescription> GetCommands();
        [OperationContract()]
        [FaultContract(typeof(ServerFault))]
        void SwitchUser();
        [OperationContract(Name = "ServerDisconnect")]
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
        [OperationContract]
        [FaultContract(typeof(ServerFault))]
        void UploadBytesToResource(byte[] data, int length, string friendlyName, string name, string path);
        [OperationContract]
        [FaultContract(typeof(ServerFault))]
        string ReadFileAsString(string fileName);
        [OperationContract]
        [FaultContract(typeof(ServerFault))]
        ScriptGlobalInformation[] GetScriptGlobals();
        [OperationContract]
        [FaultContract(typeof(ServerFault))]
        object ExecuteScript(string script);
    }
}