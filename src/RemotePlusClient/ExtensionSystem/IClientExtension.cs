using RemotePlusLibrary.Extension.Gui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RemotePlusLibrary.Extension;

namespace RemotePlusClient.ExtensionSystem
{
    public interface IClientExtension : IExtension<ClientExtensionDetails>
    {
        ThemedForm ExtensionForm { get; }
    }
}
