using System.IO.Compression;

namespace Ametrin.Utils;

public static class ZipArchiveExtensions
{
    extension(ZipArchive archive)
    {
        public ZipArchiveEntry GetOrCreateEntry(string entryName)
        => archive.GetEntry(entryName) ?? archive.CreateEntry(entryName);

        public ZipArchiveEntry OverwriteEntry(string entryName)
        {
            archive.GetEntry(entryName)?.Delete();
            return archive.CreateEntry(entryName);
        }
    }
}
