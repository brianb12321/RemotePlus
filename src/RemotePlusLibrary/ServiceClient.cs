using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RemotePlusLibrary;
using System.ServiceModel;
using RemotePlusLibrary.Core.EmailService;
using RemotePlusLibrary.Extension;
using RemotePlusLibrary.Extension.CommandSystem;
using RemotePlusLibrary.Extension.CommandSystem.CommandClasses;
using RemotePlusLibrary.Extension.Programmer;
using RemotePlusLibrary.FileTransfer;
using System.Speech.Synthesis;
using System.Windows.Forms;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;

namespace RemotePlusLibrary
{
    /// <summary>
    /// Provides a wrapper of a WCF channel factory;
    /// </summary>
    public class ServiceClient : DuplexClientBase<IRemote>, IRemote
    {
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

        public ServerExtensionCollectionProgrammer GetCollectionProgrammer()
        {
            return base.Channel.GetCollectionProgrammer();
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

        public List<ExtensionDetails> GetExtensionNames()
        {
            return Channel.GetExtensionNames();
        }

        public UserAccount GetLoggedInUser()
        {
            return Channel.GetLoggedInUser();
        }

        public IDirectory GetRemoteFiles(string path, bool useRequest)
        {
            return Channel.GetRemoteFiles(path, useRequest);
        }

        public EmailSettings GetServerEmailSettings()
        {
            return Channel.GetServerEmailSettings();
        }

        public ServerExtensionProgrammer GetServerExtensionProgrammer(string ExtensionName)
        {
            return Channel.GetServerExtensionProgrammer(ExtensionName);
        }

        public ServerExtensionProgrammer GetServerExtensionProgrammer(string LibraryName, string ExtensionName)
        {
            return Channel.GetServerExtensionProgrammer(LibraryName, ExtensionName);
        }

        public ServerExtensionLibraryProgrammer GetServerLibraryProgrammer(string LibraryName)
        {
            return Channel.GetServerLibraryProgrammer(LibraryName);
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

        public void ProgramServerEstensionCollection(ServerExtensionCollectionProgrammer collectProgrammer)
        {
            Channel.ProgramServerEstensionCollection(collectProgrammer);
        }

        public void ProgramServerExtension(string LibraryName, ServerExtensionProgrammer seProgrammer)
        {
            Channel.ProgramServerExtension(LibraryName, seProgrammer);
        }

        public void ProgramServerExtesnionLibrary(ServerExtensionLibraryProgrammer libProgrammer)
        {
            Channel.ProgramServerExtesnionLibrary(libProgrammer);
        }

        public void Register(RegisterationObject Settings)
        {
            Channel.Register(Settings);
        }

        public void Restart()
        {
            Channel.Restart();
        }

        public ExtensionReturn RunExtension(string ExtensionName, ExtensionExecutionContext Context, string[] args)
        {
            return Channel.RunExtension(ExtensionName, Context, args);
        }

        public void RunProgram(string Program, string Argument)
        {
            Channel.RunProgram(Program, Argument);
        }

        public CommandPipeline RunServerCommand(string Command, CommandExecutionMode commandMode)
        {
            return Channel.RunServerCommand(Command, commandMode);
        }

        public bool SendEmail(string To, string Subject, string Message)
        {
            return Channel.SendEmail(To, Subject, Message);
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

        public void UpdateServerEmailSettings(EmailSettings emailSetting)
        {
            Channel.UpdateServerEmailSettings(emailSetting);
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
    }
}
