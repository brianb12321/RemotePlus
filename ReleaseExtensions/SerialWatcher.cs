using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RemotePlusLibrary.Extension.WatcherSystem;
using RemotePlusLibrary;
using System.IO.Ports;
using RemotePlusServer;
using System.Threading;
using Logging;

namespace ReleaseExtensions
{
    public class SerialWatcher : WatcherBase
    {
        bool StopFlag = false;
        public SerialWatcher() : base(new WatcherDetails("SerialWatcher", "1.0.0.0"))
        {
        }

        protected override void DoWork(object args)
        {
            try
            {
                ServerManager.Remote.Client.ClientCallback.TellMessageToServerConsole(new Logging.UILogItem(Logging.OutputLevel.Info, $"Listening on {args}", "SerialWatcher"));
                SerialPort sp = new SerialPort((string)args, 9600);
                sp.Open();
                while (!StopFlag)
                {
                    if (ServerManager.Remote.Client.ExtraData.TryGetValue("global_newLine", out string val))
                    {
                        if (val == "true")
                        {
                            ServerManager.Remote.Client.ClientCallback.TellMessageToServerConsole(new Logging.UILogItem(Logging.OutputLevel.Info, $"DATA: {sp.ReadLine()}{Environment.NewLine}", "SerialWatcher"));
                        }
                        else
                        {
                            ServerManager.Remote.Client.ClientCallback.TellMessageToServerConsole(new Logging.UILogItem(Logging.OutputLevel.Info, $"DATA: {sp.ReadLine()}", "SerialWatcher"));
                        }
                    }
                    else
                    {
                        ServerManager.Remote.Client.ClientCallback.TellMessageToServerConsole(new Logging.UILogItem(Logging.OutputLevel.Info, $"DATA: {sp.ReadLine()}", "SerialWatcher"));
                    }
                    Thread.Sleep(100);
                }
                sp.Close();
            }
            catch (Exception ex)
            {
                base.SetFatal();
                ServerManager.Remote.Client.ClientCallback.TellMessage(new UILogItem(OutputLevel.Error, $"{base.Details.Name} ran into an exception. " + ex.Message, "SerialWatcher"));
            }
        }

        protected override void Stop()
        {
            StopFlag = true;
        }
    }
}
