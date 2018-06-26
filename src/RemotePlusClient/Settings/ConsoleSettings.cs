using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using RemotePlusLibrary.Configuration;
using System.Drawing;
using System.ComponentModel;

namespace RemotePlusClient.Settings
{
    [DataContract]
    public class ConsoleSettings : IFileConfig
    {
        public const string CONSOLE_SETTINGS_PATH = @"Configurations\Client\Console\ConsoleConfig.config";
        [DataMember]
        public Color DefaultErrorColor { get; set; } = Color.Red;
        [DataMember]
        public Color DefaultInfoColor { get; set; } = Color.White;
        [DataMember]
        public Color DefaultWarningColor { get; set; } = Color.OrangeRed;
        [DataMember]
        public Color DefaultDebugColor { get; set; } = Color.Blue;
        [DataMember]
        public Color DefaultBackColor { get; set; } = Color.Black;
        [DataMember]
        public string DefaultFont { get; set; }
        public void Load()
        {
            var settings = ConfigurationHelper<ConsoleSettings>.LoadConfig(CONSOLE_SETTINGS_PATH, null);
            if(string.IsNullOrEmpty(settings.DefaultFont))
            {
                Font f = new Font("Arial", 13);
                TypeConverter tc = TypeDescriptor.GetConverter(typeof(Font));
                DefaultFont = tc.ConvertToString(f);
                settings.DefaultFont = DefaultFont;
            }
            else
            {
                DefaultFont = settings.DefaultFont;
            }
            DefaultErrorColor = settings.DefaultErrorColor;
            DefaultInfoColor = settings.DefaultInfoColor;
            DefaultWarningColor = settings.DefaultWarningColor;
            DefaultDebugColor = settings.DefaultDebugColor;
            DefaultBackColor = settings.DefaultBackColor;
            DefaultFont = settings.DefaultFont;
        }

        public void Load(string fileName)
        {
            throw new NotImplementedException();
        }

        public void Save()
        {
            ConfigurationHelper<ConsoleSettings>.SaveConfig(this, CONSOLE_SETTINGS_PATH, null);
        }

        public void Save(string fileName)
        {
            throw new NotImplementedException();
        }
    }
}