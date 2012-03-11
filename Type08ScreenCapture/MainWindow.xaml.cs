using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
// using System.Windows.Shapes;

using System.IO;
using Microsoft.VisualBasic.ApplicationServices;

namespace Type08ScreenCapture
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DataContext = new AssemblyInfo(System.Reflection.Assembly.GetExecutingAssembly());
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Visibility = System.Windows.Visibility.Hidden;
            e.Cancel = true;
        }

        private void buttonGoHomePage_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://daruyanagi.net/");
        }

        private void buttonOK_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
