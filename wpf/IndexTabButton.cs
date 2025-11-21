using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Ametrin.Utils.WPF;

public sealed class IndexTabButton : RadioButton
{
    public static readonly DependencyProperty CheckedColorProperty = DependencyProperty.Register("CheckedColor", typeof(Color), typeof(IndexTabButton));
    public Color CheckedColor
    {
        get => (Color) GetValue(CheckedColorProperty);
        set => SetValue(CheckedColorProperty, value);
    }

    public IndexTabButton()
    {
        Style = FindResource("IndexTabButtonStyle") as Style;
    }
}
