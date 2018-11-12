using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.RequestSystem
{
    /// <summary>
    /// Provides a generic like API for data contracts. WCF does not support generics to be transmitted over the wire; therefore this API automatically casts objects by calling the <see cref="Resolve{TType}"/> or the <see cref="UnsafeResolve{TType}"/> methods.
    /// </summary>
    public interface IGenericObject
    {
        [DataMember]
        object Data { get; }
        TType Resolve<TType>() where TType : class;
        TType UnsafeResolve<TType>() where TType : class;
        void PutObject<TType>(TType obj) where TType : class;
    }
}