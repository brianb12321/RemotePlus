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
    public class FilePackage : StreamPackage
    {
        public string FileName { get; set; }
    }
}