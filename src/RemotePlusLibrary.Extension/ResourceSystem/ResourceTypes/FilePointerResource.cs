using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.Extension.ResourceSystem.ResourceTypes
{
    [DataContract]
    [Serializable]
    public class FilePointerResource : StreamResource
    {
        public FilePointerResource(string fileName, string id) : base(id, fileName)
        {
        }
        [DataMember]
        public override string ResourceType => "FilePointer";

        public override void Close()
        {
            Data.Close();
        }

        public override void Open()
        {
            Data = new FileStream(FileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            Length = Data.Length;
        }
    }
}