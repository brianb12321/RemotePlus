using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using RemotePlusLibrary.Configuration;

namespace RemotePlusClient.CommonUI
{
    [DataContract]
    public class FileAssociationSettings : IFileConfig
    {
        [DataMember]
        public Dictionary<string, string> Associations { get; set; } = new Dictionary<string, string>();
        [IgnoreDataMember]
        const string FILE_PATH = ConfigurationHelper<FileAssociationSettings>.CLIENT_CONFIGURATION_PATH + "\\FileAssociations.xml";
        const string ICON_PATH = ConfigurationHelper<FileAssociationSettings>.CLIENT_CONFIGURATION_PATH + "\\icons";
        public void Load()
        {
            try
            {
                FileAssociationSettings loadedSettings = ConfigurationHelper<FileAssociationSettings>.LoadConfig(FILE_PATH, null);
                this.Associations = loadedSettings.Associations;
            }
            catch (FileNotFoundException)
            {
                PreloadIcons();
                Save();
            }
        }

        public void Save()
        {
            ConfigurationHelper<FileAssociationSettings>.SaveConfig(this, FILE_PATH, null);
        }
        void PreloadIcons()
        {
            Associations.Add(".exe", $"{ICON_PATH}\\exe.ico");
            Associations.Add(".dll", $"{ICON_PATH}\\dll.ico");
            Associations.Add(".bat", $"{ICON_PATH}\\bat.ico");
            Associations.Add(".txt", $"{ICON_PATH}\\txt.ico");
            Associations.Add(".doc", $"{ICON_PATH}\\doc.ico");
            Associations.Add(".docx", $"{ICON_PATH}\\doc.ico");
        }

        public void Save(string fileName)
        {
            throw new NotImplementedException();
        }

        public void Load(string fileName)
        {
            throw new NotImplementedException();
        }
    }
}