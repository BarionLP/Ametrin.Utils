﻿namespace Ametrin.Utils;

public static class BinaryReaderExtensions {
    
    #region Big Endian
    public static double ReadDoubleBigEndian(this BinaryReader reader) => reader.ReadBigEndian(BitConverter.ToDouble, 8);
    public static float ReadSingleBigEndian(this BinaryReader reader) => reader.ReadBigEndian(BitConverter.ToSingle, 4);

    public static int ReadInt32BigEndian(this BinaryReader reader) => reader.ReadBigEndian(BitConverter.ToInt32, 4);
    public static uint ReadUInt32BigEndian(this BinaryReader reader) => reader.ReadBigEndian(BitConverter.ToUInt32, 4);

    public static short ReadInt16BigEndian(this BinaryReader reader) => reader.ReadBigEndian(BitConverter.ToInt16, 2);
    public static ushort ReadUInt16BigEndian(this BinaryReader reader) => reader.ReadBigEndian(BitConverter.ToUInt16, 2);

    public static T ReadBigEndian<T>(this BinaryReader reader, Converter<T> converter, int byteSize) where T : struct {
        Span<byte> buffer = stackalloc byte[byteSize];
        reader.ReadBigEndian(buffer);
        return converter(buffer);
    }

    public static void ReadBigEndian(this BinaryReader reader, Span<byte> buffer) {
        reader.Read(buffer);
        if(BitConverter.IsLittleEndian) {
            buffer.Reverse();
        }
    }
    #endregion


    #region Little Endian
    public static double ReadDoubleLittleEndian(this BinaryReader reader) => reader.ReadLittleEndian(BitConverter.ToDouble, 8);
    public static float ReadSingleLittleEndian(this BinaryReader reader) => reader.ReadLittleEndian(BitConverter.ToSingle, 4);

    public static int ReadInt32LittleEndian(this BinaryReader reader) => reader.ReadLittleEndian(BitConverter.ToInt32, 4);
    public static uint ReadUInt32LittleEndian(this BinaryReader reader) => reader.ReadLittleEndian(BitConverter.ToUInt32, 4);

    public static short ReadInt16LittleEndian(this BinaryReader reader) => reader.ReadLittleEndian(BitConverter.ToInt16, 2);
    public static ushort ReadUInt16LittleEndian(this BinaryReader reader) => reader.ReadLittleEndian(BitConverter.ToUInt16, 2);

    public static T ReadLittleEndian<T>(this BinaryReader reader, Converter<T> converter, int byteSize) where T : struct {
        Span<byte> buffer = stackalloc byte[byteSize];
        reader.ReadLittleEndian(buffer);
        return converter(buffer);
    }

    public static byte[] ReadBigEndian(this BinaryReader reader, int count) {
        var bytes = reader.ReadBytes(count);
        if(BitConverter.IsLittleEndian) Array.Reverse(bytes);
        return bytes;
    }
    #endregion
}

public delegate T Converter<out T>(ReadOnlySpan<byte> buffer);