using RemotePlusLibrary.Configuration;
using RemotePlusLibrary.Converters;
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
        #region Connection
        [DataMember]
        [Category("Server")]
        public int PortNumber { get; set; }
        [DataMember]
        [Category("Server")]
        public bool DisableCommandClients { get; set; } = false;
        #endregion
        #region Extension

        #endregion
        #region Security
        [DataMember]
        [Category("Security")]
        [Editor(typeof(UserAccountEditor), typeof(UITypeEditor))]
        public UserCollection Accounts { get; set; }
        [DataMember]
        [Browsable(false)]
        [Category("Security")]
        public StringCollection BannedIPs { get; set; }
        #endregion
        #region Server Info
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [Category("Server Information")]
        [XmlIgnore]
        public OperatingSystem ServerOS
        {
            get
            {
                return Environment.OSVersion;
            }
        }
        [Category("Server Information")]
        [XmlIgnore]
        public string ServerVersion
        {
            get
            {
                return Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
        }
        [Category("Server Information")]
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
        public bool LogOnShutdown { get; set; }
        #endregion Logging Settings
        #region Methods
        public void Save()
        {
            ConfigurationHelper.SaveConfig(this, "Configurations\\Server\\GlobalServerSettings.config", Core.DefaultKnownTypeManager.GetKnownTypes(null));
        }
        public void Load()
        {
            var ss = ConfigurationHelper.LoadConfig<ServerSettings>("Configurations\\Server\\GlobalServerSettings.config", Core.DefaultKnownTypeManager.GetKnownTypes(null));
            this.Accounts = ss.Accounts;
            this.BannedIPs = ss.BannedIPs;
            this.PortNumber = ss.PortNumber;
            this.LogOnShutdown = ss.LogOnShutdown;
            this.CleanLogFolder = ss.CleanLogFolder;
            this.LogFileCountThreashold = ss.LogFileCountThreashold;
            this.DisableCommandClients = ss.DisableCommandClients;
        }
        #endregion
        #region Optimization Settings
        [DataMember]
        public bool CleanLogFolder { get; set; }
        [DataMember]
        public int LogFileCountThreashold { get; set; } = 10;
        #endregion
        public ServerSettings()
        {
            PortNumber = 9000;
            Accounts = new UserCollection();
            Accounts.Add(new UserAccount(new UserCredentials("admin", "password"), new Role("Admin", new SecurityAccessRules())));
        }
    }
}