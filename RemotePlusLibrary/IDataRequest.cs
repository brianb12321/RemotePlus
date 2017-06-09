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
        RawDataRequest RequestData(RequestBuilder builder);
        bool ShowProperties { get; }
        string FriendlyName { get; }
        string Description { get; }
        Form GetProperties();
        void SaveProperties(Form f);
    }
}