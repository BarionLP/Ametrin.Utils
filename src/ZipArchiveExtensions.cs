using System.IO.Compression;

namespace Ametrin.Utils;

public static class ZipArchiveExtensions
{
    public static ZipArchiveEntry GetOrCreateEntry(this ZipArchive archive, string entryName)
        => archive.GetEntry(entryName) ?? archive.CreateEntry(entryName);

    public static ZipArchiveEntry OverwriteEntry(this ZipArchive archive, string entryName)
    {
        archive.GetEntry(entryName)?.Delete();
        return archive.CreateEntry(entryName);
    }
}
