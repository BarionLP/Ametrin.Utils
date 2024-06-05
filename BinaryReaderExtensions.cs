namespace Ametrin.Utils;

public static class BinaryReaderExtensions
{
    public static double ReadDoubleBigEndian(this BinaryReader reader) => BitConverter.ToDouble(reader.ReadBigEndian(8), 0);
    public static int ReadInt32BigEndian(this BinaryReader reader) => BitConverter.ToInt32(reader.ReadBigEndian(4), 0);
    public static uint ReadUInt32BigEndian(this BinaryReader reader) => BitConverter.ToUInt32(reader.ReadBigEndian(4), 0);

    public static byte[] ReadBigEndian(this BinaryReader reader, int count)
    {
        var bytes = reader.ReadBytes(count);
        if(BitConverter.IsLittleEndian)
            Array.Reverse(bytes);
        return bytes;
    }
}
