using RemotePlusLibrary.Core.IOC;
using RemotePlusLibrary.Extension.ResourceSystem;
using RemotePlusLibrary.SubSystem.Audio.InDevices;
using RemotePlusLibrary.SubSystem.Audio.OutDevices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RemotePlusLibrary.Core.NodeStartup;

namespace RemotePlusLibrary.SubSystem.Audio
{
    public static class AudioServerBuilderExtensions
    {
        public static INodeBuilder<IServerTaskBuilder> AddAudioDevices(this INodeBuilder<IServerTaskBuilder> builder)
        {
            return builder.AddTask(() =>
            {
                var resourceManager = IOCContainer.GetService<IResourceManager>();
                resourceManager.AddResourceDirectory("/dev", "audio");
                WaveOutDevice.Searcher("waveOut").ToList().ForEach(d => resourceManager.AddResource("/dev/audio", d));
                DirectSoundOutDevice.Searcher("directSoundOut").ToList().ForEach(d => resourceManager.AddResource("/dev/audio", d));
                WasapiOutDevice.Searcher("wasapiOut").ToList().ForEach(d => resourceManager.AddResource("/dev/audio", d));
                WaveInDevice.Searcher("waveIn").ToList().ForEach(d => resourceManager.AddResource("/dev/audio", d));
            });
        }
    }
}
