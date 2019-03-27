using System;
using System.Collections.Generic;
using System.ServiceModel;
using RemotePlusLibrary;
using System.Windows.Forms;
using System.Speech.Synthesis;
using RemotePlusLibrary.Core;
using System.IO;
using System.Linq;
using RemotePlusLibrary.Scripting;
using RemotePlusLibrary.Security.AccountSystem;
using RemotePlusLibrary.Discovery;
using RemotePlusLibrary.Core.Faults;
using RemotePlusLibrary.FileTransfer.BrowserClasses;
using RemotePlusLibrary.Contracts;
using RemotePlusLibrary.Security.Authentication;
using RemotePlusLibrary.Configuration.ServerSettings;
using RemotePlusLibrary.Client;
using RemotePlusServer.Core;
using BetterLogger;
using RemotePlusLibrary.Core.EventSystem;
using RemotePlusLibrary.Core.EventSystem.Events;
using RemotePlusLibrary.Core.IOC;
using RemotePlusLibrary.Extension.ResourceSystem;
using RemotePlusLibrary.Extension.ResourceSystem.ResourceTypes;
using TinyMessenger;
using System.Threading.Tasks;
using RemotePlusServer.Core.ExtensionSystem;
using RemotePlusLibrary.SubSystem.Command.CommandClasses;
using RemotePlusLibrary.SubSystem.Command;

