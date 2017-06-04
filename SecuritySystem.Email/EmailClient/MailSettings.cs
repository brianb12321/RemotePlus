using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SecuritySystem.Email.EmailClient
{
    [DataContract]
    public class MailSettings
    {
        [DataMember]
        public string SMTP { get; set; }
        [DataMember]
        public string UserName { get; set; }
        // DO NOT DO THIS! use an email that you don't care being hacked.
        [DataMember]
        public string Password { get; set; }
        [DataMember]
        public bool SSL { get; set; }
        [DataMember]
        public string From { get; set; }
        [DataMember]
        public bool Port { get; set; }
        [DataMember]
        public string To { get; set; }
        public void Save()
        {
            DataContractSerializer ds = new DataContractSerializer(typeof(MailSettings));
            var subReq = this;
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            using (var sww = new StringWriter())
            using (XmlWriter writer = XmlWriter.Create("Configurations\\Server\\MailSettings.config", settings))
            {
                ds.WriteObject(writer, subReq);
            }
        }
        public void Load()
        {
            DataContractSerializer ser = new DataContractSerializer(typeof(MailSettings));
            XmlReader reader = XmlReader.Create("Configurations\\Server\\MailSettings.config");
            var ss = (MailSettings)ser.ReadObject(reader);
            this.From = ss.From;
            this.Password = ss.Password;
            this.Port = ss.Port;
            this.SMTP = ss.SMTP;
            this.SSL = ss.SSL;
            this.UserName = ss.UserName;
            this.To = ss.To;
            reader.Close();
        }
    }
}
