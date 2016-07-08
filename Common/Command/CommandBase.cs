using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Common.Command
{
    public abstract class CommandBase : ICommand
    {
        public event EventHandler CanExecuteChanged;
        public abstract bool CanExecute(object parameter);
        public abstract void Execute(object parameter);
        
        public void OnCanExecuteChanged()
        {
            if (CanExecuteChanged != null) 
            {
                CanExecuteChanged(this, new EventArgs());
            }
        }

    }
}
