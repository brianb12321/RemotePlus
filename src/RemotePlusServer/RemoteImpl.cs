using System;
using System.Collections.Generic;
using System.ServiceModel;
using RemotePlusLibrary;
using System.Windows.Forms;
using System.Speech.Synthesis;
using RemotePlusLibrary.Core;
using RemotePlusLibrary.Extension.CommandSystem;
using System.IO;
using RemotePlusLibrary.Extension.CommandSystem.CommandClasses;
using RemotePlusServer.Internal;
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
using RemotePlusServer.Core.ExtensionSystem;
using BetterLogger;

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
            var l = $"Checking registiration for action {Action}.";
            ServerManager.Logger.Log(l, LogLevel.Info);
            if (_interface.Registered != true)
            {
                ServerManager.Logger.Log("The client is not registired to the server.", LogLevel.Error);
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
                if (_interface.LoggedInUser.Role.AttachedPolicyObject.Policies.FindSubFolder("Operations").Policies.First(p => p.ShortName == "EnableConsole").Values["Value"] != "True")
                {
                    _interface.Client.ClientCallback.TellMessage("You do not have promission to use the beep function.", LogLevel.Info);
                }
                else
                {
                    _interface.Beep(Hertz, Duration);
                    _interface.Client.ClientCallback.TellMessage($"Console beeped. Hertz: {Hertz}, Duration: {Duration}", LogLevel.Info);
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
                    _interface.Client.ClientCallback.TellMessage("You do not have promission to use the PlaySound function.", LogLevel.Info);
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
                    _interface.Client.ClientCallback.TellMessage("You do not have promission to use the CanPlaySoundLoop function.", LogLevel.Info);
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
                    _interface.Client.ClientCallback.TellMessage("You do not have promission to use the CanPlaySoundSync function.", LogLevel.Info);
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
            ServerManager.Logger.Log("A new client is awaiting registiration.", LogLevel.Info);
            ServerManager.Logger.Log("Instanitiating callback object.", LogLevel.Debug);
            ServerManager.Logger.Log("Getting ClientBuilder from client.", LogLevel.Debug);

            if (ServerManager.DefaultSettings.DiscoverySettings.DiscoveryBehavior == RemotePlusLibrary.Configuration.ServerSettings.ProxyConnectionMode.Connect)
            {
                _interface.Client = Client<RemoteClient>.Build(ServerStartup.proxyChannel.RegisterClient(), new RemoteClient(null, true, ServerStartup.proxyChannel));
            }
            else
            {
                var callback = OperationContext.Current.GetCallbackChannel<IRemoteClient>();
                _interface.Client = Client<RemoteClient>.Build(callback.RegisterClient(), new RemoteClient(callback, false, null));
            }
            ServerManager.Logger.Log("Received registiration object from client.", LogLevel.Info);
            this._interface.Settings = Settings;
            var l = "Processing registiration object.";
            ServerManager.Logger.Log(l, LogLevel.Debug);
            _interface.Client.ClientCallback.TellMessage(l, LogLevel.Debug);
            if (Settings.LoginRightAway)
            {
                var account = LogIn(Settings.Credentials);
                if (account == null)
                {
                    ServerManager.Logger.Log($"Client {_interface.Client.FriendlyName} [{_interface.Client.UniqueID.ToString()}] disconnected. Failed to register to the server. Authentication failed.", LogLevel.Info);
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
                var l3 = "Awaiting credentials from the client.";
                ServerManager.Logger.Log(l3, LogLevel.Info);
                _interface.Client.ClientCallback.TellMessage(l3, LogLevel.Info);
                UserCredentials upp = _interface.Client.ClientCallback.RequestAuthentication(new AuthenticationRequest(AuthenticationSeverity.Normal) { Reason = "The server requires credentials to register." });
                var account = LogIn(upp);
                if (account == null)
                {
                    ServerManager.Logger.Log($"Client {_interface.Client.FriendlyName} [{_interface.Client.UniqueID.ToString()}] disconnected. Failed to register to the server. Authentication failed.", LogLevel.Info);
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
            ServerManager.Logger.Log($"Client \"{_interface.Client.FriendlyName}\" [{_interface.Client.UniqueID}] Type: {_interface.Client.ClientType} registired.", LogLevel.Info);
            _interface.Registered = true;
            _interface.Client.ClientCallback.TellMessage("Registiration complete.",LogLevel.Info);
            _interface.Client.ClientCallback.RegistirationComplete();
        }

        public void RunProgram(string Program, string Argument)
        {
            if (CheckRegisteration("RunProgram"))
            {
                if (_interface.LoggedInUser.Role.AttachedPolicyObject.Policies.FindSubFolder("Operations").Policies.First(p => p.ShortName == "EnableConsole").Values["Value"] != "True")
                {
                    _interface.Client.ClientCallback.TellMessage("You do not have promission to use the CanRunProgram function.", LogLevel.Info);
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
                    _interface.Client.ClientCallback.TellMessage("You do not have promission to use the CanShowMessageBox function.", LogLevel.Info);
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
                    _interface.Client.ClientCallback.TellMessage("You do not have promission to use the Speak function.", LogLevel.Info);
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
                    _interface.Client.ClientCallback.TellMessage("You do not have promission to use the Console function.", LogLevel.Info);
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
                    _interface.Client.ClientCallback.TellMessage("You do not have permission to change server settings.", LogLevel.Error);
                }
                else
                {
                    ServerManager.Logger.Log("Updating server settings.", LogLevel.Info);
                    ServerManager.DefaultSettings = Settings;
                    _interface.Client.ClientCallback.TellMessage("Saving settings.", LogLevel.Info);
                    ServerManager.DefaultSettings.Save();
                    _interface.Client.ClientCallback.TellMessage("Settings saved.", LogLevel.Info);
                    ServerManager.Logger.Log("Settings saved.", LogLevel.Info);
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
                    _interface.Client.ClientCallback.TellMessage("You do not have permission to change server settings.", LogLevel.Error);
                    return null;
                }
                else
                {
                    ServerManager.Logger.Log("Retreiving server settings.", LogLevel.Info);
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
                ServerManager.Logger.Log("Requesting commands list.", LogLevel.Info);
                _interface.Client.ClientCallback.TellMessage("Returning commands list.", LogLevel.Info);
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
            ServerManager.Logger.Log("Logging in.", LogLevel.Info ,"Server Host");
            _interface.Client.ClientCallback.TellMessage("Logging in.", LogLevel.Info);
            var cred = _interface.Client.ClientCallback.RequestAuthentication(new AuthenticationRequest(AuthenticationSeverity.Normal) { Reason = "Please provide a username and password to switch to." });
            LogIn(cred);
        }
        private void LogOff()
        {
            _interface.Registered = false;
            string username = _interface.LoggedInUser.Credentials.Username;
            _interface.LoggedInUser = null;
            ServerManager.Logger.Log($"User {username} logged off.", LogLevel.Info, "Server Host");
            _interface.Client.ClientCallback.TellMessage($"user {username} logged off.", LogLevel.Info);
        }
        private UserAccount LogIn(UserCredentials cred)
        {
            // OperationContext.Current.OperationCompleted += (sender, e) => _interface.Client.ClientCallback.SendSignal(new SignalMessage(OPERATION_COMPLETED, ""));
            if (cred == null)
            {
                ServerManager.Logger.Log("The user did not pass in any credentials. Authentication failed.", LogLevel.Info);
                _interface.Client.ClientCallback.TellMessage("Can't you at least provide a username and password?", LogLevel.Info);
                _interface.Client.ClientCallback.Disconnect("Authentication failed.");
                return null;
            }
            else
            {
                var l4 = "Authenticating your user credentials.";
                ServerManager.Logger.Log(l4, LogLevel.Info);
                _interface.Client.ClientCallback.TellMessage(l4, LogLevel.Info);
                var tryUser = AccountManager.AttemptLogin(cred);
                if (tryUser != null)
                {
                    return tryUser;
                }
                else
                {
                    _interface.Client.ClientCallback.TellMessage("Registiration failed. Authentication failed.", LogLevel.Info);
                    return null;
                }
            }
        }

        public void Disconnect()
        {
            ServerManager.Logger.Log($"Client \"{_interface.Client.FriendlyName ?? "\"\""}\" [{_interface.Client.UniqueID}] disconectted.", LogLevel.Info);
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
                ServerManager.Logger.Log("Running script file.", LogLevel.Info, "Server Host");
                bool success = ServerManager.ScriptBuilder.ExecuteString(script);
                return success;
            }
            catch (Exception ex)
            {
                _interface.Client.ClientCallback.TellMessageToServerConsole($"Could not execute script file: {ex.Message}", LogLevel.Error, ScriptBuilder.SCRIPT_LOG_CONSTANT);
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