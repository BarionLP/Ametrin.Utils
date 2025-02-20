using System.IO;
using System.Text;

namespace Ametrin.Utils;

public static class FileSystemInfoExtensions
{
    public static string GetRelativePath(this FileSystemInfo main, DirectoryInfo relativeTo) => Path.GetRelativePath(relativeTo.FullName, main.FullName);
    public static bool IsRooted(this FileSystemInfo fileSystemInfo) => Path.IsPathRooted(fileSystemInfo.FullName);

    public static void GenerateCacheTag(DirectoryInfo cacheDirectory, string createdBy)
    {
        ArgumentNullException.ThrowIfNull(cacheDirectory);
        if (!cacheDirectory.Exists) throw new DirectoryNotFoundException($"Could not find {cacheDirectory}");

        File.WriteAllText(cacheDirectory.File("CACHEDIR.TAG").FullName, $"""
            Signature: 8a477f597d28d172789f06886806bc55
            # This file is a cache directory tag created by {createdBy}.
            # For information about cache directory tags, see:
            #	https://bford.info/cachedir/
            """, Encoding.ASCII);
    }
}
