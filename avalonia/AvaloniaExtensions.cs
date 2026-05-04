using System.Diagnostics;
using System.Runtime.CompilerServices;
using Avalonia.Input;
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

    extension(DragEventArgs args)
    {
        public async IAsyncEnumerable<FileInfo> ExtractFileInfos([EnumeratorCancellation] CancellationToken token = default)
        {
            await foreach (var file in args.ExtractStorageFiles(token))
            {
                yield return new(file.TryGetLocalPath() ?? throw new PlatformNotSupportedException("files have no path on this system"));
                file.Dispose();
            }
        }

        public IAsyncEnumerable<IStorageFile> ExtractStorageFiles(CancellationToken token = default)
        {
            if (args.DataTransfer.TryGetFiles() is { Length: > 0 } items)
            {
                return ProcessItems(items.ToAsyncEnumerable(), token);
            }

            return AsyncEnumerable.Empty<IStorageFile>();

            static async IAsyncEnumerable<IStorageFile> ProcessItems(IAsyncEnumerable<IStorageItem> items, [EnumeratorCancellation] CancellationToken token)
            {
                await foreach (var item in items)
                {
                    switch (item)
                    {
                        case IStorageFile file:
                            yield return file;
                            break;

                        case IStorageFolder folder:
                            await foreach (var sub in ProcessItems(folder.GetItemsAsync(), token))
                            {
                                yield return sub;
                            }
                            folder.Dispose();
                            break;

                        default:
                            item.Dispose();
                            throw new UnreachableException();
                    }
                }
            }
        }
    }
}
