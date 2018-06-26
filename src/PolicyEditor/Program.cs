using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PolicyEditor
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            PolicyEditors.Add("Default", new EditPolicyDialogBox());
            PolicyEditors.Add("BiValue", new BiValuePolicyEditor());
            Application.Run(new PolicyManager());
        }
    }
}
