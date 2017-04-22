using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RemotePlusLibrary.Extension
{
    public interface IClientExtension
    {
        Form ExtensionForm { get; }
        string FriendlyName { get; }
        void Init();
    }
}
