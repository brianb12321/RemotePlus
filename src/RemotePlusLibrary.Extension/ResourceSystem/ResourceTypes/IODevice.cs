using RemotePlusLibrary.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.Extension.ResourceSystem.ResourceTypes
{
    [DataContract]
    [Serializable]
    public abstract class IODevice : IOResource
    {
        [DataMember]
        public Dictionary<string, DeviceProperty> DeviceProperties { get; private set; } = new Dictionary<string, DeviceProperty>();
        [DataMember]
        public string Name => DeviceProperties["Name"].ToString();
        [DataMember]
        public string Description => DeviceProperties["Description"].ToString();
        [DataMember]
        public string DeviceID => DeviceProperties["DeviceID"].ToString();
        protected IODevice(string id, string name, string description, string deviceID) : base(id)
        {
            DeviceProperties.Add("Name", new DeviceProperty(true, false, "Name", name));
            DeviceProperties.Add("Description", new DeviceProperty(true, false, "Description", description));
            DeviceProperties.Add("DeviceID", new DeviceProperty(true, false, "DeviceID", deviceID));
        }

        public override string ResourceType => "IODevice";

        public override bool SaveToFile { get; set; } = false;
        /// <summary>
        /// Closes the IO Device when the process is finished using the device.
        /// </summary>
        public override abstract void Close();
        /// <summary>
        /// Opens the IO Device when the process begins.
        /// </summary>
        /// <returns></returns>
        public virtual bool Open()
        {
            return true;
        }
        /// <summary>
        /// Shuts down the device when the server closes.
        /// </summary>
        public virtual void Shutdown()
        {
            GlobalServices.Logger.Log("Shutting down device.", BetterLogger.LogLevel.Info, $"Dev {ResourceIdentifier}");
        }
        /// <summary>
        /// Prepares the device when the server opens.
        /// </summary>
        /// <returns></returns>
        public virtual bool Init()
        {
            GlobalServices.Logger.Log("Device intializing.", BetterLogger.LogLevel.Info, $"Dev {ResourceIdentifier}");
            return true;
        }

        public abstract override Stream OpenReadStream();

        public abstract override Stream OpenWriteStream();

        public override abstract int Read(byte[] buffer, int offset, int length);

        public override abstract void Write(byte[] data, int offset, int length);
        public override string ToString()
        {
            return DeviceID + " - " + Description;
        }
    }
}