using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.Extension
{
    public interface ILibraryBuilder
    {
        string FriendlyName { get; }
        string Name { get; }
        string Version { get; }
        ExtensionLibraryType LibraryType { get; }
    }
}