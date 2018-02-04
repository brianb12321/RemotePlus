using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WindowsTools
{
    public static class Win32Wrapper
    {
        [DllImport("winmm.dll", EntryPoint = "mciSendString")]
        private static extern int mciSendStringA(string lpstrCommand, string lpstrReturnString, int uReturnLength, int hwndCallback);
        public static void OpenDiskDrive(string driveLetter, string returnString)
        {
            mciSendStringA("open " + driveLetter + ": type CDaudio alias drive" + driveLetter, returnString, 0, 0);
            mciSendStringA("set drive" + driveLetter + " door open", returnString, 0, 0);
        }
    }
}
