using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace p2pchat2.ViewModels
{
    class HistoryViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private string chatHistory;

        private void OnPropertyChanged([CallerMemberName] string name = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
            Trace.WriteLine("onpropertychanged {0}");
        }

        public HistoryViewModel() { }


        public string ChatHistory
        {
            get { return chatHistory; }
            set { chatHistory = value; OnPropertyChanged("ChatHistory"); }
        }
    }
}
