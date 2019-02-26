using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RemotePlusClient
{
    public class DefaultWindowManager : IWindowManager
    {
        TabControl _left = null;
        TabControl _right = null;
        TabControl _up = null;
        TabControl _down = null;
        public DefaultWindowManager(TabControl u, TabControl d, TabControl l, TabControl r)
        {
            _left = l;
            _right = r;
            _up = u;
            _down = d;
        }
        public void Close<TViewModel>(ITabbedForm<TViewModel> form) where TViewModel : BaseViewModel
        {
            performOnControl(form.Location, (control) =>
            {
                if(control.TabPages.ContainsKey(form.FormID.ToString()))
                {
                    ((ITabbedForm<TViewModel>)control.TabPages[form.FormID.ToString()].Tag).CloseForm();
                    control.TabPages.RemoveByKey(form.FormID.ToString());
                }
            });
        }

        public void Close<TViewModel>(FormPosition pos, Guid id) where TViewModel : BaseViewModel
        {
            performOnControl(pos, (control) =>
            {
                if (control.TabPages.ContainsKey(id.ToString()))
                {
                    ((ITabbedForm<TViewModel>)control.TabPages[id.ToString()].Tag).CloseForm();
                    control.TabPages.RemoveByKey(id.ToString());
                }
            });
        }

        public ITabbedForm<TViewModel> Get<TViewModel>(FormPosition pos, Guid id) where TViewModel : BaseViewModel
        {
            ITabbedForm<TViewModel> result = null;
            performOnControl(pos, (control) =>
            {
                result = (ITabbedForm<TViewModel>)control.TabPages[id.ToString()].Tag;
            });
            return result;
        }

        public IEnumerable<ITabbedForm<TViewModel>> GetAllByID<TViewModel>(Guid id) where TViewModel : BaseViewModel
        {
            List<ITabbedForm<TViewModel>> _results = new List<ITabbedForm<TViewModel>>();
            performOnControl(FormPosition.Up, control =>
            {
                _results.AddRange(control.TabPages
                .Cast<TabPage>()
                .Where(t => t.Tag is ITabbedForm<TViewModel>)
                .Select(t => t.Tag as ITabbedForm<TViewModel>)
                .Where(t => t.FormID == id)
                .ToList());
            });
            performOnControl(FormPosition.Down, control =>
            {
                _results.AddRange(control.TabPages
                .Cast<TabPage>()
                .Where(t => t.Tag is ITabbedForm<TViewModel>)
                .Select(t => t.Tag as ITabbedForm<TViewModel>)
                .Where(t => t.FormID == id)
                .ToList());
            });
            performOnControl(FormPosition.Left, control =>
            {
                _results.AddRange(control.TabPages
                .Cast<TabPage>()
                .Where(t => t.Tag is ITabbedForm<TViewModel>)
                .Select(t => t.Tag as ITabbedForm<TViewModel>)
                .Where(t => t.FormID == id)
                .ToList());
            });
            performOnControl(FormPosition.Right, control =>
            {
                _results.AddRange(control.TabPages
                .Cast<TabPage>()
                .Where(t => t.Tag is ITabbedForm<TViewModel>)
                .Select(t => t.Tag as ITabbedForm<TViewModel>)
                .Where(t => t.FormID == id)
                .ToList());
            });
            return _results;
        }

        public IEnumerable<ITabbedForm<TViewModel>> GetAllByKind<TViewModel>() where TViewModel : BaseViewModel
        {
            List<ITabbedForm<TViewModel>> _results = new List<ITabbedForm<TViewModel>>();
            performOnControl(FormPosition.Up, control =>
            {
                _results.AddRange(control.TabPages
                .Cast<TabPage>()
                .Where(t => t.Tag is ITabbedForm<TViewModel>)
                .Select(t => t.Tag as ITabbedForm<TViewModel>)
                .ToList());
            });
            performOnControl(FormPosition.Down, control =>
            {
                _results.AddRange(control.TabPages
                .Cast<TabPage>()
                .Where(t => t.Tag is ITabbedForm<TViewModel>)
                .Select(t => t.Tag as ITabbedForm<TViewModel>)
                .ToList());
            });
            performOnControl(FormPosition.Left, control =>
            {
                _results.AddRange(control.TabPages
                .Cast<TabPage>()
                .Where(t => t.Tag is ITabbedForm<TViewModel>)
                .Select(t => t.Tag as ITabbedForm<TViewModel>)
                .ToList());
            });
            performOnControl(FormPosition.Right, control =>
            {
                _results.AddRange(control.TabPages
                .Cast<TabPage>()
                .Where(t => t.Tag is ITabbedForm<TViewModel>)
                .Select(t => t.Tag as ITabbedForm<TViewModel>)
                .ToList());
            });
            return _results;
        }

        public void Open<TViewModel>(ITabbedForm<TViewModel> form, object args) where TViewModel : BaseViewModel
        {
            performOnControl(form.Location, (control) =>
            {
                TabPage tp = new TabPage();
                tp.Text = form.FormName;
                tp.Name = form.FormID.ToString();
                form.Form.Dock = DockStyle.Fill;
                form.Form.Parent = tp;
                form.Form.Visible = true;
                tp.Controls.Add(form.Form);
                tp.Tag = form;
                control.TabPages.Add(tp);
            });
        }
        private void performOnControl(FormPosition f, Action<TabControl> t)
        {
            switch (f)
            {
                case FormPosition.Up:
                    if(_up != null)
                    {
                        t(_up);
                    }
                    break;
                case FormPosition.Down:
                    if(_down != null)
                    {
                        t(_down);
                    }
                    break;
                case FormPosition.Left:
                    if(_left != null)
                    {
                        t(_left);
                    }
                    break;
                case FormPosition.Right:
                    if(_right != null)
                    {
                        t(_right);
                    }
                    break;
            }
        }
    }
}