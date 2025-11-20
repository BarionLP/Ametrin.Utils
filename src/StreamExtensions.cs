using System.IO;
using System.Security.Cryptography;

namespace Ametrin.Utils;

public static class StreamExtensions
{
    extension(Stream stream)
    {
        public byte[] ComputeSHA256Hash()
        {
            using var hasher = SHA256.Create();
            return hasher.ComputeHash(stream);
        }

        public byte[] ComputeMD5Hash()
        {
            using var hasher = MD5.Create();
            return hasher.ComputeHash(stream);
        }
    }
}