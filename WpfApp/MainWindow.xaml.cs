using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
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
using WpfApp.DBVM;

namespace WpfApp
{
    public partial class MainWindow : Window
    {
        public static RoutedCommand Start = new RoutedCommand("Start", typeof(MainWindow));
        public static RoutedCommand Update  = new RoutedCommand("Update", typeof(MainWindow));
        public static RoutedCommand ChangeMP = new RoutedCommand("ChangeMP", typeof(MainWindow));
        public static RoutedCommand DeleteOne = new RoutedCommand("DeleteOne", typeof(MainWindow));
        public static RoutedCommand DeleteAll = new RoutedCommand("DeleteAll", typeof(MainWindow));
        string modelPath = @"E:\Library\Documents\GitHub\2021-autumn prac\409_bekresheva\Model ONNX\yolov4.onnx";
        PicturesContext db;
        public string ModelPath { get => modelPath; }
        public bool CanExecute = false;
        public MainWindow()
        {
            InitializeComponent();
            db = new PicturesContext();
            var vm = db.Results.ToList(); //Local.ToObservableCollection
            Results.ItemsSource = vm;
            CanExecute = true;
        }
        private void CanExecuteCommandHandler(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = CanExecute;
        }
        private void CanDeleteCommandHandler(object sender, CanExecuteRoutedEventArgs e)
        {
            if (!CanExecute || Results.SelectedItem == null)
            {
                e.CanExecute = false;
                return;
            }
            e.CanExecute = true;
        }
        private void StartCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {
            CanExecute = false;
            var w = new ResultWindow(ModelPath);
            w.Owner = this;
            w.Show();
            w.Closed += update;
        }
        void update(object sender, EventArgs e)
        {
            lock (db)
            {
                var vm = db.Results.ToList();
                Results.ItemsSource = vm;
            }
            CanExecute = true;
        }
        private void UpdateCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {
            lock (db)
            {
                var vm = db.Results.ToList();
                Results.ItemsSource = vm;
            }
        }
        private void ChangeMPCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg_open = new Microsoft.Win32.OpenFileDialog();
            dlg_open.Filter = "ONNX documents|*.onnx";
            if (dlg_open.ShowDialog() == true)
                modelPath = dlg_open.FileName;
            else
                throw new Exception("Fatal Error!");
        }
        private void DeleteOneCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {
            lock (db)
            {
                db.Results.Remove((Result)Results.SelectedItem); db.SaveChanges();
                var vm = db.Results.ToList();
                Results.ItemsSource = vm;
            }
        }
        private void DeleteAllCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {
            lock (db)
            {
                db.Results.RemoveRange(db.Results);
                db.Pictures.RemoveRange(db.Pictures);
                db.SaveChanges();
                var vm = db.Results.ToList();
                Results.ItemsSource = vm;
            }
        }
        private void ResultViewSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Results.SelectedItem == null)
            {
                ResultImage.Source = null;
                return;
            }
            lock (db)
            {
                var t = db.Results.FindAsync(((Result)Results.SelectedItem).ResultId);
                var sel = t.Result;
                db.Entry(sel).Reference(r => r.Picture).Load();
                var imageData = ((Result)Results.SelectedItem).Picture.Photo;
                if (imageData == null || imageData.Length == 0) ResultImage.Source = null;
                var image = new BitmapImage();
                using (var mem = new MemoryStream(imageData))
                {
                    mem.Position = 0;
                    image.BeginInit();
                    image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.UriSource = null;
                    image.StreamSource = mem;
                    image.EndInit();
                }
                image.Freeze();
                ResultImage.Source = image;
            }

        }
    }
}
