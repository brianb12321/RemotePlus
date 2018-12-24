using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.RequestSystem.DefaultUpdateRequestBuilders
{
    [DataContract]
    public class SelectFileUpdateBuilder : UpdateRequestBuilder
    {
        public SelectFileUpdateBuilder(int counter) : base("global_selectFile")
        {
            Counter = counter;
        }

        [DataMember]
        public int Counter { get; set; }

    }
}
