using System.IO;
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
    public static FileInfo CopyTo(this FileInfo mainFile, DirectoryInfo target, bool overwrite = false) => mainFile.CopyTo(Path.Combine(target.FullName, mainFile.Name), overwrite);
    public static FileInfo CopyTo(this FileInfo mainFile, FileInfo newFile, bool overwrite = false) => mainFile.CopyTo(newFile.FullName, overwrite);
    public static void MoveTo(this FileInfo mainFile, FileInfo newFile, bool overwrite = false) => mainFile.MoveTo(newFile.FullName, overwrite);
    public static void Trash(this FileInfo info, UIOption options = UIOption.OnlyErrorDialogs) => FileSystem.DeleteFile(info.FullName, options, RecycleOption.SendToRecycleBin);
    public static bool CompareHash(this FileInfo self, FileInfo other) => self.ComputeMD5Hash() == other.ComputeMD5Hash();

    public static byte[] ComputeSHA256Hash(this FileInfo fileInfo)
    {
        using var stream = fileInfo.OpenRead();
        return stream.ComputeSHA256Hash();
    }

    public static byte[] ComputeMD5Hash(this FileInfo fileInfo)
    {
        using var stream = fileInfo.OpenRead();
        return stream.ComputeMD5Hash();
    }

#if NET10_0_OR_GREATER
    extension(FileInfo fileInfo)
    {
        public string NameWithoutExtension => Path.GetFileNameWithoutExtension(fileInfo.FullName);
    }
#endif
}