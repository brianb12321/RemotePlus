using Logging;
using RemotePlusClient.CommonUI;
using RemotePlusLibrary;
using RemotePlusLibrary.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RemotePlusClient
{
    public class ClientApp
    {
        public static CMDLogging Logger { get; private set; }
        [STAThread]
        static void Main(string[] args)
        {
            Logger = new CMDLogging()
            {
                DefaultFrom = "Client"
            };
            InitializeDefaultKnownTypes();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            RequestStore.Init();
            Application.Run(new MainF());
        }
        static void InitializeDefaultKnownTypes()
        {
            Logger.AddOutput("Initializing default known types.", OutputLevel.Info);
            DefaultKnownTypeManager.LoadDefaultTypes();
            DefaultKnownTypeManager.AddType(typeof(UserAccount));
        }
    }
}
