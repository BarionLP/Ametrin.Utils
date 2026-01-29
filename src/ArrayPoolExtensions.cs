using System.Buffers;
using System.Runtime.InteropServices;

namespace Ametrin.Utils;

public static class ArrayPoolExtensions
{
    extension<T>(ArrayPool<T> arrayPool)
    {
        public ArrayPoolHandle<T> RentHandle(int requestedLength, bool clearOnReturn = false)
            => new(arrayPool, requestedLength, clearOnReturn);
    }

    // problem: when the struct gets copied and one gets disposed the other version is still valid
    // a class would solve this but introduce another allocation (except .NET 10 stack allocates the instance where possible)
    public ref struct ArrayPoolHandle<T>(ArrayPool<T> arrayPool, int requestedLength, bool clearOnReturn) : IDisposable
    {
        public ArrayPool<T> ArrayPool { get; } = Guard.ThrowIfNull(arrayPool);
        public int RequestedLength { get; } = requestedLength;
        public T[] Array { get; private set; } = arrayPool.Rent(requestedLength);
        private bool isDisposed = false;
        private readonly bool clearOnReturn = clearOnReturn;

        public readonly Span<T> AsSpan()
        {
            ObjectDisposedException.ThrowIf(isDisposed, typeof(ArrayPoolHandle<T>));
            return Array.AsSpan(0, RequestedLength);
        }

        public readonly ref T GetPinnableReference() => ref MemoryMarshal.GetReference(AsSpan());

        public static implicit operator Span<T>(ArrayPoolHandle<T> handle) => handle.AsSpan();

        public void Dispose()
        {
            if (isDisposed) return;

            ArrayPool.Return(Array, clearArray: clearOnReturn);
            Array = null!;
            isDisposed = true;
        }
    }
}