using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prediction;
using YOLOv4MLNet.DataStructures;
using System.Windows;
using Ookii.Dialogs.Wpf;
using System.ComponentModel;
using System.Collections.Immutable;


namespace WpfApp
{
    public partial class ViewModel : INotifyPropertyChanged, IUIServices
    {

        void IErrorInterface.ErrorFunc(string s)
        {
            MessageBox.Show(s);
        }

        string IInputInterface.GetPath()
        {
            VistaFolderBrowserDialog dlg = new VistaFolderBrowserDialog();
            if (dlg.ShowDialog().HasValue)
            {
                return dlg.SelectedPath;
            }
            MessageBox.Show("Choose the Folder path!");
            return "";

        }

        void IOutputInterface.ResultFunc(YoloV4Result res, string image_name)
        {
            lock (this)
            {
                Results = results.Add(res);
                OnPropertyChanged(nameof(Results));

                if (!numberOfClasses.ContainsKey(res.Label))
                {
                    NumberOfClasses = numberOfClasses.Add(res.Label, 1);
                }

                else
                {
                    int num = numberOfClasses[res.Label];
                    NumberOfClasses = numberOfClasses.SetItem(res.Label, num + 1);
                }
                if (!imagesInFolder.Contains(image_name))
                {
                    ImagesInFolder = imagesInFolder.Add(image_name);
                    OnPropertyChanged(nameof(ImagesInFolder));
                }
                if (imagesOfClasses.ContainsKey(res.Label))
                {
                    List<string> value = imagesOfClasses[res.Label];
                    if (!value.Contains(image_name))
                    {
                        value.Add(image_name);
                    }
                    ImagesOfClasses = imagesOfClasses.SetItem(res.Label, value);
                }
                else
                {
                    List<string> value = new List<string>();
                    value.Add(image_name);
                    ImagesOfClasses = imagesOfClasses.Add(res.Label, value);
                }
            }
        }

        void IOutputInterface.OutputFunc(string res)
        {
            Progress = res;
        }

    }
}
