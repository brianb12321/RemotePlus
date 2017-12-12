using System;
using System.IO;
using System.Runtime.Serialization;

namespace RemotePlusLibrary.FileTransfer
{
    [DataContract]
    public class RemoteFile
    {
        [DataMember]
        public string FullName { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public DateTime FileCreated { get; set; }
        [DataMember]
        public DateTime LastAccessed { get; set; }
        public RemoteFile(string path, DateTime fc, DateTime la)
        {
            FullName = path;
            FileCreated = fc;
            LastAccessed = la;
            Name = Path.GetFileName(path);
        }
    }
}