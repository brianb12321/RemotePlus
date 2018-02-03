using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;

namespace Recon
{
    public class ControlWriter : TextWriter
    {
        private Control textbox;
        public ControlWriter(Control textbox)
        {
            this.textbox = textbox;
        }

        public override void Write(char value)
        {
            if (textbox.InvokeRequired)
            {
                textbox.Invoke((MethodInvoker)(() => textbox.Text += value));
            }
            else
            {
                textbox.Text += value;
            }
        }

        public override void Write(string value)
        {
            if (textbox.InvokeRequired)
            {
                textbox.Invoke((MethodInvoker)(() => textbox.Text += value));
            }
            else
            {
                textbox.Text += value;
            }
        }

        public override Encoding Encoding
        {
            get { return Encoding.ASCII; }
        }
    }
}
