using System.IO;

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
}