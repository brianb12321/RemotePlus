using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using RemotePlusLibrary;
using System.Windows.Forms;
using Logging;
using System.Speech.Synthesis;
using RemotePlusLibrary.Core;
using RemotePlusLibrary.Extension;
using System.Diagnostics;

namespace RemotePlusServer
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant,
        InstanceContextMode = InstanceContextMode.Single)]
    [GlobalException(typeof(GlobalErrorHandler))]
    public class RemoteImpl : IRemote
    {
        public RemoteImpl()
        {

        }
        internal void Setup()
        {
            ServerManager.Logger.AddOutput("Added temperary extensions into dictionary.", OutputLevel.Diagnostics);
            _allExtensions = ServerManager.DefaultCollection.GetAllExtensions();
        }
        public RegistirationObject Settings { get; private set; }
        public Client Client { get; set; }
        public bool Registered { get; private set; }
        public UserAccount LoggedInUser { get; private set; }
        private Dictionary<string, ServerExtension> _allExtensions;
        void CheckRegisteration()
        {
            var l = ServerManager.Logger.AddOutput("Checking registiration.", OutputLevel.Info);
            Client.ClientCallback.TellMessage(new UILogItem(l.Level, l.Message, l.From));
            if (!Registered)
            {
                ServerManager.Logger.AddOutput("The client is not registired to the server.", OutputLevel.Error);
                OperationContext.Current.GetCallbackChannel<IRemoteClient>().Disconnect("you must be registered.");
            }
        }
        public void Beep(int Hertz, int Duration)
        {
            CheckRegisteration();
            if (!LoggedInUser.Role.Privilleges.CanBeep)
            {
                Client.ClientCallback.TellMessage("You do not have promission to use the beep function.", OutputLevel.Info);
            }
            else
            {
                Console.Beep(Hertz, Duration);
                Client.ClientCallback.TellMessage($"Console beeped. Hertz: {Hertz}, Duration: {Duration}", OutputLevel.Info);
            }
        }

        public void PlaySound(string FileName)
        {
            CheckRegisteration();
            if (!LoggedInUser.Role.Privilleges.CanPlaySound)
            {
                Client.ClientCallback.TellMessage("You do not have promission to use the PlaySound function.", OutputLevel.Info);
            }
            else
            {
                System.Media.SoundPlayer sp = new System.Media.SoundPlayer(FileName);
                sp.Play();
            }
        }

        public void PlaySoundLoop(string FileName)
        {
            CheckRegisteration();
            if (!LoggedInUser.Role.Privilleges.CanPlaySoundLoop)
            {
                Client.ClientCallback.TellMessage("You do not have promission to use the CanPlaySoundLoop function.", OutputLevel.Info);
            }
            else
            {
                System.Media.SoundPlayer sp = new System.Media.SoundPlayer(FileName);
                sp.PlayLooping();
            }
        }

        public void PlaySoundSync(string FileName)
        {
            CheckRegisteration();
            if (!LoggedInUser.Role.Privilleges.CanPlaySoundSync)
            {
                Client.ClientCallback.TellMessage("You do not have promission to use the CanPlaySoundSync function.", OutputLevel.Info);
            }
            else
            {
                System.Media.SoundPlayer sp = new System.Media.SoundPlayer(FileName);
                sp.PlaySync();
            }
        }

        public void Register(RegistirationObject Settings)
        {
            ServerManager.Logger.AddOutput("Instanitiating callback object.", OutputLevel.Diagnostics);
            var callback = OperationContext.Current.GetCallbackChannel<IRemoteClient>();
            ServerManager.Logger.AddOutput("Getting ClientBuilder from client.", OutputLevel.Diagnostics);
            Client = callback.RegisterClient().Build(callback);
            ServerManager.Logger.AddOutput("Received registiration object from client.", OutputLevel.Info);
            this.Settings = Settings;
            var l = ServerManager.Logger.AddOutput("Processing registiration object.", OutputLevel.Diagnostics);
            Client.ClientCallback.TellMessage(new UILogItem(l.Level, l.Message, l.From));
            if (Settings.LoginRightAway)
            {
                var l2 = ServerManager.Logger.AddOutput("Authenticating your user credentials.", OutputLevel.Info);
                Client.ClientCallback.TellMessage(new UILogItem(l.Level, l.Message, l.From));
                foreach (UserAccount Account in ServerManager.DefaultSettings.Accounts)
                {
                    if (Account.Verify(Settings.Credentials))
                    {
                        LoggedInUser = Account;
                        RegisterComplete();
                        break;
                    }
                }
            }
            else
            {
                var l3 = ServerManager.Logger.AddOutput("Awaiting credentials from the client.", OutputLevel.Info);
                Client.ClientCallback.TellMessage(new UILogItem(l3.Level, l3.Message, l3.From));
                UserCredentials upp = Client.ClientCallback.RequestAuthentication(new AuthenticationRequest() { Reason = "The server requires credentials to register."});
                if (upp == null)
                {
                    Client.ClientCallback.TellMessage("Can't you at least provide a username and password?", OutputLevel.Info);
                    Client.ClientCallback.Disconnect("Authentication failed.");
                }
                var l4 = ServerManager.Logger.AddOutput("Authenticating your user credentials.", OutputLevel.Info);
                Client.ClientCallback.TellMessage(new UILogItem(l4.Level, l4.Message, l4.From));
                foreach (UserAccount Account in ServerManager.DefaultSettings.Accounts)
                {
                    if (Account.Verify(upp))
                    {
                        LoggedInUser = Account;
                        RegisterComplete();
                        break;
                    }
                }
            }
            if (Registered != true)
            {
                Client.ClientCallback.TellMessage("Registiration failed. Authentication failed.", OutputLevel.Info);
                Client.ClientCallback.Disconnect("Registiration failed.");
            }
        }

        private void RegisterComplete()
        {
            ServerManager.Logger.AddOutput($"Client \"{Client.FriendlyName}\" registired.", Logging.OutputLevel.Info);
            Registered = true;
            Client.ClientCallback.TellMessage("Registiration complete.", Logging.OutputLevel.Info);
        }

        public void RunProgram(string Program, string Argument)
        {
            CheckRegisteration();
            if (!LoggedInUser.Role.Privilleges.CanRunProgram)
            {
                Client.ClientCallback.TellMessage("You do not have promission to use the CanRunProgram function.", OutputLevel.Info);
            }
            Process p = new Process();
            p.StartInfo.FileName = Program;
            p.StartInfo.Arguments = Argument;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.ErrorDataReceived += (sender, e) =>
            {
                if (e.Data != null)
                {
                    if (Client.ExtraData.TryGetValue("ps_appendNewLine", out string val))
                    {
                        if (val == "true")
                        {
                            Client.ClientCallback.TellMessageToServerConsole(e.Data);
                        }
                        else
                        {
                            Client.ClientCallback.TellMessageToServerConsole(new UILogItem(OutputLevel.Error, $"Extra data for appendText is invalid. Value: {val}", "Server Host"));
                        }
                    }
                    else
                    {
                        Client.ClientCallback.TellMessageToServerConsole(e.Data + "\n");
                    }
                }
            };
            p.OutputDataReceived += (sender, e) =>
            {
                if (e.Data != null)
                {
                    if (Client.ExtraData.TryGetValue("ps_appendNewLine", out string val))
                    {
                        if (val == "true")
                        {
                            Client.ClientCallback.TellMessageToServerConsole(e.Data);
                        }
                        else
                        {
                            Client.ClientCallback.TellMessageToServerConsole(new UILogItem(OutputLevel.Error, $"Extra data for appendText is invalid. Value: {val}", "Server Host"));
                        }
                    }
                    else
                    {
                        Client.ClientCallback.TellMessageToServerConsole(e.Data + "\n");
                    }
                }
            };
            p.Start();
            p.BeginErrorReadLine();
            p.BeginOutputReadLine();
        }

        public void ShowMessageBox(string Message, string Caption, System.Windows.Forms.MessageBoxIcon Icon, System.Windows.Forms.MessageBoxButtons Buttons)
        {
            CheckRegisteration();
            if (!LoggedInUser.Role.Privilleges.CanShowMessageBox)
            {
                Client.ClientCallback.TellMessage("You do not have promission to use the CanShowMessageBox function.", OutputLevel.Info);
            }
            else
            {
                MessageBox.Show(Message, Caption, Buttons, Icon);
            }
        }

        public void Speak(string Message, System.Speech.Synthesis.VoiceGender Gender, VoiceAge Age)
        {
            CheckRegisteration();
            if (!LoggedInUser.Role.Privilleges.CanSpeak)
            {
                Client.ClientCallback.TellMessage("You do not have promission to use the Speak function.", OutputLevel.Info);
            }
            else
            {
                System.Speech.Synthesis.SpeechSynthesizer ss = new System.Speech.Synthesis.SpeechSynthesizer();
                ss.SelectVoiceByHints(Gender, Age);
                ss.Speak(Message);
                Client.ClientCallback.TellMessage($"Server spoke. Message: {Message}, gender: {Gender.ToString()}, age: {Age.ToString()}", OutputLevel.Info);
            }
        }

        public int RunServerCommand(string Command)
        {
            CheckRegisteration();
            if (!LoggedInUser.Role.Privilleges.CanAccessConsole)
            {
                Client.ClientCallback.TellMessage("You do not have promission to use the Console function.", OutputLevel.Info);
            }
            return ServerManager.Execute(Command);
        }

        public void UpdateServerSettings(ServerSettings Settings)
        {
            ServerManager.DefaultSettings = Settings;
            Client.ClientCallback.TellMessage("Saving settings.", OutputLevel.Info);
            ServerManager.DefaultSettings.Save();
            Client.ClientCallback.TellMessage("Settings saved.", OutputLevel.Info);
        }

        public ServerSettings GetServerSettings()
        {
            return ServerManager.DefaultSettings;
        }

        public void Restart()
        {
            Application.Restart();
        }
        public UserAccount GetLoggedInUser()
        {
            return LoggedInUser;
        }

        public OperationStatus RunExtension(string ExtensionName, ExtensionExecutionContext Context, params object[] Args)
        {
            OperationStatus s = _allExtensions[ExtensionName].Execute(Context, Args);
            return s;
        }

        public List<ExtensionDetails> GetExtensionNames()
        {
            List<ExtensionDetails> l = new List<ExtensionDetails>();
            foreach (KeyValuePair<string, ServerExtension> s in _allExtensions)
            {
                l.Add(s.Value.GeneralDetails);
            }
            return l;
        }
        public List<string> GetCommands()
        {
            return ServerManager.Commands.Keys.ToList();
        }

        public void StartWatcher(string WatcherName, object args)
        {
            ServerManager.Watchers[WatcherName].Start(args);
        }

        public ServerExtensionCollectionProgrammer GetCollectionProgrammer()
        {
            ServerExtensionCollectionProgrammer cprog = new ServerExtensionCollectionProgrammer();
            return cprog;
        }

        public ServerExtensionLibraryProgrammer GetServerLibraryProgrammer(string LibraryName)
        {
            ServerExtensionLibrary lib = ServerManager.DefaultCollection.Libraries[LibraryName];
            ServerExtensionLibraryProgrammer slprog = new ServerExtensionLibraryProgrammer(lib.FriendlyName, lib.Name, lib.LibraryType);
            return slprog;
        }

        public ServerExtensionProgrammer GetServerExtensionProgrammer(string ExtensionName)
        {
            ServerExtensionProgrammer seprog = new ServerExtensionProgrammer();
            seprog.ExtensionDetails = ServerManager.DefaultCollection.GetAllExtensions()[ExtensionName].GeneralDetails;
            return seprog;
        }

        public ServerExtensionProgrammer GetServerExtensionProgrammer(string LibraryName, string ExtensionName)
        {
            ServerExtensionProgrammer seprog = new ServerExtensionProgrammer();
            seprog.ExtensionDetails = ServerManager.DefaultCollection.Libraries[LibraryName].Extensions[ExtensionName].GeneralDetails;
            return seprog;
        }

        public void ProgramServerEstensionCollection(ServerExtensionCollectionProgrammer collectProgrammer)
        {
            
        }

        public void ProgramServerExtesnionLibrary(ServerExtensionLibraryProgrammer libProgrammer)
        {
            
        }

        public void ProgramServerExtension(string LibraryName, ServerExtensionProgrammer seProgrammer)
        {
            ServerExtensionProgrammerUpdateEvent programmerEvent = new ServerExtensionProgrammerUpdateEvent(seProgrammer);
            ServerManager.DefaultCollection.Libraries[LibraryName].Extensions[seProgrammer.ExtensionDetails.Name].ProgramRequested(programmerEvent);
            if(!programmerEvent.Cancel)
            {
                //TODO Add modifications to server extension here.
            }
        }
    }
}
