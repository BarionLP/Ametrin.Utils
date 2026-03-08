using System.IO;
using System.Security.Cryptography;

namespace Ametrin.Utils;

public static class StreamExtensions
{
    extension(Stream stream)
    {
        [Obsolete("use SHA256.HashData")]
        public byte[] ComputeSHA256Hash() => SHA256.HashData(stream);

        [Obsolete("use MD5.HashData")]
        public byte[] ComputeMD5Hash() => MD5.HashData(stream);
    }
}