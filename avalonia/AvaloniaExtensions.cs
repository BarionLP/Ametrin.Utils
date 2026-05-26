using System.Diagnostics;
using System.Runtime.CompilerServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Platform.Storage;

namespace Ametrin.Utils.Avalonia;

public static class AvaloniaExtensions
{
    public static FilePickerFileType ZipFilePickerFileType => field ??= new("Zip Archive")
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
                var fileInfo = new FileInfo(file.TryGetLocalPath() ?? throw new PlatformNotSupportedException("files have no path on this system"));
                file.Dispose(); // dispose before yielding so the state machine does not have to remember
                yield return fileInfo;
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

    extension(Window window)
    {
        // PlatformImpl becomes null once a window has been closed
        public bool IsClosed => window.PlatformImpl is null;
    }

    extension(Point point)
    {
        public double SqrLength => point.X * point.X + point.Y + point.Y;
        public double Length => double.Sqrt(point.SqrLength);
    }

    extension(Visual visual)
    {
        public bool IsClientPointInside(Point point, double margin = 0)
            => point.X >= -margin && point.Y >= -margin && point.X < (visual.Bounds.Width + margin) && point.Y < (visual.Bounds.Height + margin);

        public bool IsScreenPointInside(PixelPoint point, double margin = 0)
            => visual.IsClientPointInside(visual.PointToClient(point), margin);
    }
}
