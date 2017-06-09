using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary
{
    public class RawDataRequest
    {
        public object RawData { get; private set; }
        public RequestState State { get; private set; }
        private RawDataRequest(object data, RequestState state)
        {
            RawData = data;
            State = state;
        }
        public static RawDataRequest Success(object data)
        {
            return new RawDataRequest(data, RequestState.OK);
        }
        public static RawDataRequest Cancel()
        {
            return new RawDataRequest(null, RequestState.Cancel);
        }
        public static RawDataRequest Cancel(object data)
        {
            return new RawDataRequest(data, RequestState.Cancel);
        }
    }
}
