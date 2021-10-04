using System;
using System.Collections.Immutable;
using PredictionViewModel;
using YOLOv4MLNet.DataStructures;

namespace ConsoleApp
{
    public class ConsoleUIServices : IUIServices
    {
        void IErrorInterface.ErrorFunc(string s)
        {
            Console.WriteLine($"Error: {s}");
        }

        string IInputInterface.GetPath()
        {
            //return @"E:\Library\Documents\GitHub\2021-autumn prac\409_bekresheva\Assets\Images";
            
            Console.WriteLine("Введите путь, по которому будет выполняться магия:");
            return Console.ReadLine();
            
        }

        void IOutputInterface.OutputFunc(ImmutableList<YoloV4Result> results_list)
        {
            foreach (var res in results_list)
            {
                Console.WriteLine(res.Label);
                //Console.WriteLine(res);
            }
        }

        void IOutputInterface.OutputFunc(string s)
        {
            Console.WriteLine($"Output: {s}");
        }
    }
}