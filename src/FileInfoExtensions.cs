using System.IO;
using Microsoft.VisualBasic.FileIO;

namespace Ametrin.Utils;

public static class FileInfoExtensions
{
    extension(FileInfo fileInfo)
    {
        public FileInfo GetCopyOfPathIfExists()
        => fileInfo.Exists ? GetCopyOfPathIfExists(GetCopyOfPath(fileInfo)) : fileInfo;

        public FileInfo GetCopyOfPath()
        {
            var newFileName = $"{fileInfo.NameWithoutExtension} - Copy{fileInfo.Extension}";
            return new(Path.Join(fileInfo.DirectoryName!, newFileName));
        }

        public string NameWithoutExtension => Path.GetFileNameWithoutExtension(fileInfo.FullName);

        public byte[] ComputeSHA256Hash()
        {
            using var stream = fileInfo.OpenRead();
            return stream.ComputeSHA256Hash();
        }

        public byte[] ComputeMD5Hash()
        {
            using var stream = fileInfo.OpenRead();
            return stream.ComputeMD5Hash();
        }
        
        public FileInfo CopyTo(DirectoryInfo target, bool overwrite = false) => fileInfo.CopyTo(Path.Join(target.FullName, fileInfo.Name), overwrite);
        public FileInfo CopyTo(FileInfo newFile, bool overwrite = false) => fileInfo.CopyTo(newFile.FullName, overwrite);
        public void MoveTo(FileInfo newFile, bool overwrite = false) => fileInfo.MoveTo(newFile.FullName, overwrite);
        public void Trash(UIOption options = UIOption.OnlyErrorDialogs) => FileSystem.DeleteFile(fileInfo.FullName, options, RecycleOption.SendToRecycleBin);
        public bool CompareHash(FileInfo other) => fileInfo.ComputeMD5Hash().SequenceEqual(other.ComputeMD5Hash());
    }

}