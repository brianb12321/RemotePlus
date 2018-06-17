using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace RemotePlusLibrary.Configuration
{
    public static class ConfigurationHelper<T>
    {
        public const string SERVER_CONFIGURATION_PATH = "Configurations\\Server";
        public const string CLIENT_CONFIGURATION_PATH = "Configurations\\Client";
        public static void SaveConfig(T configType, string file, IEnumerable<Type> knownTypes)
        {
            DataContractSerializer xsSubmit = new DataContractSerializer(typeof(T), knownTypes);
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
        public static T LoadConfig(string file, IEnumerable<Type> knownTypes)
        {
            DataContractSerializer ser = new DataContractSerializer(typeof(T), knownTypes);
            XmlReader reader = XmlReader.Create(file);
            var ss = (T)ser.ReadObject(reader);
            reader.Close();
            return ss;
        }
    }
}
