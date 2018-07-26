using BetterLogger;
using RemotePlusLibrary;
using RemotePlusLibrary.Client;
using RemotePlusLibrary.Contracts;
using RemotePlusLibrary.Discovery;
using RemotePlusLibrary.Extension.CommandSystem;
using RemotePlusLibrary.RequestSystem;
using RemotePlusLibrary.Security.AccountSystem;
using RemotePlusLibrary.Security.Authentication;
using System;
using System.Speech.Synthesis;
using System.Windows.Forms;

namespace RemotePlusServer
{
    public class RemoteClient : IClient
    {
        bool useProxy = false;
        IProxyServerRemote proxyChannel = null;
        public RemoteClient(IRemoteClient client, bool up, IProxyServerRemote pc)
        {
            c = client;
            useProxy = up;
            proxyChannel = pc;
        }
        IRemoteClient c = null;
        public Guid Server = Guid.NewGuid();
        public void Beep(int Hertz, int Duration)
        {
            if (useProxy)
            {
                proxyChannel.Beep(Hertz, Duration);
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
                proxyChannel.ChangePrompt(Server, newPrompt);
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
                proxyChannel.Disconnect(Server, Reason);
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
                return proxyChannel.GetCurrentPrompt();
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
                proxyChannel.PlaySound(FileName);
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
                proxyChannel.PlaySoundLoop(FileName);
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
                proxyChannel.PlaySoundSync(FileName);
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
                return proxyChannel.RegisterClient();
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
                proxyChannel.RegistirationComplete(Server);
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
                return proxyChannel.RequestAuthentication(Server, Request);
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
                return proxyChannel.RequestInformation(Server, builder);
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
                proxyChannel.RunProgram(Program, Argument);
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
                proxyChannel.SendSignal(Server, signal);
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
                return proxyChannel.ShowMessageBox(Message, Caption, Icon, Buttons);
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
                proxyChannel.Speak(Message, Gender, Age);
            }
            else
            {
                c.Speak(Message, Gender, Age);
            }
        }

        public void TellMessage(string Message, LogLevel o)
        {
            if (useProxy)
            {
                proxyChannel.TellMessage(Server, Message, o);
            }
            else
            {
                c.TellMessage(Server, Message, o);
            }
        }
        public void TellMessageToServerConsole(string Message)
        {
            if (useProxy)
            {
                proxyChannel.TellMessageToServerConsole(Server, Message);
            }
            else
            {
                c.TellMessageToServerConsole(Server, Message);
            }
        }
        public void TellMessageToServerConsoleNoNewLine(string Message)
        {
            if (useProxy)
            {
                proxyChannel.TellMessageToServerConsoleNoNewLine(Server, Message);
            }
            else
            {
                c.TellMessageToServerConsoleNoNewLine(Server, Message);
            }
        }
        public void TellMessageToServerConsole(string message, LogLevel level)
        {
            if (useProxy)
            {
                proxyChannel.TellMessageToServerConsole(Server, message, level);
            }
            else
            {
                c.TellMessageToServerConsole(Server, message, level);
            }
        }
        public void TellMessageToServerConsole(string message, LogLevel level, string from)
        {
            if (useProxy)
            {
                proxyChannel.TellMessageToServerConsole(Server, message, level, from);
            }
            else
            {
                c.TellMessageToServerConsole(Server, message, level, from);
            }
        }
        public void TellMessageToServerConsole(ConsoleText text)
        {
            if (useProxy)
            {
                proxyChannel.TellMessageToServerConsole(Server, text);
            }
            else
            {
                c.TellMessageToServerConsole(Server, text);
            }
        }
    }
}
