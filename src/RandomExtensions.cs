using Ametrin.Guards;

namespace Ametrin.Utils;

public static class RandomExtensions
{
    public static float NextSingle(this Random random, float minInclusive, float maxExclusive)
    {
        Guard.LessThanOrEqual(minInclusive, maxExclusive);

        return random.NextSingle() * (maxExclusive - minInclusive) + minInclusive;
    }

    public static double NextDouble(this Random random, double minInclusive, double maxExclusive)
    {
        Guard.LessThanOrEqual(minInclusive, maxExclusive);

        return random.NextDouble() * (maxExclusive - minInclusive) + minInclusive;
    }
}