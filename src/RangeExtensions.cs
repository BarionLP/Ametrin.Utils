namespace Ametrin.Utils;

public static class RangeExtensions
{
    extension(Range range)
    {
        public bool Contains(decimal value) => value >= range.Start.Value && value < range.End.Value;
        public bool Contains(double value) => value >= range.Start.Value && value < range.End.Value;
        public bool Contains(float value) => value >= range.Start.Value && value < range.End.Value;
        public bool Contains(long value) => value >= range.Start.Value && value < range.End.Value;
        public bool Contains(int value) => value >= range.Start.Value && value < range.End.Value;
        public bool Contains(short value) => value >= range.Start.Value && value < range.End.Value;

        public RangeEnumerator GetEnumerator() => new(range);
    }

    // struct or ref struct (what is the performance impact?)
    // i want this loop in yields
    // it is just syntactical sugar...
    // favor worse readability in performance critical situations?
    public struct RangeEnumerator
    {
        // start INCLUSIVE - end EXCLUSIVE
        private int _current;
        private readonly int _end;
        public readonly int Current => _current;

        public RangeEnumerator(Range range)
        {
            if (range.End.IsFromEnd)
            {
                throw new NotFiniteNumberException("Can't count to infinity!", range.End.Value);
            }

            _current = range.Start.Value - 1;
            _end = range.End.Value;
        }

        public bool MoveNext()
        {
            _current++;
            return _current < _end;
        }
    }
}
