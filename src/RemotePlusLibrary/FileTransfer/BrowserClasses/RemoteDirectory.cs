using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.FileTransfer.BrowserClasses
{
    [DataContract]
    public class RemoteDirectory : IDirectory
    {
        [DataMember]
        public string FullName { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public DateTime LastAccessTime { get; set; }
        [DataMember]
        public List<RemoteDirectory> Directories { get; set; } = new List<RemoteDirectory>();
        [DataMember]
        public List<RemoteFile> Files { get; set; } = new List<RemoteFile>();

        public RemoteDirectory(string path, DateTime la)
        {
            FullName = path;
            LastAccessTime = la;
            Name = new DirectoryInfo(path).Name;
        }
        public RemoteDirectory[] GetDirectories()
        {
            return Directories.ToArray();
        }
    }
}