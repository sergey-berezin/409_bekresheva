using System;
using System.IO;
using YOLOv4MLNet.DataStructures;
using System.Drawing;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Microsoft.ML;
using System.Collections.Generic;
using System.Linq;
using static Microsoft.ML.Transforms.Image.ImageResizingEstimator;
using System.Threading;

namespace Prediction
{
    public interface IOutputInterface
    {
        void ResultFunc(YoloV4Result res, string image_name);
        void OutputFunc(string res);
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
    public class PredictionClass
    {
        private const float scoreThres = 0.3f;
        private const float iouThres = 0.7f;
        private string _modelPath;
        private string[] _image_names;
        private CancellationTokenSource _cancellationTokenSource;
        private CancellationToken _token;

        private ActionBlock<string> _prediction;
        private ActionBlock<YoloV4Result> _handleOneAB;

        private PredictionEngine<YoloV4BitmapData, YoloV4Prediction> _predictionEngine;

        private IUIServices UI;
        private bool _inProgress;
        public bool InProgress
        {
            get => _inProgress;
            set
            {
                _inProgress = value;
            }
        }
        public PredictionClass(string _mp, IUIServices ui)
        {
            _image_names = null;
            _modelPath = _mp;
            _cancellationTokenSource = new CancellationTokenSource();
            _token = _cancellationTokenSource.Token;
            _inProgress = false;
            UI = ui;
            
            _prediction = new ActionBlock<string>(name => PredictOneAsync(name),
                new ExecutionDataflowBlockOptions
                {
                    CancellationToken = _token,
                    MaxDegreeOfParallelism = Environment.ProcessorCount
                }
            );
        }

        public void Init()
        {
            _image_names = Directory.GetFiles(UI.GetPath());
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

            var model = pipeline.Fit(mlContext.Data.LoadFromEnumerable(new List<YoloV4BitmapData>()));
            _predictionEngine = mlContext.Model.CreatePredictionEngine<YoloV4BitmapData, YoloV4Prediction>(model);
        }
        public void StopPrediction() {
            _cancellationTokenSource.Cancel();
            _inProgress = false;
        }

        public void PredictOneAsync(string image_name)
        {
            
            UI.OutputFunc($"Начало обработки изображения '{Path.GetFileName(image_name)}'...");
            try
            {
                var bitmap = new Bitmap(Image.FromFile(image_name));
                if (_token.IsCancellationRequested)
                    return;
                YoloV4Prediction predict;
                lock (_predictionEngine)
                {
                    predict = _predictionEngine.Predict(new YoloV4BitmapData() { Image = bitmap });
                }
                if (_token.IsCancellationRequested)
                    return;
                _handleOneAB = new ActionBlock<YoloV4Result>((res) => {
                    //Записать результат, если нужно

                    //Вывести результат на экран:
                    UI.ResultFunc(res, image_name);
                }, new ExecutionDataflowBlockOptions
                {
                    MaxDegreeOfParallelism = Environment.ProcessorCount
                });
                predict.GetResultsAsync(_handleOneAB, YoloV4Prediction.classesNames, scoreThres, iouThres);
            }
            catch (Exception ex)
            {
                UI.ErrorFunc(ex.Message);
                return;
            }

            UI.OutputFunc($"Конец обработки изображения {Path.GetFileName(image_name)}.");
        }

        public Task StartPredictionAsync()
        {
            _inProgress = true;
            Parallel.ForEach(_image_names, name => _prediction.SendAsync(name));
            _prediction.Complete();
            return _prediction.Completion;
        }

    }
}
