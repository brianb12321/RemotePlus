using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Speech.Synthesis;
using RemotePlusLibrary.Scripting;

namespace RemotePlusServer.Proxies
{
    internal class StaticRemoteFunctions
    {
        [IndexScriptObject]
        public static void speak(string message, int voiceGender, int voiceAge)
        {
            VoiceAge va = VoiceAge.Adult;
            VoiceGender vg = VoiceGender.Male;
            switch(voiceGender)
            {
                case 0:
                    vg = VoiceGender.Male;
                    break;
                case 1:
                    vg = VoiceGender.Female;
                    break;
                case 2:
                    vg = VoiceGender.Neutral;
                    break;
                case 3:
                    vg = VoiceGender.NotSet;
                    break;
                default:
                    throw new Exception("Invalid voice gender option.");
            }
            switch(voiceAge)
            {
                case 0:
                    va = VoiceAge.Adult;
                    break;
                case 1:
                    va = VoiceAge.Child;
                    break;
                case 2:
                    va = VoiceAge.Senior;
                    break;
                case 3:
                    va = VoiceAge.Teen;
                    break;
                case 4:
                    va = VoiceAge.NotSet;
                    break;
                default:
                    throw new Exception("Invalid voice age option.");
            }
            ServerManager.DefaultService.Remote.Speak(message, vg, va);
        }
        [IndexScriptObject]
        public static void beep(int freq, int duration)
        {
            ServerManager.DefaultService.Remote.Beep(freq, duration);
        }
    }
}
