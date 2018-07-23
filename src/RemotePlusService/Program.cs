using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceProcess;

namespace RemotePlusService
{
    class Program : System.ServiceProcess.ServiceBase
    {
        static void Main(string[] args)
        {
            ServiceBase.Run(new Program());
        }
        protected override void OnStart(string[] args)
        {

            base.OnStart(args);
        }
    }
}
