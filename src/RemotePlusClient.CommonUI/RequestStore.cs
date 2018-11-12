using RemotePlusClient.CommonUI.Requests;
using RemotePlusLibrary.RequestSystem;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RemotePlusClient.CommonUI
{
    public static class RequestStore
    {
        static IDataRequest current;
        static List<IDataRequest> requestForms = new List<IDataRequest>();
        public static void Init()
        {
            requestForms.Add(new RequestStringDialogBox());
            requestForms.Add(new ColorRequest());
            requestForms.Add(new MessageBoxRequest());
            requestForms.Add(new SelectLocalFileRequest());
        }
        public static void Add(IDataRequest requestForm)
        {
            requestForms.Add(requestForm);
        }
        public static List<IDataRequest> GetAll()
        {
            return requestForms;
        }
        public static IDataRequest Get(string name)
        {
            return requestForms.FirstOrDefault(r => r.URI.ToUpper() == name.ToUpper());
        }
        public static ReturnData Show(RequestBuilder builder)
        {
            try
            {
                var request = requestForms.FirstOrDefault(r => r.URI.ToUpper() == builder.Interface.ToUpper());
                if (request != null)
                {
                    current = request;
                    var rd = request.RequestData(builder);
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
        public static IDataRequest GetCurrent()
        {
            return current;
        }
    }
}