using RemotePlusLibrary.Extension.Gui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RemotePlusLibrary.Extension.ExtensionTypes.ExtensionDetailTypes;

namespace RemotePlusLibrary.Extension.ExtensionTypes
{
    public interface IClientExtension : IExtension<ClientExtensionDetails>
    {
        ThemedForm ExtensionForm { get; }
    }
}
