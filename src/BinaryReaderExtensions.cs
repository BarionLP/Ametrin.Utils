using System.IO;

namespace Ametrin.Utils;

public static class BinaryReaderExtensions
{
    extension(BinaryReader reader)
    {
        public double ReadDoubleBigEndian() => reader.ReadBigEndian(BitConverter.ToDouble, 8);
        public float ReadSingleBigEndian() => reader.ReadBigEndian(BitConverter.ToSingle, 4);

        public int ReadInt32BigEndian() => reader.ReadBigEndian(BitConverter.ToInt32, 4);
        public uint ReadUInt32BigEndian() => reader.ReadBigEndian(BitConverter.ToUInt32, 4);

        public short ReadInt16BigEndian() => reader.ReadBigEndian(BitConverter.ToInt16, 2);
        public ushort ReadUInt16BigEndian() => reader.ReadBigEndian(BitConverter.ToUInt16, 2);

        public T ReadBigEndian<T>(Func<ReadOnlySpan<byte>, T> converter, int byteSize) where T : struct
        {
            Span<byte> buffer = stackalloc byte[byteSize];
            reader.ReadBigEndian(buffer);
            return converter(buffer);
        }

        public void ReadBigEndian(Span<byte> buffer)
        {
            reader.Read(buffer);
            if (BitConverter.IsLittleEndian)
            {
                buffer.Reverse();
            }
        }
    }
}
