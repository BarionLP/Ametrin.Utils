using System.Text.RegularExpressions;

namespace Ametrin.Utils;

public static class RegexExtensions
{
    extension(Regex regex)
    {
        /// <summary>
        /// True if the entire <paramref name="input"/> is one match
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
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

        /// <summary>
        /// true if the first match starts at intex 0
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public bool StartsWith(ReadOnlySpan<char> input)
        {
            // TODO: exit early if the first char does not match
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
