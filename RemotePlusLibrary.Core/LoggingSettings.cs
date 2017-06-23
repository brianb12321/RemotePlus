﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.Core
{
    [DataContract]
    public class LoggingSettings
    {
        public const string LOGGING_SETTINGS_CATEGORY_LOGGING = "Logging";
        [DataMember]
        [Category(LOGGING_SETTINGS_CATEGORY_LOGGING)]
        public bool LogOnShutdown { get; set; }
        [Category(LOGGING_SETTINGS_CATEGORY_LOGGING)]
        [DataMember]
        public char DateDelimiter { get; set; }
        [Category(LOGGING_SETTINGS_CATEGORY_LOGGING)]
        [DataMember]
        public char TimeDelimiter { get; set; }
        [DataMember]
        [Category(LOGGING_SETTINGS_CATEGORY_LOGGING)]
        public bool CleanLogFolder { get; set; }
        [Category(LOGGING_SETTINGS_CATEGORY_LOGGING)]
        [DataMember]
        public int LogFileCountThreashold { get; set; } = 10;
        public LoggingSettings()
        {
            LogOnShutdown = true;
            DateDelimiter = '-';
            TimeDelimiter = '-';
            CleanLogFolder = false;
            LogFileCountThreashold = 10;
        }
    }
}
