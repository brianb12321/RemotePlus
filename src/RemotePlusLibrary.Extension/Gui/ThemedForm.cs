using System;
using System.Collections.Generic;
using System.Drawing;
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
        public virtual void ResizeForm(Size resizePoint)
        {
            this.Height = resizePoint.Height;
            this.Width = resizePoint.Width;
            foreach(Control c in Controls)
            {
                c.SuspendLayout();
                c.Height = resizePoint.Height;
                c.Width = resizePoint.Width;
                c.ResumeLayout();
            }
        }
    }
}
