using RemotePlusLibrary;
using System.ServiceModel;
using System.Windows.Forms;
using System;
using System.Net;
using System.Drawing;
using RemotePlusClient.CommonUI;
using RemotePlusLibrary.Extension.CommandSystem;
using System.Media;
using System.Diagnostics;
using System.Speech.Synthesis;
using System.Threading;
using RemotePlusLibrary.Security.AccountSystem;
using RemotePlusLibrary.RequestSystem;
using RemotePlusClient.UIForms;
using RemotePlusLibrary.Client;
using RemotePlusLibrary.Security.Authentication;
using RemotePlusLibrary.Contracts;
using BetterLogger;
using RemotePlusLibrary.Core;
using RemotePlusLibrary.Extension.ResourceSystem;
using TinyMessenger;

namespace RemotePlusClient
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Multiple,
        IncludeExceptionDetailInFaults = true,
        UseSynchronizationContext = false)]
    [ServiceKnownType("GetKnownTypes", typeof(DefaultKnownTypeManager))]
    public class ClientCallback : IRemoteClient
    {
        public int ServerPosition { get; private set; }
        public bool SwapFlag { get; private set; }
        public bool ConsoleStreamEnabled { get; private set; } = true;
        ILogFactory consoleStream = null;
        /// <summary>
        /// Tells the client which textbox to use for the server output
        /// </summary>
        /// <param name="logger"></param>
        public void SwapConsoleStream(ILogFactory logger)
        {
            consoleStream = logger;
            SwapFlag = true;
        }
        public void DisableConsoleStream()
        {
            ConsoleStreamEnabled = false;
        }
        public void EnableConsoleStream()
        {
            ConsoleStreamEnabled = true;
        }

        #region Callback Methods
        public ClientCallback(int serverPos)
        {
            ServerPosition = serverPos;
        }
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
        public void RunProgram(string Program, string Argument, bool shell, bool ignore)
        {
            var p = Process.Start(Program, Argument);
            if(!ignore)
            {
                p.WaitForExit();
            }
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
        public void Disconnect(Guid serverGuid, string Reason)
        {
            MainF.ConsoleObj.Logger.Log($"The server {serverGuid} disconnected from the client. Reason {Reason}", BetterLogger.LogLevel.Info, "Server host", serverGuid.ToString());
            MainF.Disconnect();
        }

        public UserCredentials RequestAuthentication(Guid serverGuid, AuthenticationRequest Request)
        {
            AuthenticationDialog ad = new AuthenticationDialog(Request);
            if(ad.ShowDialog() == DialogResult.OK)
            {
                return ad.UserInfo;
            }
            else
            {
                return null;
            }
        }

        public void TellMessage(Guid serverGuid, string Message, LogLevel o)
        {
            if (ConsoleStreamEnabled)
            {
                if (!SwapFlag)
                {
                    ClientApp.Logger.Log(Message, o, $"Server Host", $"({serverGuid}");
                }
                else
                {
                    consoleStream.Log(Message, o, $"Server Host", $"({serverGuid})");
                }
                MainF.ConsoleObj.Logger.Log(Message, o, $"Server Host",  $"({serverGuid})");
            }
        }


        public ClientBuilder RegisterClient()
        {
            ClientBuilder builder = new ClientBuilder(ClientType.GUI)
            {
                FriendlyName = "Default GUI Client"
            };
            builder.ExtraData.Add("global_newLine", "true");
            builder.ExtraData.Add("ps_appendNewLine", "false");
            return builder;
        }

        public void TellMessageToServerConsole(Guid serverGuid, string Message)
        {
            if (MainF.ServerConsoleObj == null)
            {
                MainF.ConsoleObj.AppendText(Message + Environment.NewLine);
            }
            else
            {
                MainF.ServerConsoleObj.AppendText(Message + Environment.NewLine);
            }
        }

        public ReturnData RequestInformation(Guid serverGuid, RequestBuilder builder)
        {
            ReturnData data = null;
            Thread t = new Thread((p) => data = RequestStore.Show((RequestBuilder)p));
            t.SetApartmentState(ApartmentState.STA);
            t.Start(builder);
            t.Join();
            return data;
        }
        public void UpdateRequest(Guid serverGuid, UpdateRequestBuilder message)
        {
            RequestStore.Update(message);
        }
        public void RegistirationComplete(Guid serverGuid)
        {
            //Role.RoleNames = MainF.Remote.GetServerRoleNames().ToArray();
        }

        public void SendSignal(SignalMessage message)
        {
            switch(message.Message)
            {
                case "fileTransfer":
                    ((RemoteFileBrowser)((MainF)Form.ActiveForm).TopPages["Remote File Browser"]).Counter = int.Parse(message.Value);
                    break;
            }
        }

        public void ChangePrompt(Guid serverGuid, RemotePlusLibrary.Extension.CommandSystem.PromptBuilder newPrompt)
        {
            //TODO: Implement Prompt
        }

        public RemotePlusLibrary.Extension.CommandSystem.PromptBuilder GetCurrentPrompt()
        {
            return null;
        }

        public void TellMessageToServerConsole(Guid serverGuid, ConsoleText text)
        {
            Color originalColor = MainF.ConsoleObj.ForeColor;
            MainF.ConsoleObj.ForeColor = text.TextColor;
            MainF.ConsoleObj.AppendText(text.Text + Environment.NewLine);
            MainF.ConsoleObj.ForeColor = originalColor;
        }

        public void TellMessageToServerConsole(Guid serverGuid, string Message, LogLevel level)
        {
            MainF.ServerConsoleObj.Logger.Log(Message, level, "Server Console");
        }

        public void TellMessageToServerConsole(Guid serverGuid, string Message, LogLevel level, string from)
        {
            MainF.ServerConsoleObj.Logger.Log(Message, level, from);
        }

        public void TellMessageToServerConsoleNoNewLine(Guid serverGuid, string Message)
        {
            MainF.ConsoleObj.AppendText(Message);
        }

        public Resource GetResource(string resourceIdentifier)
        {
            throw new NotImplementedException();
        }

        public void DisposeCurrentRequest(Guid serverGuid)
        {
            RequestStore.DisposeCurrentRequest();
        }

        public void PublishEvent(ITinyMessage message)
        {
            GlobalServices.EventBus.Publish(message);
        }
        #endregion
    }
}