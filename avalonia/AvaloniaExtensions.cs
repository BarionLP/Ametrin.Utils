using Avalonia.Platform.Storage;

namespace Ametrin.Utils.Avalonia;

public static class AvaloniaExtensions
{
    public static FilePickerFileType ZipFilePickerFileType { get; } = new("Zip Archive")
    {
        Patterns = ["*.zip"],
        AppleUniformTypeIdentifiers = ["public.zip-archive"],
        MimeTypes = ["application/zip"]
    };

    extension(FilePickerFileTypes)
    {
        public static FilePickerFileType Zip => ZipFilePickerFileType;
    }
}
