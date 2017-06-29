using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RemotePlusLibrary.Extension.Gui
{
    public class ThemedForm : Form
    {
        protected virtual void InitializeTheme(Theme t)
        {

        }
        public DialogResult ShowDialogAndInitalizeTheme(Theme t)
        {
            if (t.ThemeEnabled)
            {
                InitializeTheme(t);
            }
            return base.ShowDialog();
        }
        public void ShowAndInitializeTheme(Theme t)
        {
            if (t.ThemeEnabled)
            {
                InitializeTheme(t);
            }
            base.Show();
        }
        public void ShowAndInitializeTheme(Theme t, IWin32Window parentWindow)
        {
            if (t.ThemeEnabled)
            {
                InitializeTheme(t);
            }
            base.Show(parentWindow);
        }
    }
}
