namespace Ametrin.Utils;

public static class RandomExtensions
{
    extension(Random random)
    {
        public float NextSingle(float minInclusive, float maxExclusive)
        {
            Guard.LessThanOrEqual(minInclusive, maxExclusive);

            return random.NextSingle() * (maxExclusive - minInclusive) + minInclusive;
        }

        public double NextDouble(double minInclusive, double maxExclusive)
        {
            Guard.LessThanOrEqual(minInclusive, maxExclusive);

            return random.NextDouble() * (maxExclusive - minInclusive) + minInclusive;
        }

        public bool Success(double chance) => random.NextDouble() < chance;
        public bool Success(float chance) => random.NextSingle() < chance;
    }
}