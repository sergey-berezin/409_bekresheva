using System;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using PredictionViewModel;


namespace ConsoleApp
{
    class Program
    {

        static async Task Main()
        {
            string model_path = @"E:\Library\Documents\GitHub\2021-autumn prac\409_bekresheva\Model ONNX\yolov4.onnx";
            var viewModel = new PredictionViewModelClass(new ConsoleUIServices(), model_path);
            Task SetFolder = Task.Run(() => viewModel.SetFolder());
            Task Init = Task.Run(() => viewModel.Init());
            Task.WaitAll(new Task[] { Init, SetFolder });

            var ab = new ActionBlock<string>(async name => viewModel.Predict_One(name),
                new ExecutionDataflowBlockOptions
                {
                    MaxDegreeOfParallelism = Environment.ProcessorCount
                }
            );
            Parallel.ForEach(viewModel.image_names, name => ab.Post(name));
            ab.Complete();
            await ab.Completion;
            //Ввод результата:
            //Parallel.ForEach(viewModel.image_names, _ => viewModel.Output_One());
        }
    }
}