namespace RemotePlusServer
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple,
        InstanceContextMode = InstanceContextMode.Single,
        IncludeExceptionDetailInFaults = true,
        UseSynchronizationContext = false,
        MaxItemsInObjectGraph = int.MaxValue)]
    [CallbackBehavior(IncludeExceptionDetailInFaults = true,
        ConcurrencyMode = ConcurrencyMode.Multiple,
        UseSynchronizationContext = false,
        MaxItemsInObjectGraph = int.MaxValue)]
    [GlobalExceptionIOCAttribute]
    [ServiceKnownType("GetKnownTypes", typeof(DefaultKnownTypeManager))]
    public class RemoteImpl : IRemote, IRemoteWithProxy
    {
        private ServerRemoteInterface _interface = null;
        public void SetRemoteInterface(RemotePlusLibrary.ServiceArchitecture.IRemotePlusService<ServerRemoteInterface> i)
        {
            _interface = new ServerRemoteInterface();
            i.RemoteInterface = _interface;
        }
        bool CheckRegisteration(string Action)
        {
            GlobalServices.Logger.Log($"Checking registiration for action {Action}.", LogLevel.Info);
            if (_interface.Registered != true)
            {
                GlobalServices.Logger.Log("The client is not registired to the server.", LogLevel.Error);
                if (ServerStartup.proxyChannelFactory.State == CommunicationState.Opened)
                {
                    ServerStartup.proxyChannel.TellMessage(Guid.NewGuid(), "You must be registered.", LogLevel.Error);
                    return false;
                }
                else
                {
                    OperationContext.Current.GetCallbackChannel<IRemoteClient>().Disconnect(Guid.NewGuid(), "You must be registered.");
                    return true;
                }
            }
            else
            {
                return true;
            }
        }
        public void Beep(int Hertz, int Duration)
        {
            if (CheckRegisteration("beep"))
            {
                _interface.Beep(Hertz, Duration);
                _interface.Client.ClientCallback.TellMessage($"Console beeped. Hertz: {Hertz}, Duration: {Duration}", LogLevel.Info);
            }
            // OperationContext.Current.OperationCompleted += (sender, e) => _interface.Client.ClientCallback.SendSignal(new SignalMessage(OPERATION_COMPLETED, ""));
        }

        public void PlaySound(string FileName)
        {
            if(CheckRegisteration("PlaySound"))
            {
                System.Media.SoundPlayer sp = new System.Media.SoundPlayer(FileName);
                sp.Play();
            }
        }

        public void PlaySoundLoop(string FileName)
        {
            if (CheckRegisteration("PlaySoundLoop"))
            {
                System.Media.SoundPlayer sp = new System.Media.SoundPlayer(FileName);
                sp.PlayLooping();
            }
            // OperationContext.Current.OperationCompleted += (sender, e) => _interface.Client.ClientCallback.SendSignal(new SignalMessage(OPERATION_COMPLETED, ""));
        }

        public void PlaySoundSync(string FileName)
        {
            if (CheckRegisteration("PlaySoundSync"))
            {
                System.Media.SoundPlayer sp = new System.Media.SoundPlayer(FileName);
                sp.PlaySync();
            }
            // OperationContext.Current.OperationCompleted += (sender, e) => _interface.Client.ClientCallback.SendSignal(new SignalMessage(OPERATION_COMPLETED, ""));
        }

        public void Register(RegisterationObject Settings)
        {
            GlobalServices.Logger.Log("A new client is awaiting registration.", LogLevel.Info);
            GlobalServices.Logger.Log("Instantiating callback object.", LogLevel.Debug);
            GlobalServices.Logger.Log("Getting ClientBuilder from client.", LogLevel.Debug);
            //Setup the client callback.
            BuildClient();
            GlobalServices.Logger.Log("Received registration object from client.", LogLevel.Info);
            this._interface.Settings = Settings;
            GlobalServices.Logger.Log("Processing registration object.", LogLevel.Debug);
            _interface.Client.ClientCallback.TellMessage("Processing registration object.", LogLevel.Debug);
            PerformAuthentication(Settings);
            //Setup the prompt if it is a command-line client.
            if (_interface.Client.ClientType == ClientType.CommandLine && ServerStartup.proxyChannelFactory == null)
            {
                _interface.Client.ClientCallback.ChangePrompt(new RemotePlusLibrary.SubSystem.Command.PromptBuilder()
                {
                    Path = _interface.CurrentPath,
                    AdditionalData = "Current Path",
                    CurrentUser = _interface.LoggedInUser.Credentials.Username
                });
            }
            //Publish the LoginEvent onto the event bus, notifying listeners that the user has attempted to login.
            ServerManager.EventBus.Publish(new LoginEvent(_interface.Registered, this));
        }

        private void BuildClient()
        {
            if (ServerManager.DefaultSettings.DiscoverySettings.DiscoveryBehavior == ProxyConnectionMode.Connect)
            {
                _interface.Client = Client<RemoteClient>.Build(ServerStartup.proxyChannel.RegisterClient(),
                    new RemoteClient(null, true, ServerStartup.proxyChannel, ServerManager.ServerGuid),
                    OperationContext.Current.Channel);
            }
            else
            {
                var callback = OperationContext.Current.GetCallbackChannel<IRemoteClient>();
                _interface.Client = Client<RemoteClient>.Build(callback.RegisterClient(), new RemoteClient(callback, false, null, ServerManager.ServerGuid), OperationContext.Current.Channel);
            }
        }

        const string REG_FAILED = "Registration failed. The most likely cause is invalid credentials or this account has been blocked. Make sure that the provided credentials are correct and also make sure the account was not blocked. If you still receive this error message, please check the server logs for more details.";
        private void PerformAuthentication(RegisterationObject regObject)
        {
            UserAccount account = null;
            if (regObject.LoginRightAway)
            {
                account = LogIn(regObject.Credentials);
            }
            else
            {
                GlobalServices.Logger.Log("Awaiting credentials from the client.", LogLevel.Info);
                _interface.Client.ClientCallback.TellMessage("Awaiting credentials from the client.", LogLevel.Info);
                UserCredentials upp = _interface.Client.ClientCallback.RequestAuthentication(new AuthenticationRequest(AuthenticationSeverity.Normal) { Reason = "The server requires credentials to register." });
                account = LogIn(upp);
            }
            if (account == null)
            {
                GlobalServices.Logger.Log($"Client {_interface.Client.FriendlyName} [{_interface.Client.UniqueID.ToString()}] disconnected. Failed to register to the server. Authentication failed.", LogLevel.Info);
                _interface.Client.ClientCallback.Disconnect(REG_FAILED + $" Provded username: {regObject.Credentials.Username}");
            }
            else
            {
                _interface.LoggedInUser = account;
                RegisterComplete();
            }
        }

        private void RegisterComplete()
        {
            IOCContainer.GetService<IScriptingEngine>().ResetSessionContext();
            GlobalServices.Logger.Log($"Client \"{_interface.Client.FriendlyName}\" [{_interface.Client.UniqueID}] Type: {_interface.Client.ClientType} registired.", LogLevel.Info);
            _interface.Registered = true;
            _interface.Client.ClientCallback.TellMessage("Registration complete.",LogLevel.Info);
            _interface.Client.ClientCallback.RegistirationComplete();
        }

        public void RunProgram(string Program, string Argument, bool shell, bool ignore)
        {
            if (CheckRegisteration("RunProgram"))
            {
                _interface.RunProgram(Program, Argument, shell, ignore);
            }
            // OperationContext.Current.OperationCompleted += (sender, e) => _interface.Client.ClientCallback.SendSignal(new SignalMessage(OPERATION_COMPLETED, ""));
        }

        public DialogResult ShowMessageBox(string Message, string Caption, System.Windows.Forms.MessageBoxIcon Icon, System.Windows.Forms.MessageBoxButtons Buttons)
        {
            if (CheckRegisteration("ShowMessageBox"))
            {
                return _interface.ShowMessageBox(Message, Caption, Icon, Buttons);
            }
            else
            {
                return DialogResult.Abort;
            }
            // OperationContext.Current.OperationCompleted += (sender, e) => _interface.Client.ClientCallback.SendSignal(new SignalMessage(OPERATION_COMPLETED, ""));
        }

        public void Speak(string Message, VoiceGender Gender, VoiceAge Age)
        {
            if (CheckRegisteration("Speak"))
            {
                _interface.Speak(Message, Gender, Age);
            }
        }

        public CommandPipeline RunServerCommand(string Command, CommandExecutionMode commandMode)
        {
            if (CheckRegisteration("RunServerCommand"))
            {
                return _interface.RunServerCommand(Command, commandMode);
            }
            else
            {
                return null;
            }
        }

        public void UpdateServerSettings(ServerSettings Settings)
        {
            if (CheckRegisteration("UpdateServerSettings"))
            {
                GlobalServices.Logger.Log("Updating server settings.", LogLevel.Info);
                ServerManager.DefaultSettings = Settings;
                _interface.Client.ClientCallback.TellMessage("Saving settings.", LogLevel.Info);
                ServerManager.DataAccess.SaveConfig(ServerManager.DefaultSettings, ServerSettings.SERVER_SETTINGS_FILE_PATH);
                _interface.Client.ClientCallback.TellMessage("Settings saved.", LogLevel.Info);
                GlobalServices.Logger.Log("Settings saved.", LogLevel.Info);
            }
            // OperationContext.Current.OperationCompleted += (sender, e) => _interface.Client.ClientCallback.SendSignal(new SignalMessage(OPERATION_COMPLETED, ""));
        }

        public ServerSettings GetServerSettings()
        {
            // OperationContext.Current.OperationCompleted += (sender, e) => _interface.Client.ClientCallback.SendSignal(new SignalMessage(OPERATION_COMPLETED, ""));
            if (CheckRegisteration("GetServerSettings"))
            {
                GlobalServices.Logger.Log("Retrieving server settings.", LogLevel.Info);
                return ServerManager.DefaultSettings;
            }
            else
            {
                return null;
            }
        }

        public void Restart()
        {
            Application.Restart();
        }
        public UserAccount GetLoggedInUser()
        {
            // OperationContext.Current.OperationCompleted += (sender, e) => _interface.Client.ClientCallback.SendSignal(new SignalMessage(OPERATION_COMPLETED, ""));
            if (CheckRegisteration("GetLoggedInUser"))
            {
                return _interface.LoggedInUser;
            }
            else
            {
                return null;
            }
        }

        public IEnumerable<CommandDescription> GetCommands()
        {
            if (CheckRegisteration("GetCommands"))
            {
                // OperationContext.Current.OperationCompleted += (sender, e) => _interface.Client.ClientCallback.SendSignal(new SignalMessage(OPERATION_COMPLETED, ""));
                List<CommandDescription> rc = new List<CommandDescription>();
                GlobalServices.Logger.Log("Requesting commands list.", LogLevel.Info);
                _interface.Client.ClientCallback.TellMessage("Returning commands list.", LogLevel.Info);
                var _commandSystem = IOCContainer.GetService<ICommandSubsystem<IServerCommandModule>>();
                foreach (KeyValuePair<string, CommandDelegate> currentCommand in _commandSystem.AggregateAllCommandModules())
                {
                    rc.Add(new CommandDescription() { Help = _commandSystem.GetCommandHelp(currentCommand.Value), Behavior = _commandSystem.GetCommandBehavior(currentCommand.Value), HelpPage = _commandSystem.GetHelpPage(currentCommand.Value), CommandName = currentCommand.Key });
                }
                return rc;
            }
            else
            {
                return null;
            }
        }
        public IEnumerable<string> GetCommandsAsStrings()
        {
            // OperationContext.Current.OperationCompleted += (sender, e) => _interface.Client.ClientCallback.SendSignal(new SignalMessage(OPERATION_COMPLETED, ""));
            if (CheckRegisteration("GetCommandsAsStrings"))
            {
                _interface.Client.ClientCallback.SendSignal(new SignalMessage("operation_completed", ""));
                return IOCContainer.GetService<ICommandSubsystem<IServerCommandModule>>().AggregateAllCommandModules().Keys;
            }
            else
            {
                return null;
            }
        }

        public void SwitchUser()
        {
            // OperationContext.Current.OperationCompleted += (sender, e) => _interface.Client.ClientCallback.SendSignal(new SignalMessage(OPERATION_COMPLETED, ""));
            LogOff();
            GlobalServices.Logger.Log("Logging in.", LogLevel.Info ,"Server Host");
            _interface.Client.ClientCallback.TellMessage("Logging in.", LogLevel.Info);
            var cred = _interface.Client.ClientCallback.RequestAuthentication(new AuthenticationRequest(AuthenticationSeverity.Normal) { Reason = "Please provide a username and password to switch to." });
            LogIn(cred);
        }
        private void LogOff()
        {
            _interface.Registered = false;
            string username = _interface.LoggedInUser.Credentials.Username;
            _interface.LoggedInUser = null;
            GlobalServices.Logger.Log($"User {username} logged off.", LogLevel.Info, "Server Host");
            _interface.Client.ClientCallback.TellMessage($"user {username} logged off.", LogLevel.Info);
        }
        private UserAccount LogIn(UserCredentials cred)
        {
            // OperationContext.Current.OperationCompleted += (sender, e) => _interface.Client.ClientCallback.SendSignal(new SignalMessage(OPERATION_COMPLETED, ""));
            if (cred == null)
            {
                GlobalServices.Logger.Log("The user did not pass in any credentials. Authentication failed.", LogLevel.Info);
                _interface.Client.ClientCallback.TellMessage("Can't you at least provide a username and password?", LogLevel.Info);
                _interface.Client.ClientCallback.Disconnect("Authentication failed.");
                return null;
            }
            else
            {
                var l4 = "Authenticating your user credentials.";
                GlobalServices.Logger.Log(l4, LogLevel.Info);
                _interface.Client.ClientCallback.TellMessage(l4, LogLevel.Info);
                var tryUser = ServerManager.AccountManager.AttemptLogin(cred);
                if (tryUser != null)
                {
                    return tryUser;
                }
                else
                {
                    _interface.Client.ClientCallback.TellMessage("Registration failed. Authentication failed.", LogLevel.Info);
                    return null;
                }
            }
        }

        public void Disconnect()
        {
            GlobalServices.Logger.Log($"Client \"{_interface.Client.FriendlyName ?? "\"\""}\" [{_interface.Client.UniqueID}] disconectted.", LogLevel.Info);
        }

        public void EncryptFile(string fileName, string password)
        {
            // OperationContext.Current.OperationCompleted += (sender, e) => _interface.Client.ClientCallback.SendSignal(new SignalMessage(OPERATION_COMPLETED, ""));
            _interface.EncryptFile(fileName, password);
        }

        public void DecryptFile(string fileName, string password)
        {
            // OperationContext.Current.OperationCompleted += (sender, e) => _interface.Client.ClientCallback.SendSignal(new SignalMessage(OPERATION_COMPLETED, ""));
            _interface.DecryptFile(fileName, password);
        }

        public string GetCommandHelpPage(string command)
        {
            // OperationContext.Current.OperationCompleted += (sender, e) => _interface.Client.ClientCallback.SendSignal(new SignalMessage(OPERATION_COMPLETED, ""));
            return IOCContainer.GetService<ICommandSubsystem<IServerCommandModule>>().ShowHelpPage(command);
        }

        public string GetCommandHelpDescription(string command)
        {
            // OperationContext.Current.OperationCompleted += (sender, e) => _interface.Client.ClientCallback.SendSignal(new SignalMessage(OPERATION_COMPLETED, ""));
            return IOCContainer.GetService<ICommandSubsystem<IServerCommandModule>>().ShowCommandHelpDescription(command);
        }
        public IDirectory GetRemoteFiles(string path, bool usingRequest)
        {
            // OperationContext.Current.OperationCompleted += (sender, e) => _interface.Client.ClientCallback.SendSignal(new SignalMessage(OPERATION_COMPLETED, ""));
            DirectoryInfo subDir = new DirectoryInfo(path);
            if (subDir.Parent == null)
            {
                DriveInfo driveInfo = new DriveInfo(subDir.FullName);
                if(driveInfo.IsReady)
                {
                    RemoteDrive drive = new RemoteDrive(driveInfo.Name, driveInfo.VolumeLabel);
                    //Get files
                    foreach (FileInfo files in driveInfo.RootDirectory.EnumerateFiles())
                    {
                        drive.Files.Add(new RemoteFile(files.FullName, files.CreationTime, files.LastAccessTime));
                    }
                    //Get Folders
                    foreach (DirectoryInfo folders in driveInfo.RootDirectory.EnumerateDirectories())
                    {
                        drive.Directories.Add(new RemoteDirectory(folders.FullName, folders.LastAccessTime));
                    }
                    return drive;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                try
                {
                    RemoteDirectory r = new RemoteDirectory(subDir.FullName, subDir.LastAccessTime);
                    //Get files
                    foreach (FileInfo files in subDir.EnumerateFiles())
                    {
                        r.Files.Add(new RemoteFile(files.FullName, files.CreationTime, files.LastAccessTime));
                    }
                    //Get Folders
                    foreach (DirectoryInfo folders in subDir.EnumerateDirectories())
                    {
                        r.Directories.Add(new RemoteDirectory(folders.FullName, folders.LastAccessTime));
                    }
                    return r;
                }
                catch (UnauthorizedAccessException ex)
                {
                    throw new FaultException<ServerFault>(new ServerFault(ex.StackTrace, ServerManager.DefaultExtensionLibraryLoader.GetAllLibraries().Select(l => l.FriendlyName).ToList()), ex.Message);
                }
            }
        }

        public object ExecuteScript(string script)
        {
            try
            {
                GlobalServices.Logger.Log("Running script file.", LogLevel.Info, "Server Host");
                return IOCContainer.GetService<IScriptingEngine>().ExecuteString<object>(script);
            }
            catch (Exception ex)
            {
                _interface.Client.ClientCallback.TellMessageToServerConsole($"Could not execute script file: {ex.Message}", LogLevel.Error, "Scripting Engine");
                return false;
            }
        }

        public string ReadFileAsString(string fileName)
        {
            return File.ReadAllText(fileName);
        }

        public void ServerRegistered(Guid serverGuid)
        {
            ServerManager.ServerGuid = serverGuid;
        }

        public void SendSignal(SignalMessage signal)
        {
            
        }
        public void UploadBytesToResource(byte[] data, int length, string friendlyName, string name, string path)
        {
            MemoryResource fp = new MemoryResource(Path.GetFileName(name), Path.GetFileName(name), data);
            IOCContainer.GetService<IResourceManager>().AddResource(path, fp);
        }

        public Resource GetResource(string resourceIdentifier)
        {
            throw new NotImplementedException();
        }

        public void PublishEvent(ITinyMessage message)
        {
            GlobalServices.EventBus.PublishPrivate(message);
        }

        public bool HasKnownType(string name)
        {
            return DefaultKnownTypeManager.HasName(name);
        }

        public Guid GetSelectedServerGuid()
        {
            return _interface.GetSelectedServerGuid();
        }

        public Task<CommandPipeline> RunServerCommandAsync(string command, CommandExecutionMode commandMode)
        {
            return _interface.RunServerCommandAsync(command, commandMode);
        }

        public void CancelServerCommand()
        {
            _interface.CancelServerCommand();
        }
    }
}