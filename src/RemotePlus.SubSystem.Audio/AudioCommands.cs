using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RemotePlusLibrary.Extension.CommandSystem;
using RemotePlusLibrary.Extension.CommandSystem.CommandClasses;
using RemotePlusLibrary.Extension.ResourceSystem;
using RemotePlusLibrary.Extension.ResourceSystem.ResourceTypes;
using RemotePlusLibrary.RequestSystem;
using RemotePlusLibrary.RequestSystem.DefaultRequestBuilders;
using RemotePlusLibrary.ServiceArchitecture;
using RemotePlusLibrary.SubSystem.Audio.OutDevices;
using RemotePlusServer.Core;

namespace RemotePlusLibrary.SubSystem.Audio
{
    public class AudioCommands : StandordCommandClass
    {
        IRemotePlusService<ServerRemoteInterface> _service;
        IResourceManager _resourceManager;
        public AudioCommands(IRemotePlusService<ServerRemoteInterface> service, IResourceManager resourceManager)
        {
            _service = service;
            _resourceManager = resourceManager;
        }
        [CommandHelp("Plays an audio file (wav) sent by the client.")]
        public CommandResponse playWavAudio(CommandRequest req, CommandPipeline pipe, ICommandEnvironment currentEnvironment)
        {
            bool removeResource = true;
            ResourceQuery query = null;
            if (req.Arguments.Count >= 2 && req.Arguments[1].IsOfType<ResourceQuery>())
            {
                query = (ResourceQuery)req.Arguments[1].Value;
                removeResource = false;
            }
            else
            {
                var requestPathBuilder = new FileDialogRequestBuilder()
                {
                    Title = "Select audio file.",
                    Filter = "Wav File (*.wav)|*.wav"
                };
                var path = _service.RemoteInterface.Client.ClientCallback.RequestInformation(requestPathBuilder);
                if (path.AcquisitionState == RequestState.OK)
                {
                    _service.RemoteInterface.Client.ClientCallback.RequestInformation(new SendLocalFileByteStreamRequestBuilder(Path.GetFileName(path.Data.ToString()), path.Data.ToString())
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
            AudioOutDevice device = null;
            if(req.Arguments.Count >= 3)
            {
                if(req.Arguments[2].IsOfType<ResourceQuery>())
                {
                    device = _resourceManager.GetResource<AudioOutDevice>((ResourceQuery)req.Arguments[2].Value);
                    
                }
                else
                {
                    currentEnvironment.WriteLine(new ConsoleText("You must specify a wave out device."));
                }
            }
            else
            {
                device = _resourceManager.GetResource<AudioOutDevice>(new ResourceQuery("/dev/audio/waveOutDefault", Guid.Empty));
            }
            device.InitProvider(new NAudio.Wave.WaveFileReader(audio.OpenReadStream()));
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
        public override void AddCommands()
        {
            Commands.Add("playWavAudio", playWavAudio);
            Commands.Add("stopAudio", stopAudio);
        }
    }
}
