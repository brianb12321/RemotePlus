using RemotePlusLibrary.Configuration;
using RemotePlusLibrary.Extension.Gui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusClient
{
    [DataContract]
    public class ClientSettings : IFileConfig
    {
        public const string CLIENT_SETTING_PATH = "Configurations\\CLient\\GlobalClientSettings.config";
        [DataMember]
        public Theme DefaultTheme { get; set; }
        [DataMember]
        public bool DisableCommandDownloadForConsole { get; set; } = false;
        public void Load()
        {
            var c = ConfigurationHelper<ClientSettings>.LoadConfig(CLIENT_SETTING_PATH, RemotePlusLibrary.Core.DefaultKnownTypeManager.GetKnownTypes(null));
            DefaultTheme = c.DefaultTheme;
            DisableCommandDownloadForConsole = c.DisableCommandDownloadForConsole;
        }

        public void Load(string fileName)
        {
            throw new NotImplementedException();
        }

        public void Save()
        {
            ConfigurationHelper<ClientSettings>.SaveConfig(this, CLIENT_SETTING_PATH, RemotePlusLibrary.Core.DefaultKnownTypeManager.GetKnownTypes(null));
        }

        public void Save(string fileName)
        {
            throw new NotImplementedException();
        }
    }
}
