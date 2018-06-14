using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.Editors.PolicyManagerClasses
{
    public interface IPolicyEditor
    {
        Dictionary<string, string> ShowDialog(PolicyView policy);
    }
}
