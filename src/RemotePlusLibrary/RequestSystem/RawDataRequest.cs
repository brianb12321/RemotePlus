using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.RequestSystem
{
    public class RawDataRequest : IGenericObject
    {
        public object RawData { get; private set; }
        public RequestState State { get; private set; }
        public object Data { get; set; }

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

        public TType Resolve<TType>() where TType : class
        {
            return Data as TType;
        }

        public TType UnsafeResolve<TType>() where TType : class
        {
            return (TType)Data;
        }

        public void PutObject<TType>(TType obj) where TType : class
        {
            Data = obj;
        }
    }
}