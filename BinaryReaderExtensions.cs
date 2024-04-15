namespace Ametrin.Utils;

public static class BinaryReaderExtensions {
    public static int ReadInt32BigEndian(this BinaryReader br) {
        var bytes = br.ReadBytes(4);
        if(BitConverter.IsLittleEndian) Array.Reverse(bytes);
        return BitConverter.ToInt32(bytes, 0);
    }
}
