using RemotePlusLibrary;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RemotePlusClient.CommonUI
{
    public static class RequestDialogBoxStore
    {
        static Dictionary<string, IDataRequest> requestForms = new Dictionary<string, IDataRequest>();
        public static void Init()
        {
            requestForms.Add("r_string", new RequestStringDialogBox());
        }
        public static void Add(string name, IDataRequest requestForm)
        {
            requestForms.Add(name, requestForm);
        }
        public static IDataRequest Get(string name)
        {
            return requestForms[name];
        }
        public static ReturnData Show(RequestBuilder builder)
        {
            if (builder.Interface == "r_color")
            {
                ColorDialog cd = new ColorDialog();
                if (cd.ShowDialog() == DialogResult.OK)
                {
                    return new ReturnData(cd.Color.ToString(), RequestState.OK);
                }
                else
                {
                    if (builder.AcqMode == AcquisitionMode.ThrowIfCancel)
                    {
                        throw new RequestException($"Request {builder.Interface} canceled.");
                    }
                    else
                    {
                        return new ReturnData(Color.Black.ToString().ToString(), RequestState.Cancel);
                    }
                }
            }
            else
            {
                try
                {
                    var keyFound = requestForms.TryGetValue(builder.Interface, out IDataRequest val);
                    if (keyFound)
                    {
                        var rd = val.RequestData(builder);
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
                        if(builder.AcqMode == AcquisitionMode.ThrowIfNotFound)
                        {
                            throw new RequestException($"Request {builder.Interface} not found.");
                        }
                        else
                        {
                            return new ReturnData(null, RequestState.Exception);
                        }
                    }
                }
                catch
                {
                    if(builder.AcqMode == AcquisitionMode.ThrowIfException)
                    {
                        throw;
                    }
                    else
                    {
                        return new ReturnData(null, RequestState.Failed);
                    }
                }
            }
        }
    }
}