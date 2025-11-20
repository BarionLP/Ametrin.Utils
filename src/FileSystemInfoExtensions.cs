using System.IO;
using System.Text;

namespace Ametrin.Utils;

public static class FileSystemInfoExtensions
{
    extension(FileSystemInfo fileSystemInfo)
    {
        public string GetRelativePath(DirectoryInfo relativeTo) => Path.GetRelativePath(relativeTo.FullName, fileSystemInfo.FullName);
        public bool IsRooted() => Path.IsPathRooted(fileSystemInfo.FullName);
    }

    public static void GenerateCacheTag(DirectoryInfo cacheDirectory, string createdBy)
    {
        DirectoryNotFoundException.ExistsOrThrow(cacheDirectory);

        File.WriteAllText(cacheDirectory.File("CACHEDIR.TAG").FullName, $"""
            Signature: 8a477f597d28d172789f06886806bc55
            # This file is a cache directory tag created by {createdBy}.
            # For information about cache directory tags, see:
            #	https://bford.info/cachedir/
            """, Encoding.ASCII);
    }
}
