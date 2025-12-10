using System.Text.RegularExpressions;

namespace Ametrin.Utils;

public static class RegexExtensions
{
    extension(Regex regex)
    {
        public bool IsWhole(ReadOnlySpan<char> input)
        {
            foreach (var match in regex.EnumerateMatches(input))
            {
                if (match.Index is 0 && match.Length == input.Length)
                {
                    return true;
                }
                return false;
            }

            return false;
        }

        public bool StartsWith(ReadOnlySpan<char> input)
        {
            foreach (var match in regex.EnumerateMatches(input))
            {
                if (match.Index is 0)
                {
                    return true;
                }
                return false;
            }

            return false;
        }
    }
}
