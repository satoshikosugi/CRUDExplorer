using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Platform.Storage;

namespace CRUDExplorer.UI.Services;

/// <summary>
/// ウィンドウ操作のためのサービス。ViewModelからウィンドウを開く/閉じるに使用。
/// </summary>
public interface IWindowService
{
    /// <summary>
    /// メインウィンドウへの参照
    /// </summary>
    Window? MainWindow { get; set; }

    /// <summary>
    /// 子ウィンドウを表示（非モーダル）
    /// </summary>
    void ShowWindow<T>() where T : Window, new();

    /// <summary>
    /// 子ウィンドウをダイアログとして表示（モーダル）
    /// </summary>
    Task ShowDialog<T>() where T : Window, new();

    /// <summary>
    /// DataContext付きで子ウィンドウを表示
    /// </summary>
    void ShowWindow<T>(object dataContext) where T : Window, new();

    /// <summary>
    /// DataContext付きでダイアログ表示
    /// </summary>
    Task ShowDialog<T>(object dataContext) where T : Window, new();

    /// <summary>
    /// フォルダ選択ダイアログを表示
    /// </summary>
    Task<string?> ShowFolderPickerAsync(string title = "フォルダを選択");

    /// <summary>
    /// ファイル選択ダイアログを表示
    /// </summary>
    Task<string?> ShowFilePickerAsync(string title = "ファイルを選択", string[]? extensions = null);
}

public class WindowService : IWindowService
{
    public Window? MainWindow { get; set; }

    public void ShowWindow<T>() where T : Window, new()
    {
        var window = new T();
        window.Show(MainWindow!);
    }

    public async Task ShowDialog<T>() where T : Window, new()
    {
        var window = new T();
        await window.ShowDialog(MainWindow!);
    }

    public void ShowWindow<T>(object dataContext) where T : Window, new()
    {
        var window = new T { DataContext = dataContext };
        window.Show(MainWindow!);
    }

    public async Task ShowDialog<T>(object dataContext) where T : Window, new()
    {
        var window = new T { DataContext = dataContext };
        await window.ShowDialog(MainWindow!);
    }

    public async Task<string?> ShowFolderPickerAsync(string title = "フォルダを選択")
    {
        if (MainWindow == null) return null;
        var result = await MainWindow.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
        {
            Title = title,
            AllowMultiple = false
        });
        return result.Count > 0 ? result[0].Path.LocalPath : null;
    }

    public async Task<string?> ShowFilePickerAsync(string title = "ファイルを選択", string[]? extensions = null)
    {
        if (MainWindow == null) return null;
        var patterns = extensions?.Select(e => "*." + e.TrimStart('.')).ToList();
        var result = await MainWindow.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = title,
            AllowMultiple = false,
            FileTypeFilter = patterns != null
                ? new[] { new FilePickerFileType(title) { Patterns = patterns } }
                : null
        });
        return result.Count > 0 ? result[0].Path.LocalPath : null;
    }
}
