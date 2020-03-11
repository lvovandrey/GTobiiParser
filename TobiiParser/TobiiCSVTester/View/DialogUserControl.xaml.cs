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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TobiiCSVTester.View
{
    /// <summary>
    /// Логика взаимодействия для DialogUserControl.xaml
    /// </summary>
    public partial class DialogUserControl : UserControl
    {
        public DialogUserControl(string text)
        {
            InitializeComponent();
            TextBlock.Text = text;
        }
    }
}
