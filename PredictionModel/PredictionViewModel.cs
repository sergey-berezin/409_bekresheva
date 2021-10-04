using System;
using System.IO;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using YOLOv4MLNet.DataStructures;
using System.Drawing;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Microsoft.ML;
using System.Collections.Generic;
using System.Linq;
using static Microsoft.ML.Transforms.Image.ImageResizingEstimator;
using System.Threading;

namespace PredictionViewModel
{

    public abstract class PredictionViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    public interface IOutputInterface
    {
        void OutputFunc(ImmutableList<YoloV4Result> results_list);
        void OutputFunc(string s);
    }
    public interface IInputInterface
    {
        string GetPath();
    }
    public interface IErrorInterface
    {
        void ErrorFunc(string s);
    }
    public interface IUIServices : IOutputInterface, IInputInterface, IErrorInterface { }
    public class PredictionViewModelClass: PredictionViewModelBase
    {
        private const float scoreThres = 0.3f;
        private const float iouThres = 0.7f;
        private string _modelPath;
        private ConcurrentBag<ImmutableList<YoloV4Result>> _allPredictionResults;
        private readonly IUIServices UI;
        private CancellationTokenSource _cancellationTokenSource;
        private CancellationToken _token;
        private string[] _image_names;
        public string[] image_names
        {
            get => _image_names;
        }
        private PredictionEngine<YoloV4BitmapData, YoloV4Prediction> _predictionEngine;
        public ConcurrentBag<ImmutableList<YoloV4Result>> AllPredictionResults
        {
            get => _allPredictionResults;
        }
        public PredictionViewModelClass(IUIServices _ui, string _mp)
        {
            _allPredictionResults = new ConcurrentBag<ImmutableList<YoloV4Result>>();
            OnPropertyChanged(nameof(AllPredictionResults));
            UI = _ui;
            _image_names = null;
            _modelPath = _mp;
            _cancellationTokenSource = new CancellationTokenSource();
            _token = _cancellationTokenSource.Token;
        }
        public void SetFolder()
        {
            string image_folder = UI.GetPath();
            _image_names = Directory.GetFiles(image_folder);
        }
        public void Init()
        {
            MLContext mlContext = new MLContext();
            var pipeline = mlContext.Transforms.ResizeImages(inputColumnName: "bitmap", outputColumnName: "input_1:0", imageWidth: 416, imageHeight: 416, resizing: ResizingKind.IsoPad)
                .Append(mlContext.Transforms.ExtractPixels(outputColumnName: "input_1:0", scaleImage: 1f / 255f, interleavePixelColors: true))
                .Append(mlContext.Transforms.ApplyOnnxModel(
                    shapeDictionary: new Dictionary<string, int[]>()
                    {
                        { "input_1:0", new[] { 1, 416, 416, 3 } },
                        { "Identity:0", new[] { 1, 52, 52, 3, 85 } },
                        { "Identity_1:0", new[] { 1, 26, 26, 3, 85 } },
                        { "Identity_2:0", new[] { 1, 13, 13, 3, 85 } },
                    },
                    inputColumnNames: new[]
                    {
                        "input_1:0"
                    },
                    outputColumnNames: new[]
                    {
                        "Identity:0",
                        "Identity_1:0",
                        "Identity_2:0"
                    },
                    modelFile: _modelPath, recursionLimit: 100));

            // Fit on empty list to obtain input data schema
            var model = pipeline.Fit(mlContext.Data.LoadFromEnumerable(new List<YoloV4BitmapData>()));
            _predictionEngine = mlContext.Model.CreatePredictionEngine<YoloV4BitmapData, YoloV4Prediction>(model);
        }
        public void Stop_Prediction() {
            _cancellationTokenSource.Cancel();
        }
        public void Output_One()
        {
            ImmutableList<YoloV4Result> result_list;
            if (AllPredictionResults.TryTake(out result_list))
                UI.OutputFunc(result_list);
            else
                UI.ErrorFunc("List is empty");
        }
        public void Predict_One(string image_name)
        {
            UI.OutputFunc($"Начало обработки изображения '{Path.GetFileName(image_name)}'...");
            try
            {
                var bitmap = new Bitmap(Image.FromFile(image_name));
                YoloV4Prediction predict;
                lock (_predictionEngine)
                {
                    predict = _predictionEngine.Predict(new YoloV4BitmapData() { Image = bitmap });
                }
                AllPredictionResults.Add(ImmutableList<YoloV4Result>.Empty
                    .AddRange(predict.GetResults(YoloV4Prediction.classesNames, scoreThres, iouThres)));
            }
            catch(Exception ex)
            {
                UI.ErrorFunc(ex.Message);
                return;
            }
            
            UI.OutputFunc($"Конец обработки изображения {Path.GetFileName(image_name)}.");
        }
        private ActionBlock<string> _predict_one_AB;
        public async void Wait_For_Finish_Prediction()
        {
            await _predict_one_AB.Completion;
        }
        public async void Start_Prediction() //< == не работает
        {
            _predict_one_AB = new ActionBlock<string>(async name => Predict_One(name),
                new ExecutionDataflowBlockOptions
                {
                    //CancellationToken = _token,
                    MaxDegreeOfParallelism = Environment.ProcessorCount
                }
            ) ;
            Parallel.ForEach(_image_names, name => _predict_one_AB.Post(name));
            _predict_one_AB.Complete();
            Thread.Sleep(100);
            await _predict_one_AB.Completion;
        }
    }
}
