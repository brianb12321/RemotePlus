using Logging;
using RemotePlusLibrary.Extension.WatcherSystem;
using RemotePlusServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace ReleaseExtensions
{
    public class HddWatcher : WatcherBase
    {
        public bool StopFlag = false;
        public HddWatcher() : base(new WatcherDetails("HddWatcher", "1.0.0.0"))
        {
        }

        protected override void Stop()
        {
            StopFlag = true;
        }

        protected override void DoWork(object args)
        {
            ServerManager.Remote.Remote.Client.ClientCallback.TellMessage(new UILogItem(OutputLevel.Info, "Your hard drive is now being checked.", Details.Name));
            int count = 0;
            try
            {
                ManagementClass mc = new ManagementClass("Win32_PerfFormattedData_PerfDisk_PhysicalDisk");
                while (!StopFlag)
                {
                    System.Threading.Thread.Sleep(200);
                    ManagementObjectCollection moc = mc.GetInstances();
                    foreach (ManagementObject obj in moc)
                    {
                        if (obj["Name"].ToString() == "_Total")
                        {
                            count++;
                            if (Convert.ToInt64(obj["DiskBytesPersec"]) > 0)
                            {
                                ServerManager.Remote.Remote.Client.ClientCallback.TellMessage(new UILogItem(Logging.OutputLevel.Info, $"hard drive is spinning. Count: {count}", Details.Name));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ServerManager.Remote.Remote.Client.ClientCallback.TellMessage(new UILogItem(Logging.OutputLevel.Error, $"Failed to start watcher {base.Details.Name}. Reason: {ex.Message}"));
                base.SetFatal();
            }
        }

        protected override void OnFail(AggregateException exception)
        {
            
        }
    }
}
