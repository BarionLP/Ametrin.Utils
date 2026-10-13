using System.Diagnostics;
using System.Runtime.CompilerServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Platform;
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
        public IAsyncEnumerable<FileInfo> ExtractFileInfos(CancellationToken token = default)
        {
            return Impl(args.ExtractStorageFiles(token), token);

            static async IAsyncEnumerable<FileInfo> Impl(IAsyncEnumerable<IStorageFile> files, [EnumeratorCancellation] CancellationToken token)
            {
                await foreach (var file in files.WithCancellation(token))
                {
                    string? path;
                    using (file)
                    {
                        path = file.TryGetLocalPath();
                    }
                    yield return new FileInfo(path ?? throw new PlatformNotSupportedException("files have no path on this system"));
                }
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
                            using (folder)
                            {
                                await foreach (var sub in ProcessItems(folder.GetItemsAsync(), token))
                                {
                                    yield return sub;
                                }
                            }
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

        public void SetMaxHeightToScreenHeight(float factor = 0.9f)
        {
            var screen = window.Screens.ScreenFromWindow(window) ?? window.Screens.Primary;
            if (screen is null) return;

            window.SetMaxHeightToScreenHeight(screen, factor);
        }

        public void SetMaxHeightToScreenHeight(Window owner, float factor = 0.9f)
        {
            var screen = owner.Screens.ScreenFromWindow(owner) ?? owner.Screens.Primary;
            if (screen is null) return;

            window.SetMaxHeightToScreenHeight(screen, factor);
        }

        public void SetMaxHeightToScreenHeight(Screen screen, float factor = 0.9f)
        {
            window.MaxHeight = screen.WorkingArea.Height / screen.Scaling * factor; // leave some margin
        }
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
