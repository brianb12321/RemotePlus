using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Drawing;

namespace RemotePlusLibrary.Extension.CommandSystem
{
    /// <summary>
    /// Represents a non-log entry text that will be displayed on the console.
    /// </summary>
    [DataContract]
    public class ConsoleText
    {
        [DataMember]
        public string Text { get; set; }
        [DataMember]
        public Color TextColor { get; set; }
        public ConsoleText(string text)
        {
            Text = text;
        }
        public override string ToString()
        {
            return Text;
        }
    }
}