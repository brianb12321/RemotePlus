using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Speech.Synthesis;
using System.Threading.Tasks;
using System.Windows.Forms;
using RemotePlusLibrary;
using RemotePlusLibrary.Configuration.ServerSettings;
using RemotePlusLibrary.Discovery;
using RemotePlusLibrary.Extension.ResourceSystem;
using RemotePlusLibrary.FileTransfer.BrowserClasses;
using RemotePlusLibrary.Scripting;
using RemotePlusLibrary.Security.AccountSystem;
using RemotePlusLibrary.SubSystem.Command;
using RemotePlusLibrary.SubSystem.Command.CommandClasses;
using TinyMessenger;

namespace RemotePlusClient.CommonUI.ConnectionClients
{
    public class ProxyClient : DuplexClientBase<IProxyRemote>, IProxyRemote
    {
        public ProxyClient(object callbackInstance) : base(callbackInstance)
        {
        }

        public ProxyClient(InstanceContext callbackInstance) : base(callbackInstance)
        {
        }

        public ProxyClient(object callbackInstance, string endpointConfigurationName) : base(callbackInstance, endpointConfigurationName)
        {
        }

        public ProxyClient(object callbackInstance, ServiceEndpoint endpoint) : base(callbackInstance, endpoint)
        {
        }

        public ProxyClient(InstanceContext callbackInstance, string endpointConfigurationName) : base(callbackInstance, endpointConfigurationName)
        {
        }

        public ProxyClient(InstanceContext callbackInstance, ServiceEndpoint endpoint) : base(callbackInstance, endpoint)
        {
        }

        public ProxyClient(object callbackInstance, string endpointConfigurationName, string remoteAddress) : base(callbackInstance, endpointConfigurationName, remoteAddress)
        {
        }

        public ProxyClient(object callbackInstance, string endpointConfigurationName, EndpointAddress remoteAddress) : base(callbackInstance, endpointConfigurationName, remoteAddress)
        {
        }

        public ProxyClient(object callbackInstance, System.ServiceModel.Channels.Binding binding, EndpointAddress remoteAddress) : base(callbackInstance, binding, remoteAddress)
        {
        }

        public ProxyClient(InstanceContext callbackInstance, string endpointConfigurationName, string remoteAddress) : base(callbackInstance, endpointConfigurationName, remoteAddress)
        {
        }

        public ProxyClient(InstanceContext callbackInstance, string endpointConfigurationName, EndpointAddress remoteAddress) : base(callbackInstance, endpointConfigurationName, remoteAddress)
        {
        }

        public ProxyClient(InstanceContext callbackInstance, System.ServiceModel.Channels.Binding binding, EndpointAddress remoteAddress) : base(callbackInstance, binding, remoteAddress)
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

        public void Connect()
        {
            base.Open();
        }

        public object ExecuteScript(string script)
        {
            return Channel.ExecuteScript(script);
        }

        public string ReadFileAsString(string fileName)
        {
            return Channel.ReadFileAsString(fileName);
        }
        public override string ToString()
        {
            return base.ToString();
        }

        public Guid[] GetServers()
        {
            return Channel.GetServers();
        }

        public void ProxyRegister()
        {
            Channel.ProxyRegister();
        }

        public void SelectServer(int serverPosition)
        {
            Channel.SelectServer(serverPosition);
        }

        public void ProxyDisconnect()
        {
            Channel.ProxyDisconnect();
        }

        public void SelectServer(Guid guid)
        {
            Channel.SelectServer(guid);
        }

        public Guid GetSelectedServerGuid()
        {
            return Channel.GetSelectedServerGuid();
        }

        public CommandPipeline ExecuteProxyCommand(string command, CommandExecutionMode mode)
        {
            return Channel.ExecuteProxyCommand(command, mode);
        }

        public void SendSignal(SignalMessage signal)
        {
            Channel.SendSignal(signal);
        }

        public void UploadBytesToResource(byte[] data, int length, string friendlyName, string name, string path)
        {
            Channel.UploadBytesToResource(data, length, friendlyName, name, path);
        }

        public Resource GetResource(string resourceIdentifier)
        {
            return Channel.GetResource(resourceIdentifier);
        }

        public object ExecuteProxyScript(string script)
        {
            return Channel.ExecuteProxyScript(script);
        }

        public void PublishEvent(ITinyMessage message)
        {
            Channel.PublishEvent(message);
        }

        public bool HasKnownType(string name)
        {
            return Channel.HasKnownType(name);
        }

        public Task<CommandPipeline> ExecuteProxyCommandAsync(string command, CommandExecutionMode mode)
        {
            return Channel.ExecuteProxyCommandAsync(command, mode);
        }

        public Task<CommandPipeline> RunServerCommandAsync(string command, CommandExecutionMode commandMode)
        {
            return Channel.RunServerCommandAsync(command, commandMode);
        }

        public void CancelProxyCommand()
        {
            Channel.CancelProxyCommand();
        }

        public void CancelServerCommand()
        {
            Channel.CancelServerCommand();
        }
    }
}