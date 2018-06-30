using System;
using System.Collections.Generic;
using System.ServiceModel;
using RemotePlusLibrary;
using System.Windows.Forms;
using Logging;
using System.Speech.Synthesis;
using RemotePlusLibrary.Core;
using System.Diagnostics;
using RemotePlusLibrary.Extension.CommandSystem;
using System.IO;
using RemotePlusLibrary.Extension.CommandSystem.CommandClasses;
using System.Text.RegularExpressions;
using RemotePlusServer.Internal;
using System.Linq;
using RemotePlusLibrary.Scripting;
using RemotePlusLibrary.Security.AccountSystem;
using RemotePlusLibrary.Discovery;
using RemotePlusLibrary.Core.Faults;
using RemotePlusLibrary.FileTransfer.BrowserClasses;
using RemotePlusLibrary.Contracts;
using RemotePlusLibrary.Security.Authentication;
using RemotePlusLibrary.Extension.CommandSystem.CommandClasses.Parsing;
using RemotePlusLibrary.Configuration.ServerSettings;
using RemotePlusLibrary.Client;
using RemotePlusServer.Core;
using RemotePlusServer.Core.ExtensionSystem;

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
    [GlobalException(typeof(GlobalErrorHandler))]
    public class RemoteImpl : IRemote, IRemoteWithProxy
    {
        const string OPERATION_COMPLETED = "Operation_Completed";
        private ServerRemoteInterface _interface = null;
        public RemoteImpl()
        {
            _interface = new ServerRemoteInterface();
        }
        public void SetRemoteInterface(ServerRemoteInterface i)
        {
            _interface = i;
        }
        internal void Setup()
        {

        }
        bool CheckRegisteration(string Action)
        {
            var l = ServerManager.Logger.AddOutput($"Checking registiration for action {Action}.", OutputLevel.Info);
            if (_interface.Registered != true)
            {
                ServerManager.Logger.AddOutput("The client is not registired to the server.", OutputLevel.Error);
                if (ServerStartup.proxyChannelFactory.State == CommunicationState.Opened)
                {
                    ServerStartup.proxyChannel.TellMessage(Guid.NewGuid(), new UILogItem(OutputLevel.Error, "You must be registered.", "Server Host"));
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
                if (_interface.LoggedInUser.Role.AttachedPolicyObject.Policies.FindSubFolder("Operations").Policies.First(p => p.ShortName == "EnableConsole").Values["Value"] != "True")
                {
                    _interface.Client.ClientCallback.TellMessage("You do not have promission to use the beep function.", OutputLevel.Info);
                }
                else
                {
                    _interface.Beep(Hertz, Duration);
                    _interface.Client.ClientCallback.TellMessage($"Console beeped. Hertz: {Hertz}, Duration: {Duration}", OutputLevel.Info);
                }
            }
            // OperationContext.Current.OperationCompleted += (sender, e) => _interface.Client.ClientCallback.SendSignal(new SignalMessage(OPERATION_COMPLETED, ""));
        }

        public void PlaySound(string FileName)
        {
            if(CheckRegisteration("PlaySound"))
            {
                if (_interface.LoggedInUser.Role.AttachedPolicyObject.Policies.FindSubFolder("Operations").Policies.First(p => p.ShortName == "EnableConsole").Values["Value"] != "True")
                {
                    _interface.Client.ClientCallback.TellMessage("You do not have promission to use the PlaySound function.", OutputLevel.Info);
                }
                else
                {
                    System.Media.SoundPlayer sp = new System.Media.SoundPlayer(FileName);
                    sp.Play();
                }
            }
        }

        public void PlaySoundLoop(string FileName)
        {
            if (CheckRegisteration("PlaySoundLoop"))
            {
                if (_interface.LoggedInUser.Role.AttachedPolicyObject.Policies.FindSubFolder("Operations").Policies.First(p => p.ShortName == "EnableConsole").Values["Value"] != "True")
                {
                    _interface.Client.ClientCallback.TellMessage("You do not have promission to use the CanPlaySoundLoop function.", OutputLevel.Info);
                }
                else
                {
                    System.Media.SoundPlayer sp = new System.Media.SoundPlayer(FileName);
                    sp.PlayLooping();
                }
            }
            // OperationContext.Current.OperationCompleted += (sender, e) => _interface.Client.ClientCallback.SendSignal(new SignalMessage(OPERATION_COMPLETED, ""));
        }

        public void PlaySoundSync(string FileName)
        {
            if (CheckRegisteration("PlaySoundSync"))
            {
                if (_interface.LoggedInUser.Role.AttachedPolicyObject.Policies.FindSubFolder("Operations").Policies.First(p => p.ShortName == "EnableConsole").Values["Value"] != "True")
                {
                    _interface.Client.ClientCallback.TellMessage("You do not have promission to use the CanPlaySoundSync function.", OutputLevel.Info);
                }
                else
                {
                    System.Media.SoundPlayer sp = new System.Media.SoundPlayer(FileName);
                    sp.PlaySync();
                }
            }
            // OperationContext.Current.OperationCompleted += (sender, e) => _interface.Client.ClientCallback.SendSignal(new SignalMessage(OPERATION_COMPLETED, ""));
        }

        public void Register(RegisterationObject Settings)
        {
            const string REG_FAILED = "Registiration failed. The most likely cause is invalid credentials or this account has been blocked. Make sure that the provided credentials are correct and also make sure the account was not blocked. If you still receive this error message, please check the server logs for more details.";
            ServerManager.Logger.AddOutput("A new client is awaiting registiration.", OutputLevel.Info);
            ServerManager.Logger.AddOutput("Instanitiating callback object.", OutputLevel.Debug);
            ServerManager.Logger.AddOutput("Getting ClientBuilder from client.", OutputLevel.Debug);

            if (ServerManager.DefaultSettings.DiscoverySettings.DiscoveryBehavior == RemotePlusLibrary.Configuration.ServerSettings.ProxyConnectionMode.Connect)
            {
                _interface.Client = Client<RemoteClient>.Build(ServerStartup.proxyChannel.RegisterClient(), new RemoteClient(null, true, ServerStartup.proxyChannel));
            }
            else
            {
                var callback = OperationContext.Current.GetCallbackChannel<IRemoteClient>();
                _interface.Client = Client<RemoteClient>.Build(callback.RegisterClient(), new RemoteClient(callback, false, null));
            }
            ServerManager.Logger.AddOutput("Received registiration object from client.", OutputLevel.Info);
            this._interface.Settings = Settings;
            var l = ServerManager.Logger.AddOutput("Processing registiration object.", OutputLevel.Debug);
            _interface.Client.ClientCallback.TellMessage(new UILogItem(l.Level, l.Message, l.From));
            if (Settings.LoginRightAway)
            {
                var account = LogIn(Settings.Credentials);
                if (account == null)
                {
                    ServerManager.Logger.AddOutput($"_interface.Client {_interface.Client.FriendlyName} [{_interface.Client.UniqueID.ToString()}] disconnected. Failed to register to the server. Authentication failed.", OutputLevel.Info);
                    throw new FaultException(REG_FAILED + $" Provded username: {Settings.Credentials.Username}");
                }
                else
                {
                    _interface.LoggedInUser = account;
                    UpdateAccountPolicy();
                    RegisterComplete();
                }
            }
            else
            {
                var l3 = ServerManager.Logger.AddOutput("Awaiting credentials from the client.", OutputLevel.Info);
                _interface.Client.ClientCallback.TellMessage(new UILogItem(l3.Level, l3.Message, l3.From));
                UserCredentials upp = _interface.Client.ClientCallback.RequestAuthentication(new AuthenticationRequest(AuthenticationSeverity.Normal) { Reason = "The server requires credentials to register." });
                var account = LogIn(upp);
                if (account == null)
                {
                    ServerManager.Logger.AddOutput($"_interface.Client {_interface.Client.FriendlyName} [{_interface.Client.UniqueID.ToString()}] disconnected. Failed to register to the server. Authentication failed.", OutputLevel.Info);
                    throw new FaultException(REG_FAILED + $" Provded username: {upp.Username}");
                }
                else
                {
                    _interface.LoggedInUser = account;
                    UpdateAccountPolicy();
                    RegisterComplete();
                }
            }
            _HookManager.RunHooks(ServerLibraryBuilder.LOGIN_HOOK, new RemotePlusLibrary.Extension.HookSystem.HookArguments(ServerLibraryBuilder.LOGIN_HOOK));
            if (_interface.Client.ClientType == ClientType.CommandLine && ServerStartup.proxyChannelFactory == null)
            {
                _interface.Client.ClientCallback.ChangePrompt(new RemotePlusLibrary.Extension.CommandSystem.PromptBuilder()
                {
                    Path = _interface.CurrentPath,
                    AdditionalData = "Current Path",
                    CurrentUser = _interface.LoggedInUser.Credentials.Username
                });
            }
            // OperationContext.Current.OperationCompleted += (sender, e) => _interface.Client.ClientCallback.SendSignal(new SignalMessage(OPERATION_COMPLETED, ""));
        }

        private void UpdateAccountPolicy()
        {
            _interface.LoggedInUser.Role.BuildPolicyObject();
        }

        private void RegisterComplete()
        {
            ServerManager.Logger.AddOutput($"_interface.Client \"{_interface.Client.FriendlyName}\" [{_interface.Client.UniqueID}] Type: {_interface.Client.ClientType} registired.", Logging.OutputLevel.Info);
            _interface.Registered = true;
            _interface.Client.ClientCallback.TellMessage("Registiration complete.", Logging.OutputLevel.Info);
            _interface.Client.ClientCallback.RegistirationComplete();
        }

        public void RunProgram(string Program, string Argument)
        {
            if (CheckRegisteration("RunProgram"))
            {
                if (_interface.LoggedInUser.Role.AttachedPolicyObject.Policies.FindSubFolder("Operations").Policies.First(p => p.ShortName == "EnableConsole").Values["Value"] != "True")
                {
                    _interface.Client.ClientCallback.TellMessage("You do not have promission to use the CanRunProgram function.", OutputLevel.Info);
                }
                else
                {
                    
                }
            }
            // OperationContext.Current.OperationCompleted += (sender, e) => _interface.Client.ClientCallback.SendSignal(new SignalMessage(OPERATION_COMPLETED, ""));
        }

        public DialogResult ShowMessageBox(string Message, string Caption, System.Windows.Forms.MessageBoxIcon Icon, System.Windows.Forms.MessageBoxButtons Buttons)
        {
            if (CheckRegisteration("ShowMessageBox"))
            {
                if (_interface.LoggedInUser.Role.AttachedPolicyObject.Policies.FindSubFolder("Operations").Policies.First(p => p.ShortName == "EnableConsole").Values["Value"] != "True")
                {
                    _interface.Client.ClientCallback.TellMessage("You do not have promission to use the CanShowMessageBox function.", OutputLevel.Info);
                    return DialogResult.Abort;
                }
                else
                {
                    return _interface.ShowMessageBox(Message, Caption, Icon, Buttons);
                }
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
                if (_interface.LoggedInUser.Role.AttachedPolicyObject.Policies.FindSubFolder("Operations").Policies.First(p => p.ShortName == "EnableConsole").Values["Value"] != "True")
                {
                    _interface.Client.ClientCallback.TellMessage("You do not have promission to use the Speak function.", OutputLevel.Info);
                }
                else
                {
                    _interface.Speak(Message, Gender, Age);
                }
            }
            // OperationContext.Current.OperationCompleted += (sender, e) => _interface.Client.ClientCallback.SendSignal(new SignalMessage(OPERATION_COMPLETED, ""));
        }

        public CommandPipeline RunServerCommand(string Command, CommandExecutionMode commandMode)
        {
            if (CheckRegisteration("RunServerCommand"))
            {
                if (_interface.LoggedInUser.Role.AttachedPolicyObject.Policies.FindSubFolder("Operations").Policies.First(p => p.ShortName == "EnableConsole").Values["Value"] != "True")
                {
                    _interface.Client.ClientCallback.TellMessage("You do not have promission to use the Console function.", OutputLevel.Info);
                    return null;
                }
                else
                {
                    return _interface.RunServerCommand(Command, commandMode);
                }
                // OperationContext.Current.OperationCompleted += (sender, e) => _interface.Client.ClientCallback.SendSignal(new SignalMessage(OPERATION_COMPLETED, ""));
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
                if (_interface.LoggedInUser.Role.AttachedPolicyObject.Policies.FindSubFolder("Operations").Policies.First(p => p.ShortName == "EnableConsole").Values["Value"] != "True")
                {
                    _interface.Client.ClientCallback.TellMessage(new UILogItem(OutputLevel.Error, "You do not have permission to change server settings.", "Server Host"));
                }
                else
                {
                    ServerManager.Logger.AddOutput("Updating server settings.", OutputLevel.Info);
                    ServerManager.DefaultSettings = Settings;
                    _interface.Client.ClientCallback.TellMessage("Saving settings.", OutputLevel.Info);
                    ServerManager.DefaultSettings.Save();
                    _interface.Client.ClientCallback.TellMessage("Settings saved.", OutputLevel.Info);
                    ServerManager.Logger.AddOutput("Settings saved.", OutputLevel.Info);
                }
            }
            // OperationContext.Current.OperationCompleted += (sender, e) => _interface.Client.ClientCallback.SendSignal(new SignalMessage(OPERATION_COMPLETED, ""));
        }

        public ServerSettings GetServerSettings()
        {
            // OperationContext.Current.OperationCompleted += (sender, e) => _interface.Client.ClientCallback.SendSignal(new SignalMessage(OPERATION_COMPLETED, ""));
            if (CheckRegisteration("GetServerSettings"))
            {
                if (_interface.LoggedInUser.Role.AttachedPolicyObject.Policies.FindSubFolder("Operations").Policies.First(p => p.ShortName == "GetServerSettings").Values["Value"] != "True")
                {
                    _interface.Client.ClientCallback.TellMessage(new UILogItem(OutputLevel.Error, "You do not have permission to change server settings.", "Server Host"));
                    return null;
                }
                else
                {
                    ServerManager.Logger.AddOutput("Retreiving server settings.", OutputLevel.Info);
                    return ServerManager.DefaultSettings;
                }
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
                ServerManager.Logger.AddOutput("Requesting commands list.", OutputLevel.Info);
                _interface.Client.ClientCallback.TellMessage("Returning commands list.", OutputLevel.Info);
                foreach (KeyValuePair<string, CommandDelegate> currentCommand in ServerManager.ServerRemoteService.Commands)
                {
                    rc.Add(new CommandDescription() { Help = RemotePlusConsole.GetCommandHelp(currentCommand.Value), Behavior = RemotePlusConsole.GetCommandBehavior(currentCommand.Value), HelpPage = RemotePlusConsole.GetCommandHelpPage(currentCommand.Value), CommandName = currentCommand.Key });
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
                return ServerManager.ServerRemoteService.Commands.Keys;
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
            ServerManager.Logger.AddOutput("Logging in.", OutputLevel.Info ,"Server Host");
            _interface.Client.ClientCallback.TellMessage(new UILogItem(OutputLevel.Info, "Logging in.", "Server Host"));
            var cred = _interface.Client.ClientCallback.RequestAuthentication(new AuthenticationRequest(AuthenticationSeverity.Normal) { Reason = "Please provide a username and password to switch to." });
            LogIn(cred);
        }
        private void LogOff()
        {
            _interface.Registered = false;
            string username = _interface.LoggedInUser.Credentials.Username;
            _interface.LoggedInUser = null;
            ServerManager.Logger.AddOutput($"User {username} logged off.", OutputLevel.Info, "Server Host");
            _interface.Client.ClientCallback.TellMessage(new UILogItem(OutputLevel.Info, $"user {username} logged off.", "Server Host"));
        }
        private UserAccount LogIn(UserCredentials cred)
        {
            // OperationContext.Current.OperationCompleted += (sender, e) => _interface.Client.ClientCallback.SendSignal(new SignalMessage(OPERATION_COMPLETED, ""));
            if (cred == null)
            {
                ServerManager.Logger.AddOutput("The user did not pass in any credentials. Authentication failed.", OutputLevel.Info);
                _interface.Client.ClientCallback.TellMessage("Can't you at least provide a username and password?", OutputLevel.Info);
                _interface.Client.ClientCallback.Disconnect("Authentication failed.");
                return null;
            }
            else
            {
                var l4 = ServerManager.Logger.AddOutput("Authenticating your user credentials.", OutputLevel.Info);
                _interface.Client.ClientCallback.TellMessage(new UILogItem(l4.Level, l4.Message, l4.From));
                var tryUser = AccountManager.AttemptLogin(cred);
                if (tryUser != null)
                {
                    return tryUser;
                }
                else
                {
                    _interface.Client.ClientCallback.TellMessage("Registiration failed. Authentication failed.", OutputLevel.Info);
                    return null;
                }
            }
        }

        public void Disconnect()
        {
            ServerManager.Logger.AddOutput($"_interface.Client \"{_interface.Client.FriendlyName ?? "\"\""}\" [{_interface.Client.UniqueID}] disconectted.", OutputLevel.Info);
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
            return RemotePlusConsole.ShowHelpPage(ServerManager.ServerRemoteService.Commands, command);
        }

        public string GetCommandHelpDescription(string command)
        {
            // OperationContext.Current.OperationCompleted += (sender, e) => _interface.Client.ClientCallback.SendSignal(new SignalMessage(OPERATION_COMPLETED, ""));
            return RemotePlusConsole.ShowCommandHelpDescription(ServerManager.ServerRemoteService.Commands, command);
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
                    throw new FaultException<ServerFault>(new ServerFault(), ex.Message);
                }
            }
        }

        public bool ExecuteScript(string script)
        {
            try
            {
                ServerManager.Logger.AddOutput("Running script file.", OutputLevel.Info, "Server Host");
                bool success = ServerManager.ScriptBuilder.ExecuteString(script);
                return success;
            }
            catch (Exception ex)
            {
                _interface.Client.ClientCallback.TellMessageToServerConsole(new UILogItem(OutputLevel.Error, $"Could not execute script file: {ex.Message}", ScriptBuilder.SCRIPT_LOG_CONSTANT));
                return false;
            }
        }

        public ScriptGlobalInformation[] GetScriptGlobals()
        {
            var list = ServerManager.ScriptBuilder.GetGlobals();
            return list.Select(l => l.Information).ToArray();
        }

        public string ReadFileAsString(string fileName)
        {
            return File.ReadAllText(fileName);
        }

        public List<string> GetServerRoleNames()
        {
            List<string> l = new List<string>();
            foreach(Role r in Role.GlobalPool.Roles)
            {
                l.Add(r.RoleName);
            }
            return l;
        }

        public void ServerRegistered(Guid serverGuid)
        {
            ServerManager.ServerGuid = serverGuid;
        }
    }
}