using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.Immutable;
using Prediction;
using YOLOv4MLNet.DataStructures;

namespace WpfApp
{
    public partial class ViewModel : INotifyPropertyChanged, IUIServices
    {
        public event PropertyChangedEventHandler PropertyChanged;
        ImmutableDictionary<string, int> numberOfClasses;
        ImmutableDictionary<string, List<string>> imagesOfClasses;
        ImmutableList<string> imagesInFolder;
        ImmutableList<YoloV4Result> results;
        ImmutableList<string> stringResults;
        string progress;

        public ViewModel()
        {
            imagesInFolder = ImmutableList<string>.Empty;
            results = ImmutableList<YoloV4Result>.Empty;
            numberOfClasses = ImmutableDictionary<string, int>.Empty;
            imagesOfClasses = ImmutableDictionary<string, List<string>>.Empty;
            progress = "Идёт загрузка модели, пожалуйста, подождите";
        }
        public string Progress
        {
            get => progress;
            set
            {
                progress = value;
                OnPropertyChanged(nameof(Progress));
            }
        }
        public ImmutableList<string> ImagesInFolder
        {
            get => imagesInFolder;
            set
            {
                imagesInFolder = value;
                OnPropertyChanged(nameof(ImagesInFolder));
            }
        }
        public ImmutableDictionary<string, int> NumberOfClasses
        {
            get => numberOfClasses;
            set
            {
                numberOfClasses = value;
                OnPropertyChanged(nameof(NumberOfClasses));
            }
        }
        public ImmutableDictionary<string, List<string>> ImagesOfClasses
        {
            get => imagesOfClasses;
            set
            {
                imagesOfClasses = value;
                OnPropertyChanged(nameof(ImagesOfClasses));
            }
        }
        public ImmutableList<YoloV4Result> Results
        {
            get => results;
            set
            {
                results = value;
                OnPropertyChanged(nameof(Results));
            }
        }
        public ImmutableList<string> StringResults
        {
            get => StringResults;
            set
            {
                stringResults = value;
                OnPropertyChanged(nameof(stringResults));
            }
        }

        public void AddImage(string path)
        {
            lock (imagesInFolder)
            {
                ImagesInFolder = imagesInFolder.Add(path);
            }
        }
        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }

}
