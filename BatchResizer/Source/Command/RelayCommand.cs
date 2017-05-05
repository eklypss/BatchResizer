using System;
using System.Windows.Input;

namespace BatchResizer.Command
{
    public class RelayCommand : ICommand
    {
        /// <summary>
        /// Action to execute.
        /// </summary>
        private Action _action;

        /// <summary>
        ///Initializes a new instance of <see cref="RelayCommand"/> class.
        /// </summary>
        /// <param name="action"></param>
        public RelayCommand(Action action)
        {
            _action = action;
        }

        /// <summary>
        /// Determines whether <see cref="RelayCommand"/> can be executed or not.
        /// </summary>
        public bool CanExecute(object parameter)
        {
            return true;
        }

        /// <summary>
        /// Executes the <see cref="RelayCommand"/>.
        /// </summary>
        public void Execute(object parameter)
        {
            _action();
        }

        /// <summary>
        /// Event that fires when <see cref="CanExecute(object)"/> changes.
        /// </summary>
        public event EventHandler CanExecuteChanged;
    }
}