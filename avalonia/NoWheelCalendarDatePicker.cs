using Avalonia.Controls;
using Avalonia.Input;

namespace Ametrin.Utils.Avalonia;

public sealed class NoWheelCalendarDatePicker : CalendarDatePicker
{
    protected override Type StyleKeyOverride => typeof(CalendarDatePicker);
    protected override void OnPointerWheelChanged(PointerWheelEventArgs e)
    {
        // NOT calling base because it advances SelectedDate and marks the event handled.
        // Leaving it unhandled lets an outer ScrollViewer scroll
    }
}