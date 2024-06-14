namespace Ametrin.Utils;

public static class RandomExtensions
{
    public static float NextSingle(this Random random, float minInclusive, float maxExclusive)
    {
        if(minInclusive > maxExclusive)
            throw new ArgumentException("minInclusive must be less than maxExclusive.");

        return random.NextSingle() * (maxExclusive - minInclusive) + minInclusive;
    }

    public static double NextDouble(this Random random, double minInclusive, double maxExclusive)
    {
        if(minInclusive > maxExclusive)
            throw new ArgumentException("minInclusive must be less than maxExclusive.");

        return random.NextDouble() * (maxExclusive - minInclusive) + minInclusive;
    }
}