using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using RemotePlusLibrary.Core;

namespace RemotePlusLibrary.Configuration.StandordDataAccess
{
    public class ConfigurationHelper : IConfigurationDataAccess
    {
        public const string SERVER_CONFIGURATION_PATH = "Configurations\\Server";
        public const string CLIENT_CONFIGURATION_PATH = "Configurations\\Client";
        public void SaveConfig<TConfigModel>(TConfigModel configType, string file)
        {
            DataContractSerializer xsSubmit = new DataContractSerializer(typeof(TConfigModel), DefaultKnownTypeManager.GetKnownTypes(null));
            var subReq = configType;
            XmlWriterSettings settings = new XmlWriterSettings()
            {
                Indent = true,
                NamespaceHandling = NamespaceHandling.OmitDuplicates
            };
            using (var sww = new StringWriter())
            using (XmlWriter writer = XmlWriter.Create(file, settings))
            {
                xsSubmit.WriteObject(writer, subReq);
            }
        }
        public TConfigModel LoadConfig<TConfigModel>(string file)
        {
            return LoadConfig<TConfigModel>(new FileStream(file, FileMode.Open, FileAccess.Read));
        }

        public TConfigModel LoadConfig<TConfigModel>(Stream configStream)
        {
            DataContractSerializer ser = new DataContractSerializer(typeof(TConfigModel), DefaultKnownTypeManager.GetKnownTypes(null));
            XmlReader reader = XmlReader.Create(configStream);
            var ss = (TConfigModel)ser.ReadObject(reader);
            reader.Close();
            return ss;
        }
    }
}