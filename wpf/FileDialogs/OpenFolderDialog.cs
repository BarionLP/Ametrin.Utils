using System.Collections.Generic;
using System.IO;
using System.Linq;
using Ametrin.Optional;

namespace Ametrin.Utils.WPF.FileDialogs;

public sealed class OpenFolderDialog
{
    private readonly Microsoft.Win32.OpenFolderDialog _dialog = new()
    {
        Multiselect = false,
        ValidateNames = true,
    };

    public string InitialDirectory
    {
        get => _dialog.InitialDirectory;
        init => _dialog.InitialDirectory = value;
    }

    public bool AddToRecent
    {
        get => _dialog.AddToRecent;
        init => _dialog.AddToRecent = value;
    }

    public bool Multiselect
    {
        get => _dialog.Multiselect;
        init => _dialog.Multiselect = value;
    }

    public bool ValidateNames
    {
        get => _dialog.ValidateNames;
        init => _dialog.ValidateNames = value;
    }

    public Option<DirectoryInfo> GetDirectoryInfo() => GetPath().Map(path => new DirectoryInfo(path));
    public Option<string> GetPath() => ShowDialog() ? _dialog.FolderName : default;

    public IEnumerable<DirectoryInfo> GetDirectoryInfos() => GetPaths().Select(path => new DirectoryInfo(path));
    public IEnumerable<string> GetPaths() => ShowDialog() ? _dialog.FolderNames : [];

    private bool ShowDialog() => _dialog.ShowDialog() is true;
}
