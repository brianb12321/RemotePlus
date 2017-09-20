using RemotePlusLibrary.Configuration;
using RemotePlusLibrary.Converters;
using RemotePlusLibrary.Core;
using RemotePlusLibrary.Editors;
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
    [Serializable]
    [DataContract]
    public class ServerSettings : IFileConfig
    {
        #region Constants
        public const string SERVER_SETTINGS_FILE_PATH = "Configurations\\Server\\GlobalServerSettings.config";
        public const string SERVER_SETTINGS_CATEGORY_SERVER = "Server";
        public const string SERVER_SETTINGS_CATEGORY_SECURITY = "Security";
        public const string SERVER_SETTINGS_CATEGORY_SERVER_INFORMATION = "Server Information";
        #endregion Constants
        #region Connection
        [DataMember]
        [Category(SERVER_SETTINGS_CATEGORY_SERVER)]
        public int PortNumber { get; set; }
        #endregion
        #region Extension

        #endregion
        #region Security
        [DataMember]
        [Category(SERVER_SETTINGS_CATEGORY_SECURITY)]
        [Editor(typeof(UserAccountEditor), typeof(UITypeEditor))]
        public UserCollection Accounts { get; set; }
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
        [DataMember]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public LoggingSettings LoggingSettings { get; set; }
        #endregion Logging Settings
        #region Methods
        public void Save()
        {
            ConfigurationHelper.SaveConfig(this, SERVER_SETTINGS_FILE_PATH, Core.DefaultKnownTypeManager.GetKnownTypes(null));
        }
        public void Load()
        {
            var ss = ConfigurationHelper.LoadConfig<ServerSettings>(SERVER_SETTINGS_FILE_PATH, Core.DefaultKnownTypeManager.GetKnownTypes(null));
            this.Accounts = ss.Accounts;
            this.BannedIPs = ss.BannedIPs;
            this.PortNumber = ss.PortNumber;
            this.LoggingSettings = ss.LoggingSettings;
        }
        #endregion
        #region Optimization Settings

        #endregion
        public ServerSettings()
        {
            PortNumber = 9000;
            LoggingSettings = new LoggingSettings();
            Accounts = new UserCollection();
            Accounts.Add(new UserAccount(new UserCredentials("admin", "password"), new Role("Admin", new SecurityAccessRules())));
        }
    }
}