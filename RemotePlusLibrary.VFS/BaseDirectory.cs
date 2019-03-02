using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.VFS
{
    public class BaseDirectory : BaseFile, IDirectory
    {
        Dictionary<string, IFile> _files = new Dictionary<string, IFile>();
        Dictionary<string, IDirectory> _directories = new Dictionary<string, IDirectory>();
        public BaseDirectory(string path, string name) : base(path, name)
        {
        }
        public BaseDirectory(SerializationInfo info, StreamingContext context)
        {
            _directories = (Dictionary<string, IDirectory>)info.GetValue("_storedDirectories", typeof(Dictionary<string, IDirectory>));
            _files = (Dictionary<string, IFile>)info.GetValue("_storedFiles", typeof(Dictionary<string, IFile>));
            Name = (string)info.GetValue("Name", typeof(string));
            Path = (string)info.GetValue("Path", typeof(string));
        }
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            Dictionary<string, IFile> _temp = new Dictionary<string, IFile>();
            Dictionary<string, IDirectory> _tempDirs = new Dictionary<string, IDirectory>();
            foreach (IFile f in _files.Values)
            {
                if (f.SaveToBackingStore)
                {
                    _temp.Add(f.Path, f);
                }
            }
            info.AddValue("_storedFiles", _temp);
            foreach (IDirectory dir in _directories.Values)
            {
                if (dir.SaveToBackingStore)
                {
                    _tempDirs.Add(dir.Path, dir);
                }
            }
            info.AddValue("_storedDirectories", _tempDirs);
            info.AddValue("Path", Path);
            info.AddValue("Name", Name);
        }

        public Dictionary<UPath, IFile> Files { get; }

        public void DeleteChildDirectory(string dirName)
        {
            if (!HasChildDirectory(dirName)) throw new DirectoryNotFoundException();
            _directories.Remove(dirName);
        }

        public void DeleteChildFile(string fileName)
        {
            if (!HasChildFile(fileName)) throw new FileNotFoundException();
            _files.Remove(fileName);
        }

        public IDirectory GetChildDirectory(string directoryName)
        {
            if (!HasChildDirectory(directoryName)) throw new DirectoryNotFoundException();
            else return _directories[directoryName];
        }

        public IFile GetChildFile(string fileName)
        {
            if (!HasChildFile(fileName)) throw new System.IO.FileNotFoundException();
            else return _files[fileName];
        }

        public bool HasChildDirectory(string directoryName)
        {
            return _directories.ContainsKey(directoryName);
        }

        public bool HasChildFile(string fileName)
        {
            return _files.ContainsKey(fileName);
        }

        public override Stream OpenReadStream()
        {
            throw new NotImplementedException();
        }

        public override Stream OpenWriteStream()
        {
            throw new NotImplementedException();
        }

        void IDirectory.AddChildDirectory<TDirectory>(string dirName, TDirectory file)
        {
            _directories.Add(dirName, file);
        }

        void IDirectory.AddChildFile<TFile>(string fileName, TFile file)
        {
            _files.Add(fileName, file);
        }
    }
}