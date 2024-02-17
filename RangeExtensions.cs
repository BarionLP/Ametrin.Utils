namespace Ametrin.Utils;

public static class RangeExtensions {
    public static bool Contains(this Range range, float value) =>  value >= range.Start.Value && value < range.End.Value;
    public static bool Contains(this Range range, int value) =>  value >= range.Start.Value && value < range.End.Value;
    public static bool Contains(this Range range, short value) =>  value >= range.Start.Value && value < range.End.Value;

    public static RangeEnumerator GetEnumerator(this Range range) => new(range);

    // struct or ref struct (what is the performance impact?)
    // i want this loop in yields
    // it is just syntactical sugar...
    // favor worse readability in performance critical situations?
    public struct RangeEnumerator {
        // start INCLUSIVE - end EXCLUSIVE
        private int _current;
        private readonly int _end;
        public readonly int Current => _current;

        public RangeEnumerator(Range range) {
            if(range.End.IsFromEnd) throw new NotFiniteNumberException("Can't count to infinity!", range.End.Value);

            _current = range.Start.Value - 1;
            _end = range.End.Value;
        }

        public bool MoveNext() {
            _current++;
            return _current < _end;
        }
    }
}
