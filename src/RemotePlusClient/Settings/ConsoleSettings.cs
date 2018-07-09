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
    public class ConsoleSettings
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
    }
}