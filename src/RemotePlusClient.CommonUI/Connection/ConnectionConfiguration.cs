using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using RemotePlusLibrary;
using RemotePlusLibrary.Configuration;

namespace RemotePlusClient.CommonUI.Connection
{
    /// <summary>
    /// Provides the stored connection settings that will be used to connect to the server.
    /// </summary>
    [DataContract]
    public class ConnectionConfiguration : IFileConfig
    {
        public const string CONFIGURATION_NAME = ConfigurationHelper<ConnectionConfiguration>.CLIENT_CONFIGURATION_PATH + "\\Connections";
        [DataMember]
        public string ServerAddress { get; set; }
        [DataMember]
        public RegisterationObject RegisterationDetails { get; set; }
        [DataMember]
        public string ConfigurationFileName { get; set; }
        public ConnectionConfiguration(string configurationName)
        {
            ConfigurationFileName = configurationName;
            RegisterationDetails = new RegisterationObject();
        }
        public void Load()
        {
            var loadedData = ConfigurationHelper<ConnectionConfiguration>.LoadConfig(Path.Combine(CONFIGURATION_NAME, ConfigurationFileName), null);
            ServerAddress = loadedData.ServerAddress;
            RegisterationDetails = loadedData.RegisterationDetails;
        }

        public void Save()
        {
            ConfigurationHelper<ConnectionConfiguration>.SaveConfig(this, Path.Combine(ConnectionConfiguration.CONFIGURATION_NAME, ConfigurationFileName), null);
        }

        public void Save(string fileName)
        {
            ConfigurationHelper<ConnectionConfiguration>.SaveConfig(this, Path.IsPathRooted(fileName) ? fileName : Path.Combine(CONFIGURATION_NAME, ConfigurationFileName), null);
        }

        public void Load(string fileName)
        {
            ConfigurationHelper<ConnectionConfiguration>.SaveConfig(this, Path.IsPathRooted(fileName) ? fileName : Path.Combine(CONFIGURATION_NAME, ConfigurationFileName), null);
        }
    }
}
