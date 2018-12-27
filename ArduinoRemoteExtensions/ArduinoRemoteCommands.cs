using RemotePlusLibrary.Extension.CommandSystem;
using RemotePlusLibrary.Extension.CommandSystem.CommandClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using BetterLogger;
using RemotePlusLibrary.ServiceArchitecture;
using RemotePlusServer.Core;
using RemotePlusLibrary.Core.EventSystem;
using System.Threading;
using RemotePlusLibrary.Discovery;
using Ninject;
using System.Drawing;
using RemotePlusLibrary.Discovery.Events;
using ArduinoRemoteExtensionsLib.Events;

namespace ArduinoRemoteExtensions
{
    public class ArduinoRemoteCommands : StandordCommandClass
    {
        private IRemotePlusService<ServerRemoteInterface> _service;
        private IEventBus _eventBus;
        private ILogFactory _logger;
        private SerialPort _serialPort = null;

        public ArduinoRemoteCommands(ILogFactory logger, IEventBus eventBus, IRemotePlusService<ServerRemoteInterface> service)
        {
            _logger = logger;
            _service = service;
            _eventBus = eventBus;
        }
        [CommandHelp("Publishes any Arduino (Serial console) activity to the event bus.")]
        public CommandResponse arduinoEvent(CommandRequest args, CommandPipeline pipe)
        {
            _serialPort = new SerialPort(args.Arguments[1].ToString(), 9600);
            _serialPort.DataReceived += (sender, e) =>
            {
                string data = _serialPort.ReadExisting();
                _eventBus.Publish(new ServerMessageEvent(ServerManager.ServerGuid, $"Data received from Arduino: {data}", sender));
                _eventBus.Publish(new ArduinoEvent(ServerManager.ServerGuid, data, sender));
            };
            _serialPort.Open();
            _service.RemoteInterface.Client.ClientCallback.TellMessageToServerConsole("Port opened.");
            return new CommandResponse((int)CommandStatus.Success);
        }
        [CommandHelp("Cancels the current arduino serial communication.")]
        public CommandResponse arduinoCancel(CommandRequest args, CommandPipeline pipe)
        {
            _serialPort.Close();
            _service.RemoteInterface.Client.ClientCallback.TellMessageToServerConsole("Communication closed.");
            return new CommandResponse((int)CommandStatus.Success);
        }
        [CommandHelp("Responds to the Arduino event by beeping.")]
        public CommandResponse arduinoBeep(CommandRequest args, CommandPipeline pipe)
        {
            _eventBus.Subscribe<ArduinoEvent>(e =>
            {
                _logger.Log($"Arduino wants me to beep. Value: {e.Message}", LogLevel.Info);
                Console.Beep(1000, 1000);
            });
            _service.RemoteInterface.Client.ClientCallback.TellMessageToServerConsole("Arduino event registired.");
            return new CommandResponse((int)CommandStatus.Success);
        }
        public override void AddCommands()
        {
            Commands.Add("arduinoEvent", arduinoEvent);
            Commands.Add("arduinoCancel", arduinoCancel);
            Commands.Add("arduinoBeep", arduinoBeep);
        }
    }
}