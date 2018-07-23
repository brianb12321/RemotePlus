using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.FileTransfer.Service.PackageSystem
{
    /// <summary>
    /// Represents package that contains a file.
    /// </summary>
    public class FilePackage : Package, IDisposable
    {
        public string FileName { get; set; }
        public long Length { get; set; }
        public Stream Data { get; set; }
        public bool KeepAlive { get; set; }
        public virtual void Dispose()
        {
            if (Data != null)
            {
                Data.Close();
                Data = null;
            }
            GC.SuppressFinalize(this);
        }
    }
}