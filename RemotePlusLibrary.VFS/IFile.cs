using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.VFS
{
    /// <summary>
    /// Represents a virtual file.
    /// </summary>
    public interface IFile : IDisposable
    {
        string Path { get; }
        string Name { get; }
        bool SaveToBackingStore { get; set; }
        DateTime GetLastAccessed();
        DateTime GetLastChanged();
        DateTime GetCreationTime();
        void SetLastAccessed(DateTime time);
        void SetLastChanged(DateTime time);
        FileAttributes GetFileAttributes();
        void SetFileAttributes(FileAttributes attributes);
        Stream OpenReadStream();
        Stream OpenWriteStream();
    }
}