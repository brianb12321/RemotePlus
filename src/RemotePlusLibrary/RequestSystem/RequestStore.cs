using RemotePlusLibrary.Core;
using RemotePlusLibrary.RequestSystem;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RemotePlusLibrary.RequestSystem
{
    public static class RequestStore
    {
        static IRequestAdapter current;
        static List<IRequestAdapter> requestForms = new List<IRequestAdapter>();
        public static void Init()
        {

        }
        public static void Add<TBuilder, TUpdateBuilder>(IDataRequest<TBuilder, TUpdateBuilder> requestForm)
            where TBuilder : RequestBuilder
            where TUpdateBuilder : UpdateRequestBuilder
        {
            requestForms.Add(requestForm);
        }
        public static List<IRequestAdapter> GetAll()
        {
            return requestForms;
        }
        public static IRequestAdapter Get(string name)
        {
            return requestForms.FirstOrDefault(r => r.URI.ToUpper() == name.ToUpper());
        }
        public static ReturnData Show(Guid server, RequestBuilder builder)
        {
            try
            {
                builder.RequestingServer = server;
                var request = requestForms.FirstOrDefault(r => r.URI.ToUpper() == builder.Interface.ToUpper());
                if (request != null)
                {
                    current = request;
                    var rd = current.StartRequestData(builder, GlobalServices.RunningEnvironment.ExecutingSide);
                    if (rd.State == RequestState.OK)
                    {
                        return new ReturnData(rd.RawData, RequestState.OK);
                    }
                    else
                    {
                        if (builder.AcqMode == AcquisitionMode.ThrowIfCancel)
                        {
                            throw new RequestException($"Request {builder.Interface} canceled.");
                        }
                        else
                        {
                            return new ReturnData(null, RequestState.Cancel);
                        }
                    }
                }
                else
                {
                    if (builder.AcqMode == AcquisitionMode.ThrowIfNotFound)
                    {
                        throw new RequestException($"Request {builder.Interface} not found.");
                    }
                    else
                    {
                        return new ReturnData(null, RequestState.NotFound);
                    }
                }
            }
            catch (Exception e)
            {
                GlobalServices.Logger.Log($"An error occurred while processing a request: {e}", BetterLogger.LogLevel.Error);
                if (builder.AcqMode == AcquisitionMode.ThrowIfException)
                {
                    return new ReturnData(e, RequestState.Exception);
                    throw;
                }
                else
                {
                    return new ReturnData(e, RequestState.Exception);
                }
            }
        }
        public static void Update(Guid server, UpdateRequestBuilder message)
        {
            message.RequestingServer = server;
            current.StartUpdate(message);
        }
        public static IRequestAdapter GetCurrent()
        {
            return current;
        }
        public static void DisposeCurrentRequest()
        {
            if(current != null)
            {
                current.Dispose();
                current = null;
            }
        }
    }
}