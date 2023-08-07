using Microsoft.VisualBasic.FileIO;

namespace Ametrin.Utils;

public static class FileSystemInfoExtensions {
    public static void Trash(this FileSystemInfo info, UIOption options = UIOption.OnlyErrorDialogs) {
        FileSystem.DeleteFile(info.FullName, options, RecycleOption.SendToRecycleBin);
    }

    public static string GetRelativePath(this FileSystemInfo main, DirectoryInfo relativeTo) {
        return Path.GetRelativePath(relativeTo.FullName, main.FullName);
    }
}
