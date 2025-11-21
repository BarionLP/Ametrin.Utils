using System;
using System.Windows.Input;

namespace Ametrin.Utils.WPF;

public sealed class RelayCommand(Action<object?> command, Func<object?, bool>? canRun = null) : ICommand
{
    private readonly Action<object?> Command = command;
    private readonly Func<object?, bool>? CanRun = canRun;

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }

    public bool CanExecute(object? parameter = null) => CanRun is null || CanRun(parameter);

    public void Execute(object? parameter = null) => Command(parameter);
}