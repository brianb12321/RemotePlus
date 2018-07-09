using RemotePlusLibrary.Configuration;
using RemotePlusLibrary.Extension.Gui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusClient.Settings
{
    [DataContract]
    public class ClientSettings
    {
        public const string CLIENT_SETTING_PATH = "Configurations\\CLient\\GlobalClientSettings.config";
        [DataMember]
        public Theme DefaultTheme { get; set; }
        [DataMember]
        public bool DisableCommandDownloadForConsole { get; set; } = false;

        public void Load(string fileName)
        {
            throw new NotImplementedException();
        }

        public void Save(string fileName)
        {
            throw new NotImplementedException();
        }
    }
}
