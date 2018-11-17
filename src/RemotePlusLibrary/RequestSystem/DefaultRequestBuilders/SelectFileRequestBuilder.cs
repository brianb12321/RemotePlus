using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.RequestSystem.DefaultRequestBuilders
{
    public class SelectFileRequestBuilder : RequestBuilder
    {
        public SelectFileRequestBuilder() : base("global_selectFile")
        {
        }
    }
}
