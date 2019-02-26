using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusClient.DialogViewModels
{
    public class ConnectViewModel : BaseViewModel
    {
        public string ServerAddress { get; set; }
        public ConnectViewModel(string fn) : base(fn)
        {
        }
    }
}