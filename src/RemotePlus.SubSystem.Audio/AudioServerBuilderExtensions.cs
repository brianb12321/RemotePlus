using RemotePlusLibrary.Core.IOC;
using RemotePlusLibrary.Extension.ResourceSystem;
using RemotePlusLibrary.SubSystem.Audio.OutDevices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.SubSystem.Audio
{
    public static class AudioServerBuilderExtensions
    {
        public static IServerBuilder AddAudioDevices(this IServerBuilder builder)
        {
            return builder.AddTask(() =>
            {
                var resourceManager = IOCContainer.GetService<IResourceManager>();
                resourceManager.AddResourceDirectory("/dev", "audio");
                WaveOutDevice.Searcher("waveOut").ToList().ForEach(d => resourceManager.AddResource("/dev/audio", d));
                DirectSoundOutDevice.Searcher("directSoundOut").ToList().ForEach(d => resourceManager.AddResource("/dev/audio", d));
                WasapiOutDevice.Searcher("wasapiOut").ToList().ForEach(d => resourceManager.AddResource("/dev/audio", d));
            });
        }
    }
}
