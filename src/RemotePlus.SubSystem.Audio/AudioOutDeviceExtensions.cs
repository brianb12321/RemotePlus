using NAudio.Wave;
using RemotePlusLibrary.Extension.ResourceSystem.ResourceTypes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.SubSystem.Audio
{
    public static class AudioOutDeviceExtensions
    {
        /// <summary>
        /// Initialized the audio device with raw audio data.
        /// </summary>
        /// <param name="device"></param>
        /// <param name="stream">The stream to read audio from.</param>
        public static void InitWithRawStream(this AudioOutDevice device, Stream stream)
        {
            device.InitProvider(new RawSourceWaveStream(stream, new NAudio.Wave.WaveFormat()));
        }
        /// <summary>
        /// Initialized the audio device with raw audio data.
        /// </summary>
        /// <param name="device"></param>
        /// <param name="stream">The stream to read audio from.</param>
        /// <param name="sampleRate">The sample rate to play.</param>
        /// <param name="channels">The number of audio channels to use for playback.</param>
        public static void InitWithRawStream(this AudioOutDevice device, Stream stream, int sampleRate, int channels)
        {
            device.InitProvider(new RawSourceWaveStream(stream, new WaveFormat(sampleRate, channels)));
        }
        /// <summary>
        /// Initialized the audio device with raw audio data.
        /// </summary>
        /// <param name="device"></param>
        /// <param name="stream">The stream to read audio from.</param>
        /// <param name="sampleRate">he sample rate to play.</param>
        /// <param name="bit">The bit depth of the audio.</param>
        /// <param name="channel">The number of audio channels to use for playback.</param>
        public static void InitWithRawStream(this AudioOutDevice device, Stream stream, int sampleRate, int bit, int channel)
        {
            device.InitProvider(new RawSourceWaveStream(stream, new WaveFormat(sampleRate, bit, channel)));
        }
        /// <summary>
        /// Initialized the audio device with raw audio data provided by a resource.
        /// </summary>
        /// <param name="device"></param>
        /// <param name="stream">The stream to read audio from.</param>
        public static void InitWithRawResource(this AudioOutDevice device, IOResource resource)
        {
            device.InitProvider(new RawSourceWaveStream(resource.OpenReadStream(), new WaveFormat()));
        }
        /// <summary>
        /// Initialized the audio device with raw audio data provided by a resource.
        /// </summary>
        /// <param name="device"></param>
        /// <param name="stream">The stream to read audio from.</param>
        /// <param name="sampleRate">The sample rate to play.</param>
        /// <param name="channels">The number of audio channels to use for playback.</param>
        public static void InitWithRawResource(this AudioOutDevice device, IOResource resource, int sampleRate, int channels)
        {
            device.InitProvider(new RawSourceWaveStream(resource.OpenReadStream(), new WaveFormat(sampleRate, channels)));

        }
        /// <summary>
        /// Initialized the audio device with raw audio data provided by a resource.
        /// </summary>
        /// <param name="device"></param>
        /// <param name="stream">The stream to read audio from.</param>
        /// <param name="sampleRate">he sample rate to play.</param>
        /// <param name="bit">The bit depth of the audio.</param>
        /// <param name="channel">The number of audio channels to use for playback.</param>
        public static void InitWithRawResource(this AudioOutDevice device, IOResource resource, int sampleRate, int bit, int channels)
        {
            device.InitProvider(new RawSourceWaveStream(resource.OpenReadStream(), new WaveFormat(sampleRate, bit, channels)));
        }
        /// <summary>
        /// Initialized the audio device with a wave file represented by a stream.
        /// </summary>
        /// <param name="device"></param>
        /// <param name="stream">The stream to read the wave file from.</param>
        public static void InitWithWaveStream(this AudioOutDevice device, Stream stream)
        {
            device.InitProvider(new WaveFileReader(stream));
        }
        /// <summary>
        /// Initialized the audio device with a wave file represented by a resource.
        /// </summary>
        /// <param name="device"></param>
        /// <param name="stream">The stream to read the wave file from.</param>
        public static void InitWithWaveResource(this AudioOutDevice device, IOResource resource)
        {
            device.InitProvider(new WaveFileReader(resource.OpenReadStream()));
        }
    }
}