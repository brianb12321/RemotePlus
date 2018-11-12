using RemotePlusLibrary.Core;
using RemotePlusLibrary.RequestSystem.DefaultRequestOptions;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Windows.Forms;

namespace RemotePlusLibrary.RequestSystem
{
    /// <summary>
    /// Represents a request for information object.
    /// </summary>
    [DataContract]
    public class RequestBuilder : IGenericObject
    {
        /// <summary>
        /// The unique request interface (URI) string used for identifying a request interface to use.
        /// </summary>
        [DataMember]
        public string Interface { get; private set; }
        /// <summary>
        /// Sets how the client should acquire the data and provides settings for if there was en error during hhe process.
        /// </summary>
        [DataMember]
        public AcquisitionMode AcqMode { get; set; } = AcquisitionMode.None;
        [DataMember]
        public object Data { get; private set; }

        public RequestBuilder(string i)
        {
            Interface = i;
        }
        public static RequestBuilder RequestString()
        {
            return new RequestBuilder("r_string");
        }
        public static RequestBuilder RequestColor()
        {
            return new RequestBuilder("r_color");
        }
        public static RequestBuilder RequestFilePath()
        {
            return new RequestBuilder("r_selectLocalFile");
        }
        public static RequestBuilder RequestMessageBox()
        {
            return new RequestBuilder("r_messageBox");
        }
        public static RequestBuilder RequestMessageBox(string message, string capiton, MessageBoxButtons buttons, MessageBoxIcon icons)
        {
            var rb = RequestMessageBox();
            rb.PutObject(new MessageBoxRequestOptions()
            {
                Message = message,
                Caption = capiton,
                Buttons = buttons,
                Icons = icons
            });
            return rb;
        }
        public static RequestBuilder RequestFile()
        {
            return new RequestBuilder("global_selectFile");
        }
        public static RequestBuilder SendFilePackage(string localFilePath)
        {
            return new RequestBuilder("global_sendFilePackage");
        }
        public static RequestBuilder SendByteStreamPackage(string localFilePath)
        {
            return new RequestBuilder("global_sendByteStreamFilePackage");
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