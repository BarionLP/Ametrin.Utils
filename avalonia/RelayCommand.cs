using System.Windows.Input;

namespace Ametrin.Utils.Avalonia;

public sealed class RelayCommand<T>(Action<T> execute, Func<T, bool>? canExecute = null) : ICommand
{
    private readonly Action<T> _execute = execute;
    private readonly Func<T, bool>? _canExecute = canExecute;

    public bool CanExecute(T parameter) => _canExecute is null || _canExecute(parameter);
    public void Execute(T parameter) => _execute(parameter);

    bool ICommand.CanExecute(object? parameter) => CanExecute((T)parameter!);
    void ICommand.Execute(object? parameter) => Execute((T)parameter!);

    public event EventHandler? CanExecuteChanged;
    public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
}

public static class RelayCommand
{
    public static RelayCommand<T> Create<T>(Action<T> execute, Func<T, bool>? canExecute = null) => new(execute, canExecute);
}