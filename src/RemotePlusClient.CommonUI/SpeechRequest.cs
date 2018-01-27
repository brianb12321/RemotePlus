using RemotePlusLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Speech.Synthesis;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusClient.CommonUI
{
    public class SpeechRequest : IDataRequest
    {
        public bool ShowProperties => false;

        public string FriendlyName => "Speech Request";

        public string Description => "Uses a speech synthesizer to create speech.";

        public RawDataRequest RequestData(RequestBuilder builder)
        {
            SpeechSynthesizer ss = new SpeechSynthesizer();
            ss.SelectVoiceByHints((VoiceGender)Enum.Parse(typeof(VoiceGender), builder.Metadata["vg"]), (VoiceAge)Enum.Parse(typeof(VoiceAge), builder.Metadata["va"]));
            ss.Speak(builder.Message);
            return RawDataRequest.Success(null);
        }

        public void Update(string message)
        {
            throw new NotImplementedException();
        }

        public void UpdateProperties()
        {
            throw new NotImplementedException();
        }
    }
}
