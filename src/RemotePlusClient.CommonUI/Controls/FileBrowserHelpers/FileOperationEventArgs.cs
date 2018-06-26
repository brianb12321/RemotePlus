using System;
using System.Windows.Forms;

namespace RemotePlusClient.CommonUI.Controls.FileBrowserHelpers
{
    public class EntryOperationEventArgs : EventArgs
    {
        public EntryOperationEventArgs(ListViewItem entry, FileOperation operation)
        {
            Entry = entry;
            Operation = operation;
        }
        public ListViewItem Entry { get; private set; }
        public FileOperation Operation { get; private set; }
        public bool IsDirectory { get; set; }
        public string Path { get; set; }
    }
}

namespace RemotePlusClient.CommonUI
{
    public enum FileOperation
    {
        Add,
        Delete
    }
}