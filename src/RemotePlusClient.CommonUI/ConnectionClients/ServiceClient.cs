using System.Collections.Generic;
using RemotePlusLibrary;
using System.ServiceModel;
using RemotePlusLibrary.Extension.CommandSystem;
using RemotePlusLibrary.Extension.CommandSystem.CommandClasses;
using RemotePlusLibrary.Scripting;
using System.Speech.Synthesis;
using System.Windows.Forms;
using System.ServiceModel.Description;
using RemotePlusLibrary.Security.AccountSystem;
using RemotePlusLibrary.Contracts;
using RemotePlusLibrary.Configuration.ServerSettings;
using RemotePlusLibrary.FileTransfer.BrowserClasses;
using RemotePlusLibrary.Extension.ResourceSystem;
using TinyMessenger;

namespace RemotePlusClient.CommonUI.ConnectionClients
{
    /// <summary>
    /// Provides a wrapper of a WCF channel factory;
    /// </summary>
    public class ServiceClient : DuplexClientBase<IRemote>, IRemote
    {
        public int ServerPosition { get; set; } = 0;
        public ServiceClient(object callbackInstance) : base(callbackInstance)
        {
        }

        public ServiceClient(InstanceContext callbackInstance) : base(callbackInstance)
        {
        }

        public ServiceClient(object callbackInstance, string endpointConfigurationName) : base(callbackInstance, endpointConfigurationName)
        {
        }

        public ServiceClient(object callbackInstance, ServiceEndpoint endpoint) : base(callbackInstance, endpoint)
        {
        }

        public ServiceClient(InstanceContext callbackInstance, string endpointConfigurationName) : base(callbackInstance, endpointConfigurationName)
        {
        }

        public ServiceClient(InstanceContext callbackInstance, ServiceEndpoint endpoint) : base(callbackInstance, endpoint)
        {
        }

        public ServiceClient(object callbackInstance, string endpointConfigurationName, string remoteAddress) : base(callbackInstance, endpointConfigurationName, remoteAddress)
        {
        }

        public ServiceClient(object callbackInstance, string endpointConfigurationName, EndpointAddress remoteAddress) : base(callbackInstance, endpointConfigurationName, remoteAddress)
        {
        }

        public ServiceClient(object callbackInstance, System.ServiceModel.Channels.Binding binding, EndpointAddress remoteAddress) : base(callbackInstance, binding, remoteAddress)
        {
        }

        public ServiceClient(InstanceContext callbackInstance, string endpointConfigurationName, string remoteAddress) : base(callbackInstance, endpointConfigurationName, remoteAddress)
        {
        }

        public ServiceClient(InstanceContext callbackInstance, string endpointConfigurationName, EndpointAddress remoteAddress) : base(callbackInstance, endpointConfigurationName, remoteAddress)
        {
        }

        public ServiceClient(InstanceContext callbackInstance, System.ServiceModel.Channels.Binding binding, EndpointAddress remoteAddress) : base(callbackInstance, binding, remoteAddress)
        {
        }

        public void Beep(int Hertz, int Duration)
        {
            base.Channel.Beep(Hertz, Duration);
        }

        public void DecryptFile(string fileName, string password)
        {
            base.Channel.DecryptFile(fileName, password);
        }

        public void Disconnect()
        {
            base.Channel.Disconnect();
        }

        public void EncryptFile(string fileName, string password)
        {
            base.Channel.EncryptFile(fileName, password);
        }

        public string GetCommandHelpDescription(string command)
        {
            return Channel.GetCommandHelpDescription(command);
        }

        public string GetCommandHelpPage(string command)
        {
            return Channel.GetCommandHelpPage(command);
        }

        public IEnumerable<CommandDescription> GetCommands()
        {
            return Channel.GetCommands();
        }

        public IEnumerable<string> GetCommandsAsStrings()
        {
            return Channel.GetCommandsAsStrings();
        }

        public UserAccount GetLoggedInUser()
        {
            return Channel.GetLoggedInUser();
        }

        public IDirectory GetRemoteFiles(string path, bool useRequest)
        {
            return Channel.GetRemoteFiles(path, useRequest);
        }

        public ServerSettings GetServerSettings()
        {
            return Channel.GetServerSettings();
        }

        public void PlaySound(string FileName)
        {
            Channel.PlaySound(FileName);
        }

        public void PlaySoundLoop(string FileName)
        {
            Channel.PlaySoundLoop(FileName);
        }

        public void PlaySoundSync(string FileName)
        {
            Channel.PlaySoundSync(FileName);
        }

        public void Register(RegisterationObject Settings)
        {
            Channel.Register(Settings);
        }

        public void Restart()
        {
            Channel.Restart();
        }

        public void RunProgram(string Program, string Argument, bool shell, bool ignore)
        {
            Channel.RunProgram(Program, Argument, shell, ignore);
        }

        public CommandPipeline RunServerCommand(string Command, CommandExecutionMode commandMode)
        {
            return Channel.RunServerCommand(Command, commandMode);
        }

        public DialogResult ShowMessageBox(string Message, string Caption, MessageBoxIcon Icon, MessageBoxButtons Buttons)
        {
            return Channel.ShowMessageBox(Message, Caption, Icon, Buttons);
        }

        public void Speak(string Message, VoiceGender Gender, VoiceAge Age)
        {
            Channel.Speak(Message, Gender, Age);
        }

        public void SwitchUser()
        {
            Channel.SwitchUser();
        }

        public void UpdateServerSettings(ServerSettings Settings)
        {
            Channel.UpdateServerSettings(Settings);
        }
        public void Connect()
        {
            base.Open();
        }

        public bool ExecuteScript(string script)
        {
            return Channel.ExecuteScript(script);
        }

        public ScriptGlobalInformation[] GetScriptGlobals()
        {
            return Channel.GetScriptGlobals();
        }

        public string ReadFileAsString(string fileName)
        {
            return Channel.ReadFileAsString(fileName);
        }
        public override string ToString()
        {
            return ServerPosition.ToString();
        }

        public void SendSignal(SignalMessage signal)
        {
            Channel.SendSignal(signal);
        }

        public void UploadBytesToResource(byte[] data, int length, string friendlyName, string name)
        {
            Channel.UploadBytesToResource(data, length, friendlyName, name);
        }

        public Resource GetResource(string resourceIdentifier)
        {
            return Channel.GetResource(resourceIdentifier);
        }

        public void PublishEvent(ITinyMessage message)
        {
            Channel.PublishEvent(message);
        }

        public bool HasKnownType(string name)
        {
            return Channel.HasKnownType(name);
        }
    }
}