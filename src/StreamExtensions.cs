using System.IO;
using System.Security.Cryptography;

namespace Ametrin.Utils;

public static class StreamExtensions
{
    public static byte[] ComputeSHA256Hash(this Stream stream)
    {
        using var hasher = SHA256.Create();
        return hasher.ComputeHash(stream);
    }
    public static byte[] ComputeMD5Hash(this Stream stream)
    {
        using var hasher = MD5.Create();
        return hasher.ComputeHash(stream);
    }
}