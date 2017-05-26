using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.Configuration
{
    public interface IFileConfig
    {
        void Save();
        void Load();
    }
}
