using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolicyEditor
{
    public interface IPolicyEditor
    {
        Dictionary<string, string> ShowDialog(PolicyView policy);
    }
}
