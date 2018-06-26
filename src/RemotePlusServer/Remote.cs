using Logging;
using RemotePlusLibrary;
using RemotePlusLibrary.Client;
using RemotePlusLibrary.Contracts;
using RemotePlusLibrary.Extension.CommandSystem;
using RemotePlusLibrary.RequestSystem;
using RemotePlusLibrary.Security.AccountSystem;
using RemotePlusLibrary.Security.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Speech.Synthesis;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RemotePlusServer
{
    public class RemoteClient : IClient
    {
        bool useProxy = false;
        public RemoteClient(IRemoteClient client, bool up)
        {
            c = client;
            useProxy = up;
        }
        IRemoteClient c = null;
        public Guid Server = Guid.NewGuid();
        public void Beep(int Hertz, int Duration)
        {
            if (useProxy)
            {
                ServerManager.proxyChannel.Beep(Hertz, Duration);
            }
            else
            {
                c.Beep(Hertz, Duration);
            }
        }

        public void ChangePrompt(RemotePlusLibrary.Extension.CommandSystem.PromptBuilder newPrompt)
        {
            if (useProxy)
            {
                ServerManager.proxyChannel.ChangePrompt(Server, newPrompt);
            }
            else
            {
                c.ChangePrompt(Server, newPrompt);
            }
        }

        public void Disconnect(string Reason)
        {
            if (useProxy)
            {
                ServerManager.proxyChannel.Disconnect(Server, Reason);
            }
            else
            {
                c.Disconnect(Server, Reason);
            }
        }

        public RemotePlusLibrary.Extension.CommandSystem.PromptBuilder GetCurrentPrompt()
        {
            if (useProxy)
            {
                return ServerManager.proxyChannel.GetCurrentPrompt();
            }
            else
            {
                return c.GetCurrentPrompt();
            }
        }

        public void PlaySound(string FileName)
        {
            if (useProxy)
            {
                ServerManager.proxyChannel.PlaySound(FileName);
            }
            else
            {
                c.PlaySound(FileName);
            }
        }

        public void PlaySoundLoop(string FileName)
        {
            if (useProxy)
            {
                ServerManager.proxyChannel.PlaySoundLoop(FileName);
            }
            else
            {
                c.PlaySoundLoop(FileName);
            }
        }

        public void PlaySoundSync(string FileName)
        {
            if (useProxy)
            {
                ServerManager.proxyChannel.PlaySoundSync(FileName);
            }
            else
            {
                c.PlaySoundSync(FileName);
            }
        }

        public ClientBuilder RegisterClient()
        {
            if (useProxy)
            {
                return ServerManager.proxyChannel.RegisterClient();
            }
            else
            {
                return c.RegisterClient();
            }
        }

        public void RegistirationComplete()
        {
            if (useProxy)
            {
                ServerManager.proxyChannel.RegistirationComplete(Server);
            }
            else
            {
                c.RegistirationComplete(Server);
            }
        }

        public UserCredentials RequestAuthentication(AuthenticationRequest Request)
        {
            if (useProxy)
            {
                return ServerManager.proxyChannel.RequestAuthentication(Server, Request);
            }
            else
            {
                return c.RequestAuthentication(Server, Request);
            }
        }

        public ReturnData RequestInformation(RequestBuilder builder)
        {
            if (useProxy)
            {
                return ServerManager.proxyChannel.RequestInformation(Server, builder);
            }
            else
            {
                return c.RequestInformation(Server, builder);
            }
        }

        public void RunProgram(string Program, string Argument)
        {
            if (useProxy)
            {
                ServerManager.proxyChannel.RunProgram(Program, Argument);
            }
            else
            {
                c.RunProgram(Program, Argument);
            }
        }

        public void SendSignal(SignalMessage signal)
        {
            if (useProxy)
            {
                ServerManager.proxyChannel.SendSignal(Server, signal);
            }
            else
            {
                c.SendSignal(Server, signal);
            }
        }

        public DialogResult ShowMessageBox(string Message, string Caption, MessageBoxIcon Icon, MessageBoxButtons Buttons)
        {
            if (useProxy)
            {
                return ServerManager.proxyChannel.ShowMessageBox(Message, Caption, Icon, Buttons);
            }
            else
            {
                return c.ShowMessageBox(Message, Caption, Icon, Buttons);
            }
        }

        public void Speak(string Message, VoiceGender Gender, VoiceAge Age)
        {
            if (useProxy)
            {
                ServerManager.proxyChannel.Speak(Message, Gender, Age);
            }
            else
            {
                c.Speak(Message, Gender, Age);
            }
        }

        public void TellMessage(string Message, OutputLevel o)
        {
            if (useProxy)
            {
                ServerManager.proxyChannel.TellMessage(Server, Message, o);
            }
            else
            {
                c.TellMessage(Server, Message, o);
            }
        }

        public void TellMessage(UILogItem li)
        {
            if (useProxy)
            {
                ServerManager.proxyChannel.TellMessage(Server, li);
            }
            else
            {
                c.TellMessage(Server, li);
            }
        }

        public void TellMessage(UILogItem[] Logs)
        {
            if (useProxy)
            {
                ServerManager.proxyChannel.TellMessage(Server, Logs);
            }
            else
            {
                c.TellMessage(Server, Logs);
            }
        }

        public void TellMessageToServerConsole(UILogItem li)
        {
            if (useProxy)
            {
                ServerManager.proxyChannel.TellMessageToServerConsole(Server, li);
            }
            else
            {
                c.TellMessageToServerConsole(Server, li);
            }
        }

        public void TellMessageToServerConsole(string Message)
        {
            if (useProxy)
            {
                ServerManager.proxyChannel.TellMessageToServerConsole(Server, Message);
            }
            else
            {
                c.TellMessageToServerConsole(Server, Message);
            }
        }

        public void TellMessageToServerConsole(ConsoleText text)
        {
            if (useProxy)
            {
                ServerManager.proxyChannel.TellMessageToServerConsole(Server, text);
            }
            else
            {
                c.TellMessageToServerConsole(Server, text);
            }
        }
    }
}
