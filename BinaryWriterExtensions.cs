namespace Ametrin.Utils; 

public static class BinaryWriterExtensions {
    public static void WriteLittleEndian(this BinaryWriter writer, float value) => writer.WriteLittleEndian(BitConverter.GetBytes(value));
    public static void WriteLittleEndian(this BinaryWriter writer, double value) => writer.WriteLittleEndian(BitConverter.GetBytes(value));
    public static void WriteLittleEndian(this BinaryWriter writer, int value) => writer.WriteLittleEndian(BitConverter.GetBytes(value));
    public static void WriteLittleEndian(this BinaryWriter writer, uint value) => writer.WriteLittleEndian(BitConverter.GetBytes(value));
    public static void WriteLittleEndian(this BinaryWriter writer, byte[] bytes) {
        if(!BitConverter.IsLittleEndian) Array.Reverse(bytes);
        writer.Write(bytes);
    }
    public static void WriteBigEndian(this BinaryWriter writer, float value) => writer.WriteBigEndian(BitConverter.GetBytes(value));
    public static void WriteBigEndian(this BinaryWriter writer, double value) => writer.WriteBigEndian(BitConverter.GetBytes(value));
    public static void WriteBigEndian(this BinaryWriter writer, int value) => writer.WriteBigEndian(BitConverter.GetBytes(value));
    public static void WriteBigEndian(this BinaryWriter writer, uint value) => writer.WriteBigEndian(BitConverter.GetBytes(value));
    public static void WriteBigEndian(this BinaryWriter writer, byte[] bytes) {
        if(BitConverter.IsLittleEndian) Array.Reverse(bytes);
        writer.Write(bytes);
    }
}
