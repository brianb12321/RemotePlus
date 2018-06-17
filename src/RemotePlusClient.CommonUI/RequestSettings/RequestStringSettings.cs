using RemotePlusLibrary.Configuration;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusClient.CommonUI.RequestSettings
{
    [DataContract]
    public class RequestStringSettings : IFileConfig
    {
        public const string CONFIG_FILE_PATH = "Configurations\\Client\\RequestSettings\\RequestString.config";
        [DataMember]
        public Color BackgroundColor { get; set; }
        [DataMember]
        public string Prefix { get; set; }

        public void Load()
        {
            var ss = ConfigurationHelper<RequestStringSettings>.LoadConfig(CONFIG_FILE_PATH, RemotePlusLibrary.Core.DefaultKnownTypeManager.GetKnownTypes(null));
            this.BackgroundColor = ss.BackgroundColor;
            this.Prefix = ss.Prefix;
        }

        public void Load(string fileName)
        {
            throw new NotImplementedException();
        }

        public void Save()
        {
            ConfigurationHelper<RequestStringSettings>.SaveConfig(this, CONFIG_FILE_PATH, RemotePlusLibrary.Core.DefaultKnownTypeManager.GetKnownTypes(null));
        }

        public void Save(string fileName)
        {
            throw new NotImplementedException();
        }
    }
}
