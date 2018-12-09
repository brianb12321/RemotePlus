using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace RemotePlusLibrary.Extension.ResourceSystem.ResourceTypes
{
    [DataContract]
    public class FileResource : Resource
    {
        [DataMember]
        public string FilePath { get; set; }
        [IgnoreDataMember]
        public FileStream FileStream { get; set; }
        public FileResource(string id, string fp) : base(id)
        {
            FilePath = fp;
        }
        public void Open()
        {
            FileStream = new FileStream(FilePath, FileMode.Open, FileAccess.ReadWrite);
        }
        public void Close()
        {
            FileStream.Close();
        }
        public override string ToString()
        {
            return FilePath;
        }
    }
}