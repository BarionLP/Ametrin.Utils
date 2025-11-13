using System.Diagnostics;
using System.IO;
using Microsoft.VisualBasic.FileIO;
using SearchOption = System.IO.SearchOption;

namespace Ametrin.Utils;

public static class DirectoryInfoExtensions
{
    public static DirectoryInfo GetCopyOfPathIfExists(this DirectoryInfo directoryInfo)
    {
        return directoryInfo.Exists ? directoryInfo.GetCopyOfPath().GetCopyOfPathIfExists() : directoryInfo;
    }

    public static DirectoryInfo GetCopyOfPath(this DirectoryInfo directoryInfo)
    {
        return new(directoryInfo.FullName + " - Copy");
    }

    public static DirectoryInfo CreateIfNotExists(this DirectoryInfo directoryInfo)
    {
        if (!directoryInfo.Exists)
        {
            directoryInfo.Create();
        }
        Debug.Assert(directoryInfo.Exists);
        return directoryInfo;
    }

    public static FileInfo File(this DirectoryInfo directoryInfo, string fileName) => new(Path.Join(directoryInfo.FullName, fileName));
    public static DirectoryInfo Directory(this DirectoryInfo directoryInfo, string directoryName) => new(Path.Join(directoryInfo.FullName, directoryName));

    public static void ToRecycleBin(this DirectoryInfo info, UIOption options = UIOption.OnlyErrorDialogs)
    {
        FileSystem.DeleteDirectory(info.FullName, options, RecycleOption.SendToRecycleBin);
    }

    public static void ForeachFile(this DirectoryInfo directoryInfo, Action<FileInfo> action, IProgress<(float, string)>? progress, SearchOption searchOption = SearchOption.AllDirectories, string pattern = "*")
    {
        if (progress is null)
        {
            directoryInfo.ForeachFile(action, searchOption, pattern);
            return;
        }

        var files = directoryInfo.GetFiles(pattern, searchOption);
        float totalFiles = files.Length;
        var processed = 0;
        foreach (var file in files)
        {
            action(file);
            processed++;
            progress.Report((processed / totalFiles, file.FullName));
        }
    }

    public static void ForeachFile(this DirectoryInfo directoryInfo, Action<FileInfo> action, IProgress<float>? progress, SearchOption searchOption = SearchOption.AllDirectories, string pattern = "*")
    {
        if (progress is null)
        {
            directoryInfo.ForeachFile(action, searchOption, pattern);
            return;
        }

        var files = directoryInfo.GetFiles(pattern, searchOption);
        float totalFiles = files.Length;
        var processed = 0;
        foreach (var file in files)
        {
            action(file);
            processed++;
            progress.Report(processed / totalFiles);
        }
    }

    public static void ForeachFile(this DirectoryInfo directoryInfo, Action<FileInfo> action, SearchOption searchOption = SearchOption.AllDirectories, string pattern = "*")
    {
        foreach (var file in directoryInfo.EnumerateFiles(pattern, searchOption))
        {
            action(file);
        }
    }
}
