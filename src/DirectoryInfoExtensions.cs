using System.Diagnostics;
using System.IO;
using Microsoft.VisualBasic.FileIO;
using SearchOption = System.IO.SearchOption;

namespace Ametrin.Utils;

public static class DirectoryInfoExtensions
{
    extension(DirectoryInfo directoryInfo)
    {
        public DirectoryInfo GetCopyOfPathIfExists()
        {
            return directoryInfo.Exists ? directoryInfo.GetCopyOfPath().GetCopyOfPathIfExists() : directoryInfo;
        }

        public DirectoryInfo GetCopyOfPath()
        {
            return new(directoryInfo.FullName + " - Copy");
        }

        public DirectoryInfo CreateIfNotExists()
        {
            if (!directoryInfo.Exists)
            {
                directoryInfo.Create();
            }
            Debug.Assert(directoryInfo.Exists);
            return directoryInfo;
        }

        public FileInfo File(string fileName) => new(Path.Join(directoryInfo.FullName, fileName));
        public DirectoryInfo Directory(string directoryName) => new(Path.Join(directoryInfo.FullName, directoryName));

        public void ForeachFile(Action<FileInfo> action, IProgress<(float, string)>? progress, SearchOption searchOption = SearchOption.AllDirectories, string pattern = "*")
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

        public void ForeachFile(Action<FileInfo> action, IProgress<float>? progress, SearchOption searchOption = SearchOption.AllDirectories, string pattern = "*")
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

        public void ForeachFile(Action<FileInfo> action, SearchOption searchOption = SearchOption.AllDirectories, string pattern = "*")
        {
            foreach (var file in directoryInfo.EnumerateFiles(pattern, searchOption))
            {
                action(file);
            }
        }
        public void ToRecycleBin(UIOption options = UIOption.OnlyErrorDialogs)
        {
            FileSystem.DeleteDirectory(directoryInfo.FullName, options, RecycleOption.SendToRecycleBin);
        }
    }
}
