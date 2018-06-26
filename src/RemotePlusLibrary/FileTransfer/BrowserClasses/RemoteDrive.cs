using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.FileTransfer.BrowserClasses
{
    [DataContract]
    public class RemoteDrive : IDirectory
    {
        [DataMember]
        public List<RemoteDirectory> Directories { get; set; } = new List<RemoteDirectory>();
        [DataMember]
        public List<RemoteFile> Files { get; set; } = new List<RemoteFile>();
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string FullName{ get; set; }
        public RemoteDrive(List<RemoteDirectory> dirs, List<RemoteFile> files, string name, string label)
        {
            Directories = dirs;
            Files = files;
            Name = label;
            FullName = name;
        }
        public RemoteDrive(string name, string label)
        {
            Name = label;
            FullName = name;
        }

        public RemoteDirectory[] GetDirectories()
        {
            return Directories.ToArray();
        }
    }
}
