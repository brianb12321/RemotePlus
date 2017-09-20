using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.Design;

namespace RemotePlusLibrary.Editors
{
    public class UserAccountEditor : CollectionEditor
    {
        public UserAccountEditor(Type type) : base(type)
        {
        }
        protected override string GetDisplayText(object value)
        {
            return base.GetDisplayText(((UserAccount)value).Credentials.Username);
        }
    }
}
