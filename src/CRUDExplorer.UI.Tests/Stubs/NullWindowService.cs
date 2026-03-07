using System.Threading.Tasks;
using Avalonia.Controls;
using CRUDExplorer.UI.Services;

namespace CRUDExplorer.UI.Tests.Stubs;

/// <summary>
/// テスト用 IWindowService スタブ。ダイアログを表示せず null / no-op を返す。
/// </summary>
public class NullWindowService : IWindowService
{
    public Window? MainWindow { get; set; }

    public void ShowWindow<T>() where T : Window, new() { }
    public Task ShowDialog<T>() where T : Window, new() => Task.CompletedTask;
    public void ShowWindow<T>(object dataContext) where T : Window, new() { }
    public Task ShowDialog<T>(object dataContext) where T : Window, new() => Task.CompletedTask;
    public Task<string?> ShowFolderPickerAsync(string title = "フォルダを選択") => Task.FromResult<string?>(null);
    public Task<string?> ShowFilePickerAsync(string title = "ファイルを選択", string[]? extensions = null) => Task.FromResult<string?>(null);
    public Task SetClipboardTextAsync(string text) => Task.CompletedTask;
}
