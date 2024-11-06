using System.IO;
using System.Security.Cryptography;
using Microsoft.VisualBasic.FileIO;

namespace Ametrin.Utils;

public static class FileInfoExtensions
{
    public static FileInfo GetCopyOfPathIfExists(this FileInfo fileInfo)
        => !fileInfo.Exists ? fileInfo : GetCopyOfPathIfExists(GetCopyOfPath(fileInfo));

    public static FileInfo GetCopyOfPath(this FileInfo fileInfo)
    {
        var newFileName = $"{fileInfo.NameWithoutExtension()} - Copy{fileInfo.Extension}";
        return new(Path.Combine(fileInfo.DirectoryName!, newFileName));
    }

    public static string NameWithoutExtension(this FileInfo fileInfo) => Path.GetFileNameWithoutExtension(fileInfo.FullName);
    public static void CopyTo(this FileInfo mainFile, DirectoryInfo target, bool overwrite = false) => mainFile.CopyTo(Path.Combine(target.FullName, mainFile.Name), overwrite);
    public static void CopyTo(this FileInfo mainFile, FileInfo newFile, bool overwrite = false) => mainFile.CopyTo(newFile.FullName, overwrite);
    public static void MoveTo(this FileInfo mainFile, FileInfo newFile, bool overwrite = false) => mainFile.MoveTo(newFile.FullName, overwrite);
    public static void Trash(this FileInfo info, UIOption options = UIOption.OnlyErrorDialogs) => FileSystem.DeleteFile(info.FullName, options, RecycleOption.SendToRecycleBin);
    public static bool CompareHash(this FileInfo self, FileInfo other) => self.ComputeMd5Hash() == other.ComputeMd5Hash();

    public static byte[] ComputeSHA256Hash(this FileInfo fileInfo)
    {
        using var stream = File.OpenRead(fileInfo.FullName);
        return stream.ComputeSHA256Hash();
    }

    public static byte[] ComputeMd5Hash(this FileInfo fileInfo)
    {
        using var stream = File.OpenRead(fileInfo.FullName);
        return stream.ComputeMD5Hash();
    }
}