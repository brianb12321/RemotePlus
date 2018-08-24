using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Speech.Synthesis;
using RemotePlusLibrary.Scripting;
using RemotePlusServer.Core;

namespace RemotePlusServer.Core.Proxies
{
    public class StaticRemoteFunctions
    {
        [IndexScriptObject]
        public static void speak(string message, VoiceGender voiceGender, VoiceAge voiceAge)
        {
            ServerManager.ServerRemoteService.RemoteInterface.Speak(message, voiceGender, voiceAge);
        }
        [IndexScriptObject]
        public static void beep(int freq, int duration)
        {
            ServerManager.ServerRemoteService.RemoteInterface.Beep(freq, duration);
        }
    }
}
