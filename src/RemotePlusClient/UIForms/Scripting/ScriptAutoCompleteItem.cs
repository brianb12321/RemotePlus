using AutocompleteMenuNS;
using RemotePlusLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RemotePlusLibrary.Scripting;

namespace RemotePlusClient.UIForms.Scripting
{
    public class ScriptAutoCompleteItem : AutocompleteItem
    {
        ScriptGlobalInformation Information { get; set; }
        public ScriptAutoCompleteItem(ScriptGlobalInformation info) : base(info.Name)
        {
            Information = info;
            ToolTipText = Information.Description;
            ToolTipTitle = info.Name + $" - {info.Type}";
            SelectIcon();
        }

        private void SelectIcon()
        {
            switch(Information.Type)
            {
                case ScriptGlobalType.Function:
                    ImageIndex = 0;
                    break;
                case ScriptGlobalType.Variable:
                    ImageIndex = 1;
                    break;
            }
        }
    }
}
