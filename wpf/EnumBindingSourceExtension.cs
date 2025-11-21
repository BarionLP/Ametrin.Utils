using System;
using System.Windows.Markup;

namespace Ametrin.Utils.WPF;

public sealed class EnumBindingSourceExtension(Type enumType) : MarkupExtension
{
    public Type EnumType { get; } = enumType ?? throw new ArgumentNullException(nameof(enumType));

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        return EnumType.IsEnum ? Enum.GetValues(EnumType) : throw new ArgumentException("Type must be for an Enum.");
    }
}
