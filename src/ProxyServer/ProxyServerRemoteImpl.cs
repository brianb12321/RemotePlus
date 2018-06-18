using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using RemotePlusLibrary.Discovery;
using RemotePlusLibrary;
using RemotePlusLibrary.Core.EmailService;
using RemotePlusLibrary.Extension;
using RemotePlusLibrary.Extension.CommandSystem;
using RemotePlusLibrary.Extension.CommandSystem.CommandClasses;
using RemotePlusLibrary.Extension.Programmer;
using RemotePlusLibrary.FileTransfer;
using RemotePlusLibrary.Scripting;
using RemotePlusLibrary.Security.AccountSystem;
using System.Speech.Synthesis;
using System.Windows.Forms;
using Logging;
using RemotePlusLibrary.Core;

namespace ProxyServer
{
    [ServiceBehavior(IncludeExceptionDetailInFaults = true,
        InstanceContextMode = InstanceContextMode.Single,
        ConcurrencyMode = ConcurrencyMode.Multiple,
        UseSynchronizationContext = false)]
    [CallbackBehavior(IncludeExceptionDetailInFaults = true,
        ConcurrencyMode = ConcurrencyMode.Multiple,
         UseSynchronizationContext = false)]
    [GlobalException(typeof(GlobalErrorHandler))]
    public class ProxyServerRemoteImpl : IProxyServerRemote, IProxyRemote
    {
        Client<IRemote> selectedClient = null;
        Client<IRemoteClient> ProxyClient = null;
        List<Client<IRemote>> ConnectedServers { get; } = new List<Client<IRemote>>();

        public void Beep(int Hertz, int Duration)
        {
            selectedClient.ClientCallback.Beep(Hertz, Duration);
        }

        public void DecryptFile(string fileName, string password)
        {
            selectedClient.ClientCallback.DecryptFile(fileName, password);
        }

        public void Disconnect()
        {
            selectedClient.ClientCallback.Disconnect();
            ConnectedServers.Remove(selectedClient);
        }

        public void EncryptFile(string fileName, string password)
        {
            selectedClient.ClientCallback.EncryptFile(fileName, password);
        }

        public bool ExecuteScript(string script)
        {
            return selectedClient.ClientCallback.ExecuteScript(script);
        }

        public string GetCommandHelpDescription(string command)
        {
            return selectedClient.ClientCallback.GetCommandHelpDescription(command);
        }

        public string GetCommandHelpPage(string command)
        {
            return selectedClient.ClientCallback.GetCommandHelpPage(command);
        }

        public IEnumerable<CommandDescription> GetCommands()
        {
            return selectedClient.ClientCallback.GetCommands();
        }

        public IEnumerable<string> GetCommandsAsStrings()
        {
            return selectedClient.ClientCallback.GetCommandsAsStrings();
        }

        public List<ExtensionDetails> GetExtensionNames()
        {
            return selectedClient.ClientCallback.GetExtensionNames();
        }

        public UserAccount GetLoggedInUser()
        {
            return selectedClient.ClientCallback.GetLoggedInUser();
        }

        public IDirectory GetRemoteFiles(string path, bool useRequest)
        {
            return selectedClient.ClientCallback.GetRemoteFiles(path, useRequest);
        }

        public ScriptGlobalInformation[] GetScriptGlobals()
        {
            return selectedClient.ClientCallback.GetScriptGlobals();
        }

        public EmailSettings GetServerEmailSettings()
        {
            return selectedClient.ClientCallback.GetServerEmailSettings();
        }

        public List<string> GetServerRoleNames()
        {
            return selectedClient.ClientCallback.GetServerRoleNames();
        }

        public Guid[] GetServers()
        {
            return ConnectedServers.Select(s => s.UniqueID).ToArray();
        }

        public ServerSettings GetServerSettings()
        {
            return selectedClient.ClientCallback.GetServerSettings();
        }

        public void PlaySound(string FileName)
        {
            selectedClient.ClientCallback.PlaySound(FileName);
        }

        public void PlaySoundLoop(string FileName)
        {
            selectedClient.ClientCallback.PlaySoundLoop(FileName);
        }

        public void PlaySoundSync(string FileName)
        {
            selectedClient.ClientCallback.PlaySoundSync(FileName);
        }

        public void ProxyDisconnect()
        {
            ProxyManager.Logger.AddOutput($"Client [{ProxyClient.UniqueID}] disconnected from proxy server. Proxy server notifying connected servers that the client has disconnected.", OutputLevel.Info);
            foreach(Client<IRemote> client in ConnectedServers)
            {
                client.ClientCallback.Disconnect();
                ProxyManager.Logger.AddOutput($"Server [{client.UniqueID}] notified of client disconnection.", OutputLevel.Info);
            }
        }

        public void ProxyRegister()
        {
            var callback = OperationContext.Current.GetCallbackChannel<IRemoteClient>();
            ProxyClient = Client<IRemoteClient>.Build(callback.RegisterClient(), callback);
        }

        public string ReadFileAsString(string fileName)
        {
            return selectedClient.ClientCallback.ReadFileAsString(fileName);
        }

        public void Register()
        {
            var callback = OperationContext.Current.GetCallbackChannel<IRemote>();
            var tempClient = Client<IRemote>.Build(new ClientBuilder(ClientType.Server), callback);
            ConnectedServers.Add(tempClient);
            ProxyManager.Logger.AddOutput($"Server [{tempClient.UniqueID}] joined the proxy cluster.", Logging.OutputLevel.Info);
        }

        public void Register(RegisterationObject Settings)
        {
            selectedClient.ClientCallback.Register(Settings);
        }

        public void Restart()
        {
            selectedClient.ClientCallback.Restart();
        }

        public ExtensionReturn RunExtension(string ExtensionName, ExtensionExecutionContext Context, string[] args)
        {
            return selectedClient.ClientCallback.RunExtension(ExtensionName, Context, args);
        }

        public void RunProgram(string Program, string Argument)
        {
            selectedClient.ClientCallback.RunProgram(Program, Argument);
        }

        public CommandPipeline RunServerCommand(string Command, CommandExecutionMode commandMode)
        {
            return selectedClient.ClientCallback.RunServerCommand(Command, commandMode);
        }

        public void SelectServer(int serverPosition)
        {
            selectedClient = ConnectedServers[serverPosition];
        }

        public void SelectServer(Guid guid)
        {
            selectedClient = ConnectedServers.First(s => s.UniqueID == guid);
        }

        public bool SendEmail(string To, string Subject, string Message)
        {
            return selectedClient.ClientCallback.SendEmail(To, Subject, Message);
        }

        public DialogResult ShowMessageBox(string Message, string Caption, MessageBoxIcon Icon, MessageBoxButtons Buttons)
        {
            return selectedClient.ClientCallback.ShowMessageBox(Message, Caption, Icon, Buttons);
        }

        public void Speak(string Message, VoiceGender Gender, VoiceAge Age)
        {
            selectedClient.ClientCallback.Speak(Message, Gender, Age);
        }

        public void SwitchUser()
        {
            selectedClient.ClientCallback.SwitchUser();
        }

        public void UpdateServerEmailSettings(EmailSettings emailSetting)
        {
            selectedClient.ClientCallback.UpdateServerEmailSettings(emailSetting);
        }

        public void UpdateServerSettings(ServerSettings Settings)
        {
            selectedClient.ClientCallback.UpdateServerSettings(Settings);
        }


        public void Disconnect(Guid guid, string Reason)
        {
            ProxyClient.ClientCallback.Disconnect(selectedClient.UniqueID, Reason);
        }
        public RemotePlusLibrary.Extension.CommandSystem.PromptBuilder GetCurrentPrompt()
        {
            return ProxyClient.ClientCallback.GetCurrentPrompt();
        }
        public ClientBuilder RegisterClient()
        {
            return ProxyClient.ClientCallback.RegisterClient();
        }

        public void RegistirationComplete(Guid guid)
        {
            ProxyClient.ClientCallback.RegistirationComplete(selectedClient.UniqueID);
        }
        public UserCredentials RequestAuthentication(Guid guid, AuthenticationRequest Request)
        {
            return ProxyClient.ClientCallback.RequestAuthentication(selectedClient.UniqueID, Request);
        }

        public ReturnData RequestInformation(Guid guid, RequestBuilder builder)
        {
            return ProxyClient.ClientCallback.RequestInformation(selectedClient.UniqueID, builder);
        }
        public void TellMessage(Guid guid, string Message, OutputLevel o)
        {
            ProxyClient.ClientCallback.TellMessage(selectedClient.UniqueID, Message, o);
        }

        public void TellMessage(Guid guid, UILogItem li)
        {
            ProxyClient.ClientCallback.TellMessage(selectedClient.UniqueID, li);
        }

        public void TellMessage(Guid guid, UILogItem[] Logs)
        {
            ProxyClient.ClientCallback.TellMessage(selectedClient.UniqueID, Logs);
        }

        public void TellMessageToServerConsole(Guid guid, UILogItem li)
        {
            ProxyClient.ClientCallback.TellMessageToServerConsole(selectedClient.UniqueID, li);
        }

        public void TellMessageToServerConsole(Guid guid, string Message)
        {
            ProxyClient.ClientCallback.TellMessageToServerConsole(selectedClient.UniqueID, Message);
        }

        public void TellMessageToServerConsole(Guid guid, ConsoleText text)
        {
            ProxyClient.ClientCallback.TellMessageToServerConsole(selectedClient.UniqueID, text);
        }

        public void SendSignal(Guid guid, SignalMessage signal)
        {
            ProxyClient.ClientCallback.SendSignal(selectedClient.UniqueID, signal);
        }

        public void ChangePrompt(Guid guid, RemotePlusLibrary.Extension.CommandSystem.PromptBuilder newPrompt)
        {
            ProxyClient.ClientCallback.ChangePrompt(selectedClient.UniqueID, newPrompt);
        }

        public Guid GetSelectedServerGuid()
        {
            return selectedClient.UniqueID;
        }
    }
}
