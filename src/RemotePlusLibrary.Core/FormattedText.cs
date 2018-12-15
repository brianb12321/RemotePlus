﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Drawing;

namespace RemotePlusLibrary.Core
{
    /// <summary>
    /// Represents a non-log entry text that will be displayed on the console.
    /// </summary>
    [DataContract]
    public class FormattedText
    {
        [DataMember]
        public string Text { get; set; }
        [DataMember]
        public Color TextColor { get; set; }
        public FormattedText(string text)
        {
            Text = text;
        }
    }
}