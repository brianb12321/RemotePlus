using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AudioSwitcher.AudioApi.CoreAudio;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using NDesk.Options;
using Ninject;
using RemotePlusLibrary.Core.IOC;
using RemotePlusLibrary.Extension;
using RemotePlusLibrary.Extension.ResourceSystem;
using RemotePlusLibrary.Extension.ResourceSystem.ResourceTypes;
using RemotePlusLibrary.RequestSystem;
using RemotePlusLibrary.RequestSystem.DefaultRequestBuilders;
using RemotePlusLibrary.ServiceArchitecture;
using RemotePlusLibrary.SubSystem.Audio.OutDevices;
using RemotePlusLibrary.SubSystem.Command;
using RemotePlusLibrary.SubSystem.Command.CommandClasses;
using RemotePlusServer;
using RemotePlusServer.Core;
using RemotePlusServer.Core.ExtensionSystem;

namespace RemotePlusLibrary.SubSystem.Audio
{
    [ExtensionModule]
    public class AudioCommands : ServerCommandClass
    {
        IRemotePlusService<ServerRemoteInterface> _service;
        IResourceManager _resourceManager;

        public object IWaveFormat { get; private set; }
        [CommandHelp("Loads an audio stream as a wave provider.")]
        public CommandResponse loadAudio(CommandRequest req, CommandPipeline pipe, ICommandEnvironment currentEnvironment)
        {
            string audioFileFormatString = "wav";
            string audioResourceString = string.Empty;
            int sampleRate = 44100;
            int bitDepth = 16;
            int channels = 2;
            int silenceDuration = 1000;
            bool showHelp = false;
            bool useFile = false;
            bool concatenate = false;
            OptionSet set = new OptionSet()
                .Add("useFile|u", "Use a file path instead of a resource path.", v => useFile = true)
                .Add("concatenate|e", "Cancatenates a audio file with the previous audio stream.", v => concatenate = true)
                .Add("silenceDuration|s=", "When concatenating, determines in milliseconds the amount of silence before playing again.", v =>
                {
                    if (int.TryParse(v, out int result))
                    {
                        silenceDuration = result;
                    }
                    else
                    {
                        currentEnvironment.WriteLine(new ConsoleText("Silence duration must be a number.") { TextColor = Color.Red });
                    }
                })
                .Add("audioFileFormat|f=", "The audio format that the file will be read as. The following formats are supported: wav, mp3, raw", v => audioFileFormatString = v)
                .Add("audioResource|a=", "The audio resource or file to open.", v => audioResourceString = v)
                .Add("sampleRate|r=", "When in raw format, sets the sample rate of playback. Default is 44100 hz.", v =>
                {
                    if (int.TryParse(v, out int result))
                    {
                        sampleRate = result;
                    }
                    else
                    {
                        currentEnvironment.WriteLine(new ConsoleText("Sample rate must be a number.") { TextColor = Color.Red });
                    }
                })
                .Add("bitDepth|b=", "When in raw format, sets the bit depth of audio. Default is 16.", v =>
                {
                    if (int.TryParse(v, out int result))
                    {
                        bitDepth = result;
                    }
                    else
                    {
                        currentEnvironment.WriteLine(new ConsoleText("bit depth must be a number.") { TextColor = Color.Red });
                    }
                })
                .Add("channels|c=", "When in raw format, sets the number of channels to use for playback.", v =>
                {
                    if (int.TryParse(v, out int result))
                    {
                        channels = result;
                    }
                    else
                    {
                        currentEnvironment.WriteLine(new ConsoleText("channel must be a number.") { TextColor = Color.Red });
                    }
                })
                .Add("help|?", "Shows the help screen.", v => showHelp = true);
            set.Parse(req.Arguments.Select(e => e.ToString()));
            if(showHelp)
            {
                set.WriteOptionDescriptions(currentEnvironment.Out);
                currentEnvironment.WriteLine();
                return new CommandResponse((int)CommandStatus.Success);
            }
            ResourceQuery query = null;
            IWaveProvider provider = null;
            IOResource audio = null;
            if (useFile)
            {
                audio = new FilePointerResource(audioResourceString, Path.GetFileNameWithoutExtension(audioResourceString));
            }
            else if(!string.IsNullOrEmpty(audioResourceString))
            {
                query = new ResourceQuery(audioResourceString, Guid.Empty);
                audio = _resourceManager.GetResource<IOResource>(query);
            }
            switch (audioFileFormatString.ToUpper())
            {
                case "WAV":
                    provider = new WaveFileReader(audio.OpenReadStream());
                    break;
                case "MP3":
                    provider = new Mp3FileReader(audio.OpenReadStream());
                    break;
                case "RAW":
                    provider = new RawSourceWaveStream(audio.OpenReadStream(), new WaveFormat(sampleRate, bitDepth, channels));
                    break;
                default:
                    currentEnvironment.WriteLine(new ConsoleText("Invalid audio file format.") { TextColor = Color.Red });
                    return new CommandResponse((int)CommandStatus.Fail);
            }
            if(concatenate)
            {
                if(req.HasLastCommand && req.LastCommand.ReturnData is IWaveProvider)
                {
                    return new CommandResponse((int)CommandStatus.Success)
                    {
                        ReturnData = provider.ToSampleProvider()
                        .FollowedBy(TimeSpan.FromMilliseconds(silenceDuration), ((IWaveProvider)req.LastCommand.ReturnData).ToSampleProvider())
                        .ToWaveProvider()
                    };
                }
                else
                {
                    currentEnvironment.WriteLine(new ConsoleText("No wave provider in pipeline.") { TextColor = Color.Red });
                    return new CommandResponse((int)CommandStatus.Fail);
                }
            }
            return new CommandResponse((int)CommandStatus.Success)
            {
                ReturnData = provider
            };
        }
        [CommandHelp("Plays an audio file (wav) sent by the client.")]
        public CommandResponse playAudio(CommandRequest req, CommandPipeline pipe, ICommandEnvironment currentEnvironment)
        {
            var client = currentEnvironment.ClientContext.GetClient<RemoteClient>();
            string audioDeviceString = string.Empty;
            string audioResourceString = string.Empty;
            string audioFileFormatString = "wav";
            int sampleRate = 44100;
            int bitDepth = 16;
            int channels = 2;
            bool displayHelp = false;
            int pitch = 1;
            OptionSet set = new OptionSet()
                .Add("audioDevice|d=", "The audio device to send audio data to.", v => audioDeviceString = v)
                .Add("audioFileFormat|f=", "The audio format that the file will be read as. The following formats are supported: wav, mp3, raw", v => audioFileFormatString = v)
                .Add("audioResource|a=", "The audio resource to open. Leave empty to select a file from the client.", v => audioResourceString = v)
                .Add("sampleRate|r=", "When in raw format, sets the sample rate of playback. Default is 44100 hz.", v =>
                {
                    if (int.TryParse(v, out int result))
                    {
                        sampleRate = result;
                    }
                    else
                    {
                        currentEnvironment.WriteLine(new ConsoleText("Sample rate must be a number.") { TextColor = Color.Red });
                    }
                })
                .Add("bitDepth|b=", "When in raw format, sets the bit depth of audio. Default is 16.", v =>
                {
                    if (int.TryParse(v, out int result))
                    {
                        bitDepth = result;
                    }
                    else
                    {
                        currentEnvironment.WriteLine(new ConsoleText("bit depth must be a number.") { TextColor = Color.Red });
                    }
                })
                .Add("channels|c=", "When in raw format, sets the number of channels to use for playback.", v =>
                {
                    if (int.TryParse(v, out int result))
                    {
                        channels = result;
                    }
                    else
                    {
                        currentEnvironment.WriteLine(new ConsoleText("channel must be a number.") { TextColor = Color.Red });
                    }
                })
                .Add("help|?", "Displays the help screen", v => displayHelp = true)
                .Add("pitch|p=", "Raises or lowers the pitch of audio by x amount of semitones.", v =>
                {
                    if (int.TryParse(v, out int result))
                    {
                        pitch = result;
                    }
                    else
                    {
                        currentEnvironment.WriteLine(new ConsoleText("Pitch must be a number.") { TextColor = Color.Red });
                    }
                });
            set.Parse(req.Arguments.Select(e => e.ToString()));
            if (displayHelp)
            {
                set.WriteOptionDescriptions(currentEnvironment.Out);
                currentEnvironment.WriteLine();
                return new CommandResponse((int)CommandStatus.Success);
            }
            AudioOutDevice device = null;
            if (!string.IsNullOrEmpty(audioDeviceString))
            {
                device = _resourceManager.GetResource<AudioOutDevice>(new ResourceQuery(audioDeviceString, Guid.Empty));
            }
            else
            {
                device = _resourceManager.GetResource<AudioOutDevice>(new ResourceQuery("/dev/audio/waveOutDefault", Guid.Empty));
            }
            //Play what has been passed to.
            if (req.HasLastCommand && req.LastCommand.ReturnData is IWaveProvider)
            {
                IWaveProvider passedProvider = req.LastCommand.ReturnData as IWaveProvider;
                device.InitProvider(passedProvider);
                device.Play();
                return new CommandResponse((int)CommandStatus.Success);
            }
            bool removeResource = true;
            ResourceQuery query = null;
            if (!string.IsNullOrEmpty(audioResourceString))
            {
                query = new ResourceQuery(audioResourceString, Guid.Empty);
                removeResource = false;
            }
            else
            {
                var requestPathBuilder = new FileDialogRequestBuilder()
                {
                    Title = "Select audio file.",
                    Filter = "Wav File (*.wav)|*.wav"
                };
                var path = client.ClientCallback.RequestInformation(requestPathBuilder);
                if (path.AcquisitionState == RequestState.OK)
                {
                    client.ClientCallback.RequestInformation(new SendLocalFileByteStreamRequestBuilder(Path.GetFileName(path.Data.ToString()), path.Data.ToString())
                    {
                        PathToSave = "/temp"
                    });
                    query = new ResourceQuery($"/temp/{Path.GetFileName(path.Data.ToString())}", Guid.Empty);
                }
                else
                {
                    return new CommandResponse((int)CommandStatus.Fail);
                }
            }
            currentEnvironment.WriteLine($"Going to play audio file.");
            var audio = _resourceManager.GetResource<IOResource>(query);
            currentEnvironment.WriteLine($"Now playing audio file. Name {audio.ToString()}");
            IWaveProvider provider = null;
            switch(audioFileFormatString.ToUpper())
            {
                case "WAV":
                    provider = new WaveFileReader(audio.OpenReadStream());
                    break;
                case "MP3":
                    provider = new Mp3FileReader(audio.OpenReadStream());
                    break;
                case "RAW":
                    provider = new RawSourceWaveStream(audio.OpenReadStream(), new WaveFormat(sampleRate, bitDepth, channels));
                    break;
                default:
                    currentEnvironment.WriteLine(new ConsoleText("Invalid audio file format.") { TextColor = Color.Red });
                    return new CommandResponse((int)CommandStatus.Fail);
            }
            if (pitch != 1)
            {
                SmbPitchShiftingSampleProvider shifter = new SmbPitchShiftingSampleProvider(provider.ToSampleProvider());
                var semiTone = Math.Pow(2, 1.0 / 12);
                shifter.PitchFactor = (float)((pitch > 1) ? semiTone * pitch : 1.0 / (semiTone * Math.Abs(pitch)));
                provider = shifter.ToWaveProvider();
            }
            device.InitProvider(provider);
            device.PlaybackStopped += (sender, e) =>
            {
                if (removeResource)
                {
                    _resourceManager.RemoveResource(audio.Path);
                }
            };
            device.Play();
            audio.Close();
            return new CommandResponse((int)CommandStatus.Success);
        }
        [CommandHelp("Plays a sound wave generated by an oscillator.")]
        public CommandResponse oscAudio(CommandRequest req, CommandPipeline pipe, ICommandEnvironment currentEnvironment)
        {
            string toneType = string.Empty;
            double duration = 1000;
            int frequency = 1000;
            int sampleRate = 44100;
            double gain = 0.2;
            int channels = 2;
            bool showHelp = false;
            OptionSet set = new OptionSet()
                .Add("toneType|t=", "The tone to generate: Sine, Square, Triangle, SawTooth, PNoise, WNoise", v => toneType = v)
                .Add("duration|d=", "The duration in milliseconds to play audio. Set 0 for infinite.", v =>
                {
                    if (double.TryParse(v, out double result))
                    {
                        duration = result;
                    }
                    else
                    {
                        currentEnvironment.WriteLine(new ConsoleText("Duration must be a number.") { TextColor = Color.Red });
                    }
                })
                .Add("frequency|f=", "The frequency to play the sound wave at.", v =>
                {
                    if (int.TryParse(v, out int result))
                    {
                        frequency = result;
                    }
                    else
                    {
                        currentEnvironment.WriteLine(new ConsoleText("Frequency must be a number.") { TextColor = Color.Red });
                    }
                })
                .Add("sampleRate|r=", "When in raw format, sets the sample rate of playback. Default is 44100 hz.", v =>
                {
                    if (int.TryParse(v, out int result))
                    {
                        sampleRate = result;
                    }
                    else
                    {
                        currentEnvironment.WriteLine(new ConsoleText("Sample rate must be a number.") { TextColor = Color.Red });
                    }
                })
                .Add("channels|c=", "When in raw format, sets the number of channels to use for playback.", v =>
                {
                    if (int.TryParse(v, out int result))
                    {
                        channels = result;
                    }
                    else
                    {
                        currentEnvironment.WriteLine(new ConsoleText("Channel must be a number.") { TextColor = Color.Red });
                    }
                })
                .Add("gain|g=", "Determines how loud to play the tone.", v =>
                {
                    if (double.TryParse(v, out double result))
                    {
                        gain = result;
                    }
                    else
                    {
                        currentEnvironment.WriteLine(new ConsoleText("Gain must be a number.") { TextColor = Color.Red });
                    }
                })
                .Add("help|?", "Shows the help screen.", v => showHelp = true);
            set.Parse(req.Arguments.Select(e => e.ToString()));
            if(showHelp)
            {
                set.WriteOptionDescriptions(currentEnvironment.Out);
                currentEnvironment.WriteLine();
                return new CommandResponse((int)CommandStatus.Success);
            }
            SignalGenerator gen = new SignalGenerator(sampleRate, channels);
            SignalGeneratorType sigType = SignalGeneratorType.Sin;
            switch(toneType.ToUpper())
            {
                case "SINE":
                    sigType = SignalGeneratorType.Sin;
                    break;
                case "TRIANGLE":
                    sigType = SignalGeneratorType.Triangle;
                    break;
                case "SQUARE":
                    sigType = SignalGeneratorType.Square;
                    break;
                case "SAWTOOTH":
                    sigType = SignalGeneratorType.SawTooth;
                    break;
                case "PNOISE":
                    sigType = SignalGeneratorType.Pink;
                    break;
                case "WNOISE":
                    sigType = SignalGeneratorType.White;
                    break;
            }
            gen.Frequency = frequency;
            gen.Type = sigType;
            gen.Gain = gain;
            return new CommandResponse((int)CommandStatus.Success)
            {
                ReturnData = (duration > 0) ? gen.Take(TimeSpan.FromMilliseconds(duration)).ToWaveProvider() : gen.ToWaveProvider()
            };
        }
        [CommandHelp("Manipulates a chunk of audio.")]
        public CommandResponse manAudio(CommandRequest req, CommandPipeline pipe, ICommandEnvironment currentEnvironment)
        {
            double duration = 0;
            float leftVol = 1;
            float rightVol = 1;
            int semitones = 1;
            bool showHelp = false;
            bool takeAudio = false;
            bool toMono = false;
            bool toStereo = false;
            OptionSet set = new OptionSet()
                .Add("take|t", "Delete a section of audio.", v => takeAudio = true)
                .Add("mono|m", "Changes the audio to mono.", v => toMono = true)
                .Add("stereo|s", "Changes the audio to stereo.", v => toStereo = true)
                .Add("duration|d=", "The amount in milliseconds to take from the audio.", v =>
                {
                    if (double.TryParse(v, out double result))
                    {
                        duration = result;
                    }
                    else
                    {
                        currentEnvironment.WriteLine(new ConsoleText("Duration must be a number.") { TextColor = Color.Red });
                    }
                })
                .Add("help|?", "Shows the help screen.", v => showHelp = true)
                .Add("leftVol|l=", "The amount of volume on the left when converting to stereo", v =>
                {
                    if (float.TryParse(v, out float result))
                    {
                        leftVol = result;
                    }
                    else
                    {
                        currentEnvironment.WriteLine(new ConsoleText("Left volume must be a number.") { TextColor = Color.Red });
                    }
                })
                .Add("pitch|p=", "Raises or lowers the pitch of audio by x amount of semitones.", v =>
                {
                    if (int.TryParse(v, out int result))
                    {
                        semitones = result;
                    }
                    else
                    {
                        currentEnvironment.WriteLine(new ConsoleText("Pitch must be a number.") { TextColor = Color.Red });
                    }
                })
                .Add("rightVol|r=", "The amount of volume on the right when converting to stereo", v =>
                {
                    if (float.TryParse(v, out float result))
                    {
                        rightVol = result;
                    }
                    else
                    {
                        currentEnvironment.WriteLine(new ConsoleText("Right volume must be a number.") { TextColor = Color.Red });
                    }
                });
            set.Parse(req.Arguments.Select(e => e.ToString()));
            if (showHelp)
            {
                set.WriteOptionDescriptions(currentEnvironment.Out);
                currentEnvironment.WriteLine();
                return new CommandResponse((int)CommandStatus.Success);
            }
            if(req.HasLastCommand && req.LastCommand.ReturnData is IWaveProvider)
            {
                ISampleProvider sampleProvider = ((IWaveProvider)req.LastCommand.ReturnData).ToSampleProvider();
                if (toMono) sampleProvider = sampleProvider.ToMono();
                if (toStereo) sampleProvider = sampleProvider.ToStereo(leftVol, rightVol);
                if (takeAudio) sampleProvider = sampleProvider.Take(TimeSpan.FromMilliseconds(duration));
                return new CommandResponse((int)CommandStatus.Success)
                {
                    ReturnData = sampleProvider.ToWaveProvider()
                };
            }
            else
            {
                currentEnvironment.WriteLine(new ConsoleText("You must provide an audio sample provider from a previous command.") { TextColor = Color.Red });
                return new CommandResponse((int)CommandStatus.Fail);
            }
        }
        [CommandHelp("Stops a specific device from playing.")]
        public CommandResponse stopAudio(CommandRequest req, CommandPipeline pipe, ICommandEnvironment currentEnvironment)
        {
            if (req.Arguments.Count >= 2)
            {
                if (req.Arguments[1].IsOfType<ResourceQuery>())
                {
                    _resourceManager.GetResource<AudioOutDevice>((ResourceQuery)req.Arguments[1].Value).Stop();
                }
                else
                {
                    currentEnvironment.WriteLine(new ConsoleText("You must specify audio device.") { TextColor = Color.Red });
                    return new CommandResponse((int)CommandStatus.Fail);
                }
            }
            else
            {
                _resourceManager.GetAllResourcesByType<AudioOutDevice>("/dev/audio")
                    .Where(dev => dev.PlaybackState == NAudio.Wave.PlaybackState.Playing)
                    .ToList()
                    .ForEach(dev =>
                    {
                        try
                        {
                            dev.Stop();
                        }
                        catch (Exception ex)
                        {
                            currentEnvironment.WriteLine(new ConsoleText($"Device {dev.Name} failed to stop: {ex.Message}") { TextColor = Color.Red });
                        }
                    });
            }
            return new CommandResponse((int)CommandStatus.Success);
        }
        [CommandHelp("Sets the server audio to a specific percentage.")]
        public CommandResponse setGlobalVolume(CommandRequest args, CommandPipeline pipe, ICommandEnvironment currentEnvironment)
        {
            if (args.Arguments.Count < 2)
            {
                currentEnvironment.WriteLine(new ConsoleText("You must specify a percentage.") { TextColor = Color.Red });
                return new CommandResponse((int)CommandStatus.Fail);
            }
            else
            {
                if (int.TryParse(args.Arguments[1].ToString(), out int percent))
                {
                    CoreAudioDevice defaultPlaybackDevice = new CoreAudioController().DefaultPlaybackDevice;
                    defaultPlaybackDevice.Volume = percent;
                    return new CommandResponse((int)CommandStatus.Success);
                }
                else
                {
                    currentEnvironment.WriteLine(new ConsoleText("Given ToString() is invalid.") { TextColor = Color.Red });
                    return new CommandResponse((int)CommandStatus.Fail);
                }
            }
        }
        public override void InitializeServices(IServiceCollection services)
        {
            _service = services.GetService<IRemotePlusService<ServerRemoteInterface>>();
            _resourceManager = services.GetService<IResourceManager>();
            Commands.Add("setGlobalVolume", setGlobalVolume);
            Commands.Add("loadAudio", loadAudio);
            Commands.Add("playAudio", playAudio);
            Commands.Add("stopAudio", stopAudio);
            Commands.Add("oscAudio", oscAudio);
            Commands.Add("manAudio", manAudio);
        }
    }
}
