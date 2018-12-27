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
using ArduinoRemoteExtensions.Events;

namespace ArduinoRemoteExtensions
{
    public class ArduinoRemoteCommands : StandordCommandClass
    {
        private IRemotePlusService<ServerRemoteInterface> _service;
        private IEventBus _eventBus;
        public ArduinoRemoteCommands([Named("ProxyEventBus")] IEventBus eventBus, IRemotePlusService<ServerRemoteInterface> service)
        {
            _service = service;
            _eventBus = eventBus;
        }
        [CommandHelp("Publishes any Arduino (Serial console) activity to the event bus.")]
        public CommandResponse arduinoEvent(CommandRequest args, CommandPipeline pipe)
        {
            SerialPort sp = new SerialPort(args.Arguments[1].ToString());
            sp.Open();
            Task.Factory.StartNew(() =>
            {
                while(sp.IsOpen)
                {
                    string s = sp.ReadLine();
                    _eventBus.Publish(new ArduinoEvent(s, this));
                    Thread.Sleep(500);
                }
            });
            return new CommandResponse((int)CommandStatus.Success);
        }
        public override void AddCommands()
        {
            Commands.Add("arduinoEvent", arduinoEvent);
        }
    }
}