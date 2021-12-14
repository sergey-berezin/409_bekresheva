using System;
using System.Collections.Generic;
using System.Globalization;
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
using Prediction;

namespace WpfApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string model_path = @"E:\Library\Documents\GitHub\2021-autumn prac\409_bekresheva\Model ONNX\yolov4.onnx";
        ViewModel ViewModel;
        PredictionClass prediction;
        public static RoutedCommand ChangeInputFolder = new RoutedCommand("ChangeInputFolder", typeof(MainWindow));
        public static RoutedCommand ChangeModelPath = new RoutedCommand("ChangeModelPath", typeof(MainWindow));
        public static RoutedCommand StartCommand = new RoutedCommand("StartCommand", typeof(MainWindow));
        public static RoutedCommand ResetCommand = new RoutedCommand("ResetCommand", typeof(MainWindow));
        public static RoutedCommand StopCommand = new RoutedCommand("StopCommand", typeof(MainWindow));
        public MainWindow()
        {
            CultureInfo.CurrentCulture = new CultureInfo("ru-RU");
            ViewModel = new ViewModel();
            DataContext = ViewModel;
            InitializeComponent();
            prediction = new PredictionClass(model_path, ViewModel);
            prediction.Init();
            ViewModel.Progress = "Готово к работе!";
        }

    }
}
