using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using RemotePlusLibrary.Configuration;
using System.Drawing;

namespace RemotePlusClient
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
        public void Load()
        {
            var settings = ConfigurationHelper.LoadConfig<ConsoleSettings>(CONSOLE_SETTINGS_PATH, null);
            DefaultErrorColor = settings.DefaultErrorColor;
            DefaultInfoColor = settings.DefaultInfoColor;
            DefaultWarningColor = settings.DefaultWarningColor;
            DefaultDebugColor = settings.DefaultDebugColor;
            DefaultBackColor = settings.DefaultBackColor;
        }

        public void Load(string fileName)
        {
            throw new NotImplementedException();
        }

        public void Save()
        {
            ConfigurationHelper.SaveConfig(this, CONSOLE_SETTINGS_PATH, null);
        }

        public void Save(string fileName)
        {
            throw new NotImplementedException();
        }
    }
}
