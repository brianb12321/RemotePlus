using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.Design;

namespace RemotePlusLibrary.Editors
{
    public class StringCollectionEditor : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            var editorService = provider.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;
            if(editorService != null)
            {
                var selectionControl = new ExtensionRulesStringCollectionEditorForm((List<string>)value);
                editorService.ShowDialog(selectionControl);
                if (selectionControl.DialogResult == System.Windows.Forms.DialogResult.OK)
                {
                    value = selectionControl.Value;
                }
            }
            return value ?? new string[] { };
        }
    }
}
