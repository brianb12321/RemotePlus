using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.VFS
{
    [Serializable]
    public class MemoryFileSystem : IFileSystem
    {
        IDirectory rootDirectory = null;
        public MemoryFileSystem(IDirectory root)
        {
            rootDirectory = root;
        }

        public void AddDirectory(UPath path)
        {
            getDirectoryRecursive(path.GetPathElements(), 0, rootDirectory).AddChildDirectory(path.GetLastElement(), new BaseDirectory(path.Path, path.GetLastElement()));
        }

        public void AddFile<TFile>(UPath path, TFile file) where TFile : class, IFile
        {
            getDirectoryRecursive(path.GetPathElements(), 0, rootDirectory).AddChildFile(path.GetLastElement(), file);
        }

        public void DeleteFile(UPath path)
        {
            getDirectoryRecursive(path.GetPathElements(), 0, rootDirectory).DeleteChildFile(path.GetLastElement());
        }

        public TFile GetFile<TFile>(UPath path) where TFile : class, IFile
        {
            return getDirectoryRecursive(path.GetPathElements(), 0, rootDirectory).GetChildFile(path.GetLastElement()) as TFile;
        }

        private IDirectory getDirectoryRecursive(string[] path, int pos, IDirectory currentDirectory)
        {
            //         0   1   2
            //path: $/dev/mnt/tcp
            if (path.Length == 1)
            {
                return currentDirectory.GetChildDirectory(path[0]);
            }
            else
            {
                if (currentDirectory.HasChildDirectory(path[pos]))
                {
                    IDirectory foundIt = currentDirectory.GetChildDirectory(path[pos]);
                    if (path.Length - 1 == pos)
                    {
                        return foundIt;
                    }
                    var foundResourceDir = getDirectoryRecursive(path, ++pos, foundIt);
                    if (foundResourceDir != null)
                    {
                        return foundResourceDir;
                    }
                    else
                    {
                        throw new KeyNotFoundException();
                    }
                }
                else
                {
                    throw new KeyNotFoundException();
                }
            }
        }
    }
}