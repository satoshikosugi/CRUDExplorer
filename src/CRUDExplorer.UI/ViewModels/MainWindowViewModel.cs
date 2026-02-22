using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Avalonia.Platform.Storage;
using CRUDExplorer.Core.Models;

namespace CRUDExplorer.UI.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _sourcePath = string.Empty;

    [ObservableProperty]
    private string _statusMessage = "Ready";

    [ObservableProperty]
    private string _filterText = string.Empty;

    [ObservableProperty]
    private int _crudViewType = 0; // 0: TableCRUD, 1: ColumnCRUD

    [ObservableProperty]
    private ObservableCollection<object> _crudMatrixData = new();

    [ObservableProperty]
    private ObservableCollection<CrudListItem> _crudListData = new();

    [ObservableProperty]
    private object? _selectedCrudItem;

    // Commands
    [RelayCommand]
    private void OpenFolder()
    {
        // TODO: Implement folder picker dialog
        StatusMessage = "フォルダ選択...";
    }

    [RelayCommand]
    private void AnalyzeCrud()
    {
        if (string.IsNullOrEmpty(SourcePath))
        {
            StatusMessage = "ソースフォルダを選択してください";
            return;
        }

        StatusMessage = "CRUD解析を実行中...";

        // TODO: Implement CRUD analysis using SqlAnalyzer

        StatusMessage = "CRUD解析が完了しました";
    }

    [RelayCommand]
    private void LoadTableDef()
    {
        StatusMessage = "テーブル定義を読み込み中...";

        // TODO: Implement table definition loading using FileSystemHelper

        StatusMessage = "テーブル定義の読み込みが完了しました";
    }

    [RelayCommand]
    private void ShowTableCrud()
    {
        CrudViewType = 0;
        StatusMessage = "テーブルCRUDビューに切り替えました";
    }

    [RelayCommand]
    private void ShowColumnCrud()
    {
        CrudViewType = 1;
        StatusMessage = "カラムCRUDビューに切り替えました";
    }

    [RelayCommand]
    private void ShowFilter()
    {
        // TODO: Open filter dialog
        StatusMessage = "フィルタダイアログを表示";
    }

    [RelayCommand]
    private void OpenSettings()
    {
        // TODO: Open settings window
        StatusMessage = "設定画面を表示";
    }

    [RelayCommand]
    private void ShowVersion()
    {
        // TODO: Open version/license window
        StatusMessage = "バージョン情報を表示";
    }

    [RelayCommand]
    private void Exit()
    {
        // TODO: Close application
        Environment.Exit(0);
    }
}

/// <summary>
/// CRUD一覧のアイテム
/// </summary>
public class CrudListItem
{
    public string DisplayText { get; set; } = string.Empty;
    public Query? Query { get; set; }
    public string FileName { get; set; } = string.Empty;
    public int LineNo { get; set; }
}
