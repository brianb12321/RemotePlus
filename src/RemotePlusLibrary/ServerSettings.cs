using RemotePlusLibrary.Configuration;
using RemotePlusLibrary.Core;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing.Design;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace RemotePlusLibrary
{
    /// <summary>
    /// Represents the configuration settings for the server.
    /// </summary>
    [Serializable]
    [DataContract]
    public class ServerSettings : IFileConfig
    {
        #region Constants
        /// <summary>
        /// The path to the server settings file.
        /// </summary>
        public const string SERVER_SETTINGS_FILE_PATH = "Configurations\\Server\\GlobalServerSettings.config";
        /// <summary>
        /// The category used for server settings.
        /// </summary>
        public const string SERVER_SETTINGS_CATEGORY_SERVER = "Server";
        /// <summary>
        /// The category used for security settings.
        /// </summary>
        public const string SERVER_SETTINGS_CATEGORY_SECURITY = "Security";
        /// <summary>
        /// The category used for information settings.
        /// </summary>
        public const string SERVER_SETTINGS_CATEGORY_SERVER_INFORMATION = "Server Information";
        #endregion Constants
        #region Connection
        /// <summary>
        /// The port number used for listening.
        /// </summary>
        [DataMember]
        [Category(SERVER_SETTINGS_CATEGORY_SERVER)]
        public int PortNumber { get; set; }
        [DataMember]
        [Category(SERVER_SETTINGS_CATEGORY_SERVER)]
        [Description("Determines whether to open an endpoint for service reference generation.")]
        public bool EnableMetadataExchange { get; set; }
        [DataMember]
        [Category(SERVER_SETTINGS_CATEGORY_SERVER)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public DiscoverySettings DiscoverySettings { get; set; } = new DiscoverySettings();
        #endregion
        #region Extension

        #endregion
        #region Security
        [DataMember]
        [Browsable(false)]
        [Category(SERVER_SETTINGS_CATEGORY_SECURITY)]
        public StringCollection BannedIPs { get; set; }
        #endregion
        #region Server Info
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [Category(SERVER_SETTINGS_CATEGORY_SERVER_INFORMATION)]
        [XmlIgnore]
        public OperatingSystem ServerOS
        {
            get
            {
                return Environment.OSVersion;
            }
        }
        [Category(SERVER_SETTINGS_CATEGORY_SERVER_INFORMATION)]
        [XmlIgnore]
        public string ServerVersion
        {
            get
            {
                return Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
        }
        [Category(SERVER_SETTINGS_CATEGORY_SERVER_INFORMATION)]
        [XmlIgnore]
        public string ServerFolder
        {
            get
            {
                return Environment.CurrentDirectory;
            }
        }

        #endregion
        #region Logging Settings
        /// <summary>
        /// Represents all the settings used by the logger.
        /// </summary>
        [DataMember]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public LoggingSettings LoggingSettings { get; set; }
        #endregion Logging Settings
        #region Methods
        public void Save()
        {
            ConfigurationHelper<ServerSettings>.SaveConfig(this, SERVER_SETTINGS_FILE_PATH, Core.DefaultKnownTypeManager.GetKnownTypes(null));
        }
        public void Load()
        {
            var ss = ConfigurationHelper<ServerSettings>.LoadConfig(SERVER_SETTINGS_FILE_PATH, Core.DefaultKnownTypeManager.GetKnownTypes(null));
            this.BannedIPs = ss.BannedIPs;
            this.PortNumber = ss.PortNumber;
            this.LoggingSettings = ss.LoggingSettings;
            this.EnableMetadataExchange = ss.EnableMetadataExchange;
            this.DiscoverySettings = ss.DiscoverySettings;
        }

        public void Save(string fileName)
        {
            throw new NotImplementedException();
        }

        public void Load(string fileName)
        {
            throw new NotImplementedException();
        }
        #endregion
        #region Optimization Settings

        #endregion
        public ServerSettings()
        {
            PortNumber = 9000;
            LoggingSettings = new LoggingSettings();
        }
    }
}