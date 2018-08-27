using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WindowsTools
{
    public static class Win32Wrapper
    {
        [DllImport("winmm.dll", EntryPoint = "mciSendString", CharSet = CharSet.Unicode)]
        private static extern int mciSendStringA(string lpstrCommand, string lpstrReturnString, int uReturnLength, int hwndCallback);
        [DllImport("user32.dll", EntryPoint = "BlockInput")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool blockInput([MarshalAs(UnmanagedType.Bool)] bool fBlockIt);
        private const int APPCOMMAND_VOLUME_MUTE = 0x80000;
        private const int WM_APPCOMMAND = 0x319;
        [DllImport("user32.dll")]
        private static extern IntPtr SendMessageW(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam); 
        public static void OpenDiskDrive(string driveLetter, string returnString)
        {
            mciSendStringA("open " + driveLetter + ": type CDaudio alias drive" + driveLetter, returnString, 0, 0);
            mciSendStringA("set drive" + driveLetter + " door open", returnString, 0, 0);
        }
        public static void BlockInputForInterval(int interval)
        {
            try
            {
                blockInput(true);
                Thread.Sleep(interval);
            }
            finally
            {
                blockInput(false);
            }

        }
        public static void ToggleMute()
        {
            IntPtr currentHandle = Process.GetCurrentProcess().Handle;
            SendMessageW(currentHandle, WM_APPCOMMAND, currentHandle, (IntPtr)APPCOMMAND_VOLUME_MUTE);
        }
    }
}
