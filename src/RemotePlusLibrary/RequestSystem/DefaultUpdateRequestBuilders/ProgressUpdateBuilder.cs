using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.RequestSystem.DefaultUpdateRequestBuilders
{
    [DataContract]
    public class ProgressUpdateBuilder : UpdateRequestBuilder
    {
        [DataMember]
        public int NewValue { get; set; }
        [DataMember]
        public string Text { get; set; }
        public ProgressUpdateBuilder(int value) : base("r_progress")
        {
            NewValue = value;
        }
    }
}