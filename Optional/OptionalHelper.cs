namespace Ametrin.Utils.Optional;

internal static class OptionalHelper
{
    public static bool Equals<T>(IOptional<T>? left, IOptional<T>? right)
    {
        var isLeftNone = left is null || !left.HasValue;
        var isRightNone = right is null || !right.HasValue;

        if(isLeftNone)
        {
            return isRightNone;
        }
        if(isRightNone)
            return false;

        return EqualityComparer<T>.Default.Equals(left!.Value, right!.Value);
    }

    public static int CompareTo<T>(IOptional<T>? original, IOptional<T>? other)
    {
        var isOriginalNone = original is null || !original.HasValue;
        var isOtherNone = other is null || !other.HasValue;

        if(isOriginalNone)
        {
            return isOtherNone ? 0 : -1;
        }
        if(isOtherNone)
            return 1;

        return original!.CompareTo(other!.Value);
    }
}