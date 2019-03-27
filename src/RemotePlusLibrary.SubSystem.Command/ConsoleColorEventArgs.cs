using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.SubSystem.Command
{
    public class ConsoleColorEventArgs : EventArgs
    {
        public Color TextColor { get; private set; }
        public ConsoleColorEventArgs(Color color)
        {
            TextColor = color;
        }
    }
}