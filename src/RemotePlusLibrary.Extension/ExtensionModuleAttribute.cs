using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.Extension
{
    [AttributeUsage(AttributeTargets.Class,
        AllowMultiple = false,
        Inherited = false)]
    public class ExtensionModuleAttribute : Attribute
    {
        public ExtensionModuleAttribute()
        {
        }
    }
}