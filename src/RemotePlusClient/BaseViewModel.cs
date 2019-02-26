using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusClient
{
    public abstract class BaseViewModel
    {
        public string FormName { get; set; }
        protected BaseViewModel(string fn)
        {
            FormName = fn;
        }
    }
}