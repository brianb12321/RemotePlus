using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using RemotePlusLibrary;
using System.Windows.Forms;
using System.IO;
using Logging;
using System.Speech.Synthesis;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Channels;
using RemotePlusLibrary.Core;
using RemotePlusLibrary.Extension;
using RemotePlusLibrary.FileTransfer;
using System.Net.Sockets;
using System.Diagnostics;

namespace RemotePlusServer
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant,
        IncludeExceptionDetailInFaults = true,
        InstanceContextMode = InstanceContextMode.Single)]
    [GlobalException(typeof(GlobalErrorHandler))]
    public class RemoteImpl : IRemote
    {
        public RemoteImpl()
        {
            Extensions = new Dictionary<string, ServerExtension>();
        }
        public RegistirationObject Settings { get; private set; }
        public Client Client { get; set; }
        public bool Registered { get; private set; }
        public UserAccount LoggedInUser { get; private set; }
        public Dictionary<string, ServerExtension> Extensions { get; private set; }
        public ServerExtension SelectedExtension { get; private set; }
        void CheckRegisteration()
        {
            if (!Registered)
            {
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
            var callback = OperationContext.Current.GetCallbackChannel<IRemoteClient>();
            Client = callback.RegisterClient().Build(callback);
            this.Settings = Settings;
            if (Settings.LoginRightAway)
            {
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
                UserCredentials upp = Client.ClientCallback.RequestAuthentication();
                if (upp == null)
                {
                    Client.ClientCallback.TellMessage("Can't you at least provide a username and password?", OutputLevel.Info);
                    Client.ClientCallback.Disconnect("Authentication failed.");
                }
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
            ServerManager.Logger.AddOutput("Client registired.", Logging.OutputLevel.Info);
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
                    Client.ClientCallback.TellMessageToServerConsole(e.Data + "\n");
                }
            };
            p.OutputDataReceived += (sender, e) =>
            {
                if (e.Data != null)
                {
                    Client.ClientCallback.TellMessageToServerConsole(e.Data + "\n");
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
        public void AddExtension(ServerExtension ext)
        {
            Extensions.Add(ext.GeneralDetails.Name, ext);
        }
        public UserAccount GetLoggedInUser()
        {
            return LoggedInUser;
        }

        public OperationStatus RunExtension(string ExtensionName, params object[] Args)
        {
            SelectedExtension = Extensions[ExtensionName];
            OperationStatus s = SelectedExtension.Execute(Args);
            s.Success = true;
            return s;
        }

        public List<ExtensionDetails> GetExtensionNames()
        {
            List<ExtensionDetails> l = new List<ExtensionDetails>();
            foreach (KeyValuePair<string, ServerExtension> s in Extensions)
            {
                l.Add(s.Value.GeneralDetails);
            }
            return l;
        }

        public RemoteFileInfo DownloadFile(DownloadRequest request)
        {
            RemoteFileInfo result = new RemoteFileInfo();
            try
            {
                string filePath = request.DownloadFileName;
                FileInfo fileInfo = new FileInfo(filePath);
                if (!fileInfo.Exists)
                {
                    throw new FileNotFoundException("The file " + filePath + " does not exist on the server.");
                }
                FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                result.FileByteStream = fs;
                result.FileName = filePath;
                result.Length = fileInfo.Length;

            }
            catch (Exception ex)
            {
                throw new FaultException(ex.Message);
            }
            return result;
        }

        public void UploadFile(RemoteFileInfo request)
        {
            using (Stream stream = request.FileByteStream)
            {
                using (Stream sstream = new FileStream(request.FileName, FileMode.Create, FileAccess.ReadWrite))
                {
                    byte[] buffer = new byte[1024];
                    int length;
                    while ((length = stream.Read(buffer, 0, buffer.Length)) != 0)
                    {
                        sstream.Write(buffer, 0, length);
                    }
                }
            }
        }

        public void HaultExtension()
        {
            SelectedExtension.HaultExtension();
        }

        public void ResumeExtension()
        {
            SelectedExtension.ResumeExtension();
        }
        public List<string> GetCommands()
        {
            return ServerManager.Commands.Keys.ToList();
        }
    }
}
