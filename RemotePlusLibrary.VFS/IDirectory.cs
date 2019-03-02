using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.VFS
{
    public interface IDirectory : IFile
    {
        Dictionary<UPath, IFile> Files { get; }
        void AddChildFile<TFile>(string fileName, TFile file) where TFile : class, IFile;
        void AddChildDirectory<TDirectory>(string dirName, TDirectory file) where TDirectory : class, IDirectory;
        void DeleteChildDirectory(string dirName);
        void DeleteChildFile(string fileName);
        IFile GetChildFile(string fileName);
        IDirectory GetChildDirectory(string directoryName);
        bool HasChildFile(string fileName);
        bool HasChildDirectory(string directoryName);
    }
}