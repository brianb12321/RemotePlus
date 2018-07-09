using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using RemotePlusLibrary;
using RemotePlusLibrary.Configuration.StandordDataAccess;

namespace RemotePlusClient.CommonUI.Connection
{
    /// <summary>
    /// Provides the stored connection settings that will be used to connect to the server.
    /// </summary>
    [DataContract]
    public class ConnectionConfiguration
    {
        public const string CONFIGURATION_NAME = ConfigurationHelper.CLIENT_CONFIGURATION_PATH + "\\Connections";
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
    }
}
