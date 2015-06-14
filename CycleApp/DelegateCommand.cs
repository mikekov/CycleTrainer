using System;
using System.Windows.Input;

namespace CycleApp
{
	public class DelegateCommand : ICommand
	{
		public event EventHandler CanExecuteChanged;
		public delegate bool CanExecuteCallback();

		public DelegateCommand(Action execute, CanExecuteCallback can_execute)
			: this(s => { execute(); }, s => { return can_execute != null ? can_execute() : true; })
		{}

		public DelegateCommand(Action<object> execute) : this(execute, null)
		{}

		public DelegateCommand(Action<object> execute, Predicate<object> can_execute)
		{
			_execute = execute;
			_can_execute = can_execute;
		}

		public bool CanExecute(object parameter)
		{
			if (_can_execute == null)
				return true;

			return _can_execute(parameter);
		}

		public void Execute(object parameter)
		{
			_execute(parameter);
		}

		public void RaiseCanExecuteChanged()
		{
			if (CanExecuteChanged != null)
				CanExecuteChanged(this, EventArgs.Empty);
		}

		readonly Predicate<object> _can_execute;
		readonly Action<object> _execute;
	}
}
