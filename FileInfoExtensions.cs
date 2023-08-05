using Microsoft.VisualBasic.FileIO;
using System.IO;
using System.Security.Cryptography;

namespace Ametrin.Utils;

public static class FileInfoExtensions {    
    public static FileInfo GetCopyOfPathIfExists(this FileInfo fileInfo) {
        if(!fileInfo.Exists) return fileInfo;
        return GetCopyOfPathIfExists(GetCopyOfPath(fileInfo));
    }
    
    public static FileInfo GetCopyOfPath(this FileInfo fileInfo) {
        var newFileName = $"{fileInfo.NameWithoutExtension()} - Copy{fileInfo.Extension}";
        return new(Path.Combine(fileInfo.DirectoryName!, newFileName));
    }

    public static string NameWithoutExtension(this FileInfo fileInfo) {
        return Path.GetFileNameWithoutExtension(fileInfo.FullName);
    }

    public static void CopyTo(this FileInfo mainFile, FileInfo newFile, bool overwrite = false) {
        mainFile.CopyTo(newFile.FullName, overwrite);
    }

    public static bool CompareHash(this FileInfo self, FileInfo other) {
        return self.ComputeMd5Hash() == other.ComputeMd5Hash();
    }

    public static string ComputeSha256Hash(this FileInfo fileInfo) {
        using var sha256Hash = SHA256.Create();
        using var stream = File.OpenRead(fileInfo.FullName);

        var hash = sha256Hash.ComputeHash(stream);
        return BitConverter.ToString(hash).Replace("-", string.Empty);
    }

    static string ComputeMd5Hash(this FileInfo fileInfo) {
        using var md5Hash = MD5.Create();
        using var stream = File.OpenRead(fileInfo.FullName);

        var hash = md5Hash.ComputeHash(stream);
        return BitConverter.ToString(hash).Replace("-", string.Empty);
    }
}