using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RemotePlusLibrary
{
    public interface IDataRequest
    {
        Form RequestForm { get; }
        RawDataRequest RequestData(RequestBuilder builder);
    }
}