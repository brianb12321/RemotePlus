using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.RequestSystem
{
    [DataContract]
    public abstract class UpdateRequestBuilder
    {
        [DataMember]
        public string UpdateURI { get; set; }
        protected UpdateRequestBuilder(string uri)
        {
            UpdateURI = uri;
        }
    }
}