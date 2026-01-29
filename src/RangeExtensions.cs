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

    extension<T>(ICollection<T> source)
    {
        public Range IndexRange => ..source.Count;
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

public readonly record struct UInt32Range(uint Start, uint End)
{
    public Enumerator GetEnumerator() => new(Start, End);

    public ref struct Enumerator
    {
        // start INCLUSIVE - end EXCLUSIVE
        // we have next separatly because current cannot start as -1
        private uint _current;
        private uint _next = 0;
        private readonly uint _end;
        public readonly uint Current => _current;

        public Enumerator(uint start, uint end)
        {
            if (start > end) throw new ArgumentException();

            _current = start;
            _next = start;
            _end = end;
        }

        public bool MoveNext()
        {
            _current = _next;
            _next++;
            return _current < _end;
        }
    }
}
