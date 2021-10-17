using System;
using Prediction;
using YOLOv4MLNet.DataStructures;

namespace ConsoleApp
{
    public class ConsoleUIServices : IUIServices
    {
        string IInputInterface.GetPath()
        {
            //return @"E:\Library\Documents\GitHub\2021-autumn prac\409_bekresheva\Assets\Images";
            
            Console.WriteLine("Введите путь, по которому будет выполняться магия:");
            return Console.ReadLine();
        }

        void IOutputInterface.OutputFunc(YoloV4Result res)
        {
            Console.WriteLine(res);
        }
        void IOutputInterface.OutputFunc(string res)
        {
            Console.WriteLine(res);
        }

        void IErrorInterface.ErrorFunc(string res)
        {
            Console.WriteLine(res);
        }
    }
}