using System.Diagnostics;
using System.IO;

namespace Ametrin.Utils;

public static class BinaryWriterExtensions
{
    extension(BinaryWriter writer)
    {
        public void WriteBigEndian(float value) => writer.WriteBigEndian(value, sizeof(float), BitConverter.TryWriteBytes);
        public void WriteBigEndian(double value) => writer.WriteBigEndian(value, sizeof(double), BitConverter.TryWriteBytes);
        public void WriteBigEndian(short value) => writer.WriteBigEndian(value, sizeof(short), BitConverter.TryWriteBytes);
        public void WriteBigEndian(ushort value) => writer.WriteBigEndian(value, sizeof(ushort), BitConverter.TryWriteBytes);
        public void WriteBigEndian(int value) => writer.WriteBigEndian(value, sizeof(int), BitConverter.TryWriteBytes);
        public void WriteBigEndian(uint value) => writer.WriteBigEndian(value, sizeof(uint), BitConverter.TryWriteBytes);

        public void WriteBigEndian<T>(T value, int byteSize, Func<Span<byte>, T, bool> converter)
        {
            Span<byte> buffer = stackalloc byte[byteSize];
            var isSuccess = converter(buffer, value);
            Debug.Assert(isSuccess);
            writer.WriteBigEndian(buffer);
        }

        private void WriteBigEndian(Span<byte> buffer)
        {
            if (BitConverter.IsLittleEndian)
            {
                // for this to be public we can't operate on buffer directly
                buffer.Reverse();
            }
            writer.Write(buffer);
        }
    }
}
