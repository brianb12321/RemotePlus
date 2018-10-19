using System.Collections.Generic;
using System.ServiceModel;
using RemotePlusLibrary.Core;
using RemotePlusLibrary.Extension.CommandSystem;
using RemotePlusLibrary.Extension.CommandSystem.CommandClasses;
using RemotePlusLibrary.Scripting;
using RemotePlusLibrary.Security.AccountSystem;
using RemotePlusLibrary.Core.Faults;
using RemotePlusLibrary.Configuration.ServerSettings;
using RemotePlusLibrary.FileTransfer.BrowserClasses;

namespace RemotePlusLibrary.Contracts
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    /// <summary>
    /// The operations that the client can perform on the server.
    /// </summary>
    [ServiceContract(CallbackContract = typeof(IRemoteClient))]
    [ServiceKnownType("GetKnownTypes", typeof(DefaultKnownTypeManager))]
    public interface IRemote : IBidirectionalContract
    {
        [OperationContract()]
        [FaultContract(typeof(ServerFault))]
        [FaultContract(typeof(ProxyFault))]
        void Register(RegisterationObject Settings);
        [OperationContract]
        [FaultContract(typeof(ServerFault))]
        [FaultContract(typeof(ProxyFault))]
        CommandPipeline RunServerCommand(string Command, CommandExecutionMode commandMode);
        [OperationContract()]
        [FaultContract(typeof(ServerFault))]
        [FaultContract(typeof(ProxyFault))]
        void UpdateServerSettings(ServerSettings Settings);
        [OperationContract]
        [FaultContract(typeof(ServerFault))]
        [FaultContract(typeof(ProxyFault))]
        ServerSettings GetServerSettings();
        [OperationContract()]
        [FaultContract(typeof(ServerFault))]
        [FaultContract(typeof(ProxyFault))]
        void Restart();
        [OperationContract]
        [FaultContract(typeof(ServerFault))]
        [FaultContract(typeof(ProxyFault))]
        UserAccount GetLoggedInUser();
        [OperationContract]
        [FaultContract(typeof(ServerFault))]
        [FaultContract(typeof(ProxyFault))]
        IEnumerable<string> GetCommandsAsStrings();
        [OperationContract]
        [FaultContract(typeof(ServerFault))]
        [FaultContract(typeof(ProxyFault))]
        IEnumerable<CommandDescription> GetCommands();
        [OperationContract()]
        [FaultContract(typeof(ServerFault))]
        [FaultContract(typeof(ProxyFault))]
        void SwitchUser();
        [OperationContract(Name = "ServerDisconnect")]
        [FaultContract(typeof(ServerFault))]
        [FaultContract(typeof(ProxyFault))]
        void Disconnect();
        [OperationContract()]
        [FaultContract(typeof(ServerFault))]
        [FaultContract(typeof(ProxyFault))]
        void EncryptFile(string fileName, string password);
        [FaultContract(typeof(ServerFault))]
        [FaultContract(typeof(ProxyFault))]
        [OperationContract()]
        void DecryptFile(string fileName, string password);
        [FaultContract(typeof(ServerFault))]
        [FaultContract(typeof(ProxyFault))]
        [OperationContract]
        string GetCommandHelpPage(string command);
        [FaultContract(typeof(ServerFault))]
        [FaultContract(typeof(ProxyFault))]
        [OperationContract]
        string GetCommandHelpDescription(string command);
        [FaultContract(typeof(ServerFault))]
        [FaultContract(typeof(ProxyFault))]
        [OperationContract]
        [ServiceKnownType(typeof(RemoteDrive))]
        [ServiceKnownType(typeof(RemoteDirectory))]
        IDirectory GetRemoteFiles(string path, bool useRequest);
        [OperationContract]
        [FaultContract(typeof(ServerFault))]
        void UploadBytesToPackageSystem(byte[] data, int length, string name);
        [OperationContract]
        [FaultContract(typeof(ServerFault))]
        [FaultContract(typeof(ProxyFault))]
        string ReadFileAsString(string fileName);
        [OperationContract]
        [FaultContract(typeof(ServerFault))]
        [FaultContract(typeof(ProxyFault))]
        ScriptGlobalInformation[] GetScriptGlobals();
        [OperationContract]
        [FaultContract(typeof(ServerFault))]
        [FaultContract(typeof(ProxyFault))]
        bool ExecuteScript(string script);
    }
}