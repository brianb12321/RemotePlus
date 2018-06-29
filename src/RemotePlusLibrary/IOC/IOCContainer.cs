using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary
{
    public static class IOCContainer
    {
        public static IKernel Kernel { get; set; } = new StandardKernel();
        /// <summary>
        /// Sets up all the necessary services that aren't WCF service specific. example: What kind of server to use.
        /// </summary>
        public static void Setup()
        {
            
        }
    }
}
