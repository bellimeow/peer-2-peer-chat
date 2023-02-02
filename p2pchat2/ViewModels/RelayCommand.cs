using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace p2pchat2.ViewModels
{
    public class RelayCommand : ICommand
    {
        private Action commandTask;
        public event EventHandler CanExecuteChanged;
        public RelayCommand(Action task)
        {
            commandTask = task;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public object ParameterValue
        {
            get; set;
        }

        public void Execute(object parameter)
        {
            this.ParameterValue = parameter;
            commandTask();
        }
    }
}