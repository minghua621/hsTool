using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Command
{
    public class ActiveDelegateCommand<TViewModel> : RelayCommand<TViewModel>
        where TViewModel : NotificationBase
    {
        protected TViewModel _viewModel;

        public ActiveDelegateCommand(TViewModel viewModel, Action<TViewModel> execute, Func<TViewModel, bool> canExecute = null)
            : base(execute, canExecute)
        {
            _viewModel = viewModel;
            _viewModel.PropertyChanged += (s, e) =>
            {
                this.OnCanExecuteChanged();
            };
        }

        public override bool CanExecute(object parameter)
        {
            return _canExecute == null ? true : _canExecute(_viewModel);
        }

        public override void Execute(object parameter)
        {
            _execute(_viewModel);
        }        
    }
}
