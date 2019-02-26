using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RemotePlusClient.Utils
{
    public static class WinFormExtension
    {
        public static void InvokeEx(this Control control, Action a)
        {
            if(control.InvokeRequired)
            {
                control.Invoke(a);
            }
            else
            {
                a();
            }
        }
    }
}