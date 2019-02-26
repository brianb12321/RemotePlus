﻿using RemotePlusLibrary.Configuration;
using RemotePlusLibrary.Core;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace RemotePlusLibrary.Configuration.ServerSettings
{
    /// <summary>
    /// Represents the configuration settings for the server.
    /// </summary>
    [Serializable]
    [DataContract]
    public class ServerSettings
    {
        #region Config
        [DataMember]
        public bool UseDefaultUserIfNoneExists { get; set; } = true;
        #endregion
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
        #region Optimization Settings

        #endregion
        public ServerSettings()
        {
            PortNumber = 9000;
            LoggingSettings = new LoggingSettings();
        }
    }
}