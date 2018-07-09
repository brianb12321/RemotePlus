using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using NewRemotePlusClient.Models;
using RemotePlusLibrary.Scripting;

namespace NewRemotePlusClient.Views.Pages
{
    /// <summary>
    /// Interaction logic for ScriptingEditorView.xaml
    /// </summary>
    public partial class ScriptingEditorView : UserControl
    {
        public ScriptingEditorView()
        {
            InitializeComponent();
            DataContext = new ViewModels.Pages.ScriptingEditorViewModel();
            var xshd = HighlightingLoader.LoadXshd(System.Xml.XmlReader.Create("Resources\\Python.xshd"));
            var def = HighlightingLoader.Load(xshd, HighlightingManager.Instance);
            editor.SyntaxHighlighting = def;
        }
    }
}