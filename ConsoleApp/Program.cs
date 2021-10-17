using System;
using System.Threading.Tasks;
using Prediction;
using System.Diagnostics;

namespace ConsoleApp
{
    class Program
    {
        private static void StopPredicting(object sender, ConsoleCancelEventArgs args)
        {
            args.Cancel = true;
            viewModel.StopPrediction();
        }
        private static PredictionClass viewModel;
        static async Task Main()
        {
            //Добавляем обработчик ctrl+c
            Console.CancelKeyPress += new ConsoleCancelEventHandler(StopPredicting);

            Console.WriteLine("Для выхода нажмите Ctrl+C");

            string model_path = @"E:\Library\Documents\GitHub\2021-autumn prac\409_bekresheva\Model ONNX\yolov4.onnx";

            //Чтобы PredictionClass знал, что ему делать с результатами, передаём ему ui, и запускаем init
            ConsoleUIServices ui = new ConsoleUIServices();
            try
            {
                viewModel = new PredictionClass(model_path, ui);
            }
            catch (Exception )
            {
                Console.WriteLine("Я не знаю, в чём дело, но, скорее всего, проблемы с открытием файлов");
                Console.WriteLine("Убедитесь, что в директории находятся только файлы .jpg");
            }
            
            viewModel.Init();

            //Запускаем обработку
            Task Prediction = viewModel.StartPredictionAsync();
            try
            {
                await Prediction;
                if (!Prediction.IsCompleted)
                    throw new Exception("Task is not complited");
            }catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
            }
            return;
        }
    }
}
