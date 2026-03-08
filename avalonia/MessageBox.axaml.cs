using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;

namespace Ametrin.Utils.Avalonia;

public sealed partial class MessageBox : Window
{
    public MessageBox(string icon, string message, params ReadOnlySpan<object> options)
    {
        InitializeComponent();
        IconLabel.Text = icon;
        MessageLabel.Text = message;

        foreach (var option in options)
        {
            var button = new Button
            {
                Content = option,
                MinWidth = 64,
                HorizontalContentAlignment = HorizontalAlignment.Center,
            };

            button.Click += (sender, args) =>
            {
                Close(option);
            };

            ButtonPanel.Children.Add(button);
        }
    }

    public async Task<T> ShowDialog<T>(Window owner, T defaultOption)
    {
        var result = await ShowDialog<object>(owner);
        return result is T t ? t : defaultOption;
    }

    public static MessageBoxBuilder Info(string message, string title = "Info", string icon = "ℹ️")
        => Builder(message, title, icon);
    public static MessageBoxBuilder Success(string message, string title = "Success", string icon = "✅")
        => Builder(message, title, icon);
    public static MessageBoxBuilder Warning(string message, string title = "Warning", string icon = "⚠️")
        => Builder(message, title, icon);
    public static MessageBoxBuilder Error(string message, string title = "Error", string icon = "⛔")
        => Builder(message, title, icon);

    public static MessageBoxBuilder Builder(string message, string title, string icon) => new()
    {
        Title = title,
        Icon = icon,
        Message = message,
    };

}

public sealed class MessageBoxBuilder
{
    public required string Title { get; set; }
    public required string Icon { get; set; }
    public required string Message { get; set; }


    public MessageBox Build(params ReadOnlySpan<object> options) => new(Icon, Message, options)
    {
        Title = Title,
    };

    public Task<T> ShowDialog<T>(Visual owner, T defaultOption, params ReadOnlySpan<T> options)
        where T : notnull
        => ShowDialog((Window)TopLevel.GetTopLevel(owner)!, defaultOption, options);
    public Task<T> ShowDialog<T>(Window owner, T defaultOption, params ReadOnlySpan<T> options)
        where T : notnull 
        => Build([defaultOption, .. options]).ShowDialog(owner, defaultOption);

    public void Show(object option) => Build([option]).Show();
}

public enum MessageBoxResult
{
    Ok,
    Cancel,
    Yes,
    No,
    Retry,
    Overwrite,
    Ignore,
}