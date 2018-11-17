using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.RequestSystem
{
    public class RawDataResponse : IGenericObject
    {
        public object RawData { get; private set; }
        public RequestState State { get; private set; }
        public object Data { get; set; }

        private RawDataResponse(object data, RequestState state)
        {
            RawData = data;
            State = state;
        }
        public static RawDataResponse Success(object data)
        {
            return new RawDataResponse(data, RequestState.OK);
        }
        public static RawDataResponse Cancel()
        {
            return new RawDataResponse(null, RequestState.Cancel);
        }
        public static RawDataResponse Cancel(object data)
        {
            return new RawDataResponse(data, RequestState.Cancel);
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