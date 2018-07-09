using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using RemotePlusLibrary.Configuration;
using RemotePlusLibrary.Configuration.StandordDataAccess;

namespace RemotePlusClient.CommonUI.Controls.FileBrowserHelpers
{
    [DataContract]
    public class FileAssociationSettings
    {
        [DataMember]
        public Dictionary<string, string> Associations { get; set; } = new Dictionary<string, string>();
        [IgnoreDataMember]
        public const string FILE_PATH = ConfigurationHelper.CLIENT_CONFIGURATION_PATH + "\\FileAssociations.xml";
        const string ICON_PATH = ConfigurationHelper.CLIENT_CONFIGURATION_PATH + "\\icons";
        void PreloadIcons()
        {
            Associations.Add(".exe", $"{ICON_PATH}\\exe.ico");
            Associations.Add(".dll", $"{ICON_PATH}\\dll.ico");
            Associations.Add(".bat", $"{ICON_PATH}\\bat.ico");
            Associations.Add(".txt", $"{ICON_PATH}\\txt.ico");
            Associations.Add(".doc", $"{ICON_PATH}\\doc.ico");
            Associations.Add(".docx", $"{ICON_PATH}\\doc.ico");
        }
    }
}