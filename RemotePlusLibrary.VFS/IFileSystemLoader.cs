using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.VFS
{
    public interface IFileSystemLoader
    {
        void Save(IFileSystem system);
        IFileSystem Load();
    }
}