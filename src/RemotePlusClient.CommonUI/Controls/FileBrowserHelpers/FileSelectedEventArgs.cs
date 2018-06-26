using System;

namespace RemotePlusClient.CommonUI.Controls.FileBrowserHelpers
{
    public class FileSelectedEventArgs : EventArgs
    {
        public string SelectedName { get; set; }
        public string Type { get; set; }
        public FileSelectedEventArgs(string t, string file)
        {
            Type = t;
            SelectedName = file;
        }
    }
}