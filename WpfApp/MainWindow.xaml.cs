using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Prediction;

namespace WpfApp
{
    public partial class MainWindow : Window
    {
        public static RoutedCommand Start = new RoutedCommand("Start", typeof(MainWindow));
        bool CanStart = false;

        public MainWindow()
        {
            InitializeComponent();
            CanStart = true;
        }
        private void CanStartCommandHandler(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = CanStart;
        }
        private void StartCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {
            var w = new ResultWindow();
            w.Show();
            CanStart = false;
        }
    }
}
