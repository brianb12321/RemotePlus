using BetterLogger;
using RemotePlusLibrary;
using RemotePlusLibrary.Core;
using RemotePlusLibrary.Extension.CommandSystem;
using RemotePlusLibrary.ServiceArchitecture;
using RemotePlusServer.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusServer.Core
{
    public class FileTransferService : StandordService<FileTransferServciceInterface>
    {
        public FileTransferService(Type implementation, Binding binding, string address) : base(typeof(RemotePlusLibrary.FileTransfer.Service.IFileTransferContract), implementation, binding, address)
        {

        }
    }
}