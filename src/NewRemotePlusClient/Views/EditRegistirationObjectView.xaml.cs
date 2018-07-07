using NewRemotePlusClient.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace NewRemotePlusClient.Views
{
    /// <summary>
    /// Interaction logic for EditRegistirationObject.xaml
    /// </summary>
    public partial class EditRegistirationObject : Window, IWindow
    {
        public EditRegistirationObject()
        {
            InitializeComponent();
            DataContext = new ViewModels.EditRegistirationObjectViewModel();
        }
    }
}
