using System;
using System.IO;

namespace RemotePlusLibrary.VFS
{
    [Serializable]
    public abstract class BaseFile : IFile
    {
        DateTime _creationDate;
        DateTime _lastAccessed;
        DateTime _lastChanged;
        FileAttributes _fileAttrib = FileAttributes.Normal;
        public string Path { get; protected set; }
        public string Name { get; protected set; }
        public bool SaveToBackingStore { get; set; } = true;
        protected BaseFile(string path, string name)
        {
            Path = path;
            Name = name;
        }
        protected BaseFile()
        {
        }

        public DateTime GetCreationTime()
        {
            return _creationDate;
        }

        public FileAttributes GetFileAttributes()
        {
            return _fileAttrib;
        }

        public DateTime GetLastAccessed()
        {
            return _lastAccessed;
        }

        public DateTime GetLastChanged()
        {
            return _lastChanged;
        }

        public abstract Stream OpenReadStream();

        public abstract Stream OpenWriteStream();

        public virtual void SetFileAttributes(FileAttributes attributes)
        {
            _fileAttrib = attributes;
        }

        public void SetLastAccessed(DateTime time)
        {
            _lastAccessed = time;
        }

        public void SetLastChanged(DateTime time)
        {
            _lastChanged = time;
        }
        public virtual void Dispose()
        {

        }
    }
}