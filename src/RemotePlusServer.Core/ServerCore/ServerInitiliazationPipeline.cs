using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusServer.Core.ServerCore
{
    /// <summary>
    /// Provides helper methods for initializing the server.
    /// </summary>
    public class ServerInitiliazationPipeline
    {
        public static bool LoadServerSettings { get; set; } = true;
        public static string ServerConfigPath { get; set; } = "Configurations\\Server\\Roles.config";
    }
}
