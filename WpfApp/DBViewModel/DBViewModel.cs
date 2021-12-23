using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Immutable;
using System.ComponentModel;

namespace WpfApp.DBVM
{
    public class DBViewModel: INotifyPropertyChanged
    {
        ImmutableList<Result> results;
        public ImmutableList<Result> Results
        {
            get => results;
            set
            {
                results = value;
                OnPropertyChanged(nameof(Results));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
