namespace Ametrin.Utils;

public static class FileSystemInfoExtensions {
    public static string GetRelativePath(this FileSystemInfo main, DirectoryInfo relativeTo) => Path.GetRelativePath(relativeTo.FullName, main.FullName);
}
