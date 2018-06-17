using RemotePlusLibrary.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.Core.EmailService
{
    [DataContract]
    public class EmailSettings : IFileConfig
    {
        public const string EMAIL_CONFIG_FILE = "Configurations\\Email\\DefaultEmailSettings.config";
        [DataMember]
        public string SMTPHost { get; set; }
        [DataMember]
        public string FromAddress { get; set; }
        [DataMember]
        public int Port { get; set; }
        [DataMember]
        public bool EnableSSL { get; set; }
        [DataMember]
        public string DefaultTo { get; set; }
        [DataMember]
        public string Username { get; set; }
        [DataMember]
        public string Password { get; set; }
        [DataMember]
        public int Timeout { get; set; }
        [DataMember]
        public AdvancedEmailSettings AdvancedSettings { get; set; }
        public EmailSettings()
        {
            SMTPHost = "SMTP.someserver.com";
            Port = 465;
            EnableSSL = true;
            DefaultTo = "billybobjones@someserver.com";
            Username = "bob@someserver.com";
            Password = "password123";
            FromAddress = "billybobjones@someserver.com";
            Timeout = 1000;
            AdvancedSettings = new AdvancedEmailSettings();
        }

        public void Load()
        {
            var s = ConfigurationHelper<EmailSettings>.LoadConfig(EMAIL_CONFIG_FILE, null);
            SMTPHost = s.SMTPHost;
            Port = s.Port;
            DefaultTo = s.DefaultTo;
            EnableSSL = s.EnableSSL;
            Username = s.Username;
            Password = s.Password;
            FromAddress = s.FromAddress;
            Timeout = s.Timeout;
            AdvancedSettings = s.AdvancedSettings;
        }

        public void Save()
        {
            ConfigurationHelper<EmailSettings>.SaveConfig(this, EMAIL_CONFIG_FILE, null);
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