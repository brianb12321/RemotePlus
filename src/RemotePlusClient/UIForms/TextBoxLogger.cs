using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BetterLogger;

namespace RemotePlusClient.UIForms
{
    public class TextBoxLogger : ILogger
    {
        public Color WarningColor { get; set; } = Color.Orange;
        public Color ErrorColor { get; set; } = Color.Red;
        public Color DebugColor { get; set; } = Color.Blue;
        public Color VerboseColor { get; set; } = Color.Green;
        public Color InfoColor { get; set; }
        private RichTextBox _textBox = null;
        public TextBoxLogger(RichTextBox tb)
        {
            _textBox = tb;
            InfoColor = _textBox.ForeColor;
        }
        public void Log(string message, LogLevel level)
        {
            //TODO: Add custom color
            _textBox.AppendText(message + Environment.NewLine);
        }
    }
}
