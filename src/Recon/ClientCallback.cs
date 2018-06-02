using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Media;
using System.ServiceModel;
using System.Speech.Synthesis;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Logging;
using RemotePlusClient.CommonUI;
using RemotePlusLibrary;
using RemotePlusLibrary.Extension.CommandSystem;
using RemotePlusLibrary.AccountSystem;

namespace Recon
{
    [CallbackBehavior(IncludeExceptionDetailInFaults = true,
        ConcurrencyMode = ConcurrencyMode.Multiple,
        UseSynchronizationContext = false)]
    public class ClientCallback : IRemoteClient
    {
        public void Beep(int Hertz, int Duration)
        {
            Console.Beep(Hertz, Duration);
        }
        public void PlaySound(string FileName)
        {
            SoundPlayer player = new SoundPlayer(FileName);
            player.Play();
        }

        public void PlaySoundLoop(string FileName)
        {
            SoundPlayer player = new SoundPlayer(FileName);
            player.PlaySync();
        }

        public void PlaySoundSync(string FileName)
        {
            SoundPlayer player = new SoundPlayer(FileName);
            player.PlayLooping();
        }
        public void RunProgram(string Program, string Argument)
        {
            Process.Start(Program, Argument);
        }
        public DialogResult ShowMessageBox(string Message, string Caption, MessageBoxIcon Icon, MessageBoxButtons Buttons)
        {
            return MessageBox.Show(Message, Caption, Buttons, Icon);
        }

        public void Speak(string Message, VoiceGender Gender, VoiceAge Age)
        {
            SpeechSynthesizer ss = new SpeechSynthesizer();
            ss.SelectVoiceByHints(Gender, Age);
            ss.Speak(Message);
        }
        public void ChangePrompt(RemotePlusLibrary.Extension.CommandSystem.PromptBuilder newPrompt)
        {
            
        }

        public void Disconnect(string Reason)
        {
            ReconManager.Icon.ShowBalloonTip(5000, "Disconnected", $"You have been disconnected by the server: {Reason}", ToolTipIcon.Error);
            ReconManager.Logger.AddOutput($"Disconnected: {Reason}", OutputLevel.Info);
            ReconManager.Client.Close();
        }

        public RemotePlusLibrary.Extension.CommandSystem.PromptBuilder GetCurrentPrompt()
        {
            return null;
        }

        public ClientBuilder RegisterClient()
        {
            ClientBuilder cb = new ClientBuilder(ClientType.Headless);
            cb.FriendlyName = "Recon";
            return cb;
        }

        public void RegistirationComplete()
        {
            ReconManager.Icon.ShowBalloonTip(5000, "You are now connected!", "You are now connected to the server.", ToolTipIcon.Info);
            ReconManager.Menu.Invoke((MethodInvoker)(() => ReconManager.Menu.Items["Connect"].Enabled = false));
            ReconManager.Menu.Invoke((MethodInvoker)(() => ReconManager.Menu.Items["ExecuteScript"].Enabled = true));
        }

        public UserCredentials RequestAuthentication(AuthenticationRequest Request)
        {
            AuthenticationDialog ad = new AuthenticationDialog(Request);
            if (ad.ShowDialog() == DialogResult.OK)
            {
                return ad.UserInfo;
            }
            else
            {
                return null;
            }
        }

        public ReturnData RequestInformation(RequestBuilder builder)
        {
            return RequestStore.Show(builder);
        }

        public void SendSignal(SignalMessage signal)
        {
            switch(signal.Message)
            {
                case "recon_post":
                    ReconManager.Icon.ShowBalloonTip(1000, "Server Script", $"{signal.Value}", ToolTipIcon.Info);
                    break;
            }
        }

        public void TellMessage(string Message, OutputLevel o)
        {
            ReconManager.Logger.AddOutput(Message, o);
        }

        public void TellMessage(UILogItem li)
        {
            ReconManager.Logger.AddOutput(li);
        }

        public void TellMessage(UILogItem[] Logs)
        {
            foreach (LogItem li in Logs)
            {
                ReconManager.Logger.AddOutput(li);
            }
        }

        public void TellMessageToServerConsole(UILogItem li)
        {
            li.From = "Server Console " + li.From;
            ReconManager.Logger.AddOutput(li);
        }

        public void TellMessageToServerConsole(string Message)
        {
            Console.WriteLine(Message);
        }

        public void TellMessageToServerConsole(ConsoleText text)
        {
            Colorful.Console.ForegroundColor = text.TextColor;
            Colorful.Console.Write(text.Text);
            Colorful.Console.ResetColor();
        }
    }
}
