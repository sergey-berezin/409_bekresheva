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
using Prediction;

namespace WpfApp
{
    /// <summary>
    /// Логика взаимодействия для ResultWindow.xaml
    /// </summary>
    public partial class ResultWindow : Window
    {
        string model_path = @"E:\Library\Documents\GitHub\2021-autumn prac\409_bekresheva\Model ONNX\yolov4.onnx";
        ViewModel ViewModel;
        PredictionClass prediction;
        public static RoutedCommand ChangeInputFolder = new RoutedCommand("ChangeInputFolder", typeof(ResultWindow));
        public static RoutedCommand ChangeModelPath = new RoutedCommand("ChangeModelPath", typeof(ResultWindow));
        public static RoutedCommand StartCommand = new RoutedCommand("StartCommand", typeof(ResultWindow));
        public static RoutedCommand ResetCommand = new RoutedCommand("ResetCommand", typeof(ResultWindow));
        public static RoutedCommand StopCommand = new RoutedCommand("StopCommand", typeof(ResultWindow));
        bool initialization;
        public ResultWindow()
        {
            ViewModel = new ViewModel();
            DataContext = ViewModel;
            ViewModel.Progress = "Пожалуйста, выберете файл с моделью или папку с картинками!";
            initialization = false;
            InitializeComponent();
        }
    }
}
