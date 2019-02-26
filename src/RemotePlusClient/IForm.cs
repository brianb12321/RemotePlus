using System;
using System.Windows.Forms;

namespace RemotePlusClient
{
    public interface IForm<TForm> where TForm : Control
    {
        TForm Form { get; }
        Guid FormID { get; }
        string FormName { get; }
        object FormTag { get; set; }
        void CloseForm();
    }
}