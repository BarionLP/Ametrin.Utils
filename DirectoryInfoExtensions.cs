namespace Ametrin.Utils; 

public static class DirectoryInfoExtensions {
    public static DirectoryInfo GetCopyOfPathIfExists(this DirectoryInfo directoryInfo) {
        if(!directoryInfo.Exists) return directoryInfo;
        return directoryInfo.GetCopyOfPath().GetCopyOfPathIfExists();
    }

    public static DirectoryInfo GetCopyOfPath(this DirectoryInfo directoryInfo) {
        return new(directoryInfo.FullName + " - Copy");
    }

    //public static IEnumerable<FileInfo> EnumerateFiles(this DirectoryInfo directoryInfo) {
    //    return directoryInfo.EnumerateFiles();
    //}
}
