using System.IO;
using System.Runtime.CompilerServices;

namespace Ametrin.Guards;

public static class ExceptionGuardExtensions
{
    extension(FileNotFoundException)
    {
        [StackTraceHidden]
        public static FileInfo ExistsOrThrow(FileInfo fileInfo, [CallerArgumentExpression(nameof(fileInfo))] string paramName = "")
        {
            ArgumentNullException.ThrowIfNull(fileInfo, paramName);
            if (fileInfo.Exists) return fileInfo;
            throw new FileNotFoundException(null, fileInfo.FullName);
        }

        [StackTraceHidden]
        public static string ExistsOrThrow(string filePath, [CallerArgumentExpression(nameof(filePath))] string paramName = "")
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(filePath, paramName);
            if (File.Exists(filePath)) return filePath;
            throw new FileNotFoundException(null, filePath);
        }
    }

    extension(DirectoryNotFoundException)
    {
        [StackTraceHidden]
        public static DirectoryInfo ExistsOrThrow(DirectoryInfo directoryInfo, [CallerArgumentExpression(nameof(directoryInfo))] string paramName = "")
        {
            ArgumentNullException.ThrowIfNull(directoryInfo, paramName);
            if (directoryInfo.Exists) return directoryInfo;
            throw new DirectoryNotFoundException(directoryInfo.FullName);
        }
    }
}
