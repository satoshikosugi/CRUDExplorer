using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using AvaloniaEdit;
using CRUDExplorer.UI.Controls;
using CRUDExplorer.UI.ViewModels;

namespace CRUDExplorer.UI.Views;

public partial class AnalyzeQueryWindow : Window
{
    private TextEditor? _sqlEditor;
    private SqlTextColorizer? _colorizer;

    public AnalyzeQueryWindow()
    {
        InitializeComponent();
        // DataContextはWindowService経由で設定される
        DataContextChanged += OnDataContextChanged;
        Loaded += OnWindowLoaded;
    }

    private void OnWindowLoaded(object? sender, RoutedEventArgs e)
    {
        Loaded -= OnWindowLoaded;
        // ウィンドウがロードされた後、エディタとViewModelのテキストを同期する
        if (_sqlEditor != null && DataContext is AnalyzeQueryViewModel vm)
        {
            _sqlEditor.Text = vm.SqlText;
        }
    }

    private void OnDataContextChanged(object? sender, EventArgs e)
    {
        if (DataContext is not AnalyzeQueryViewModel vm) return;

        // コールバックを設定
        vm.SetCallbacks(
            showGrepWindow: (param) =>
            {
                var parts = param.Split(':', 3);
                var scope = parts.Length > 0 ? parts[0] : "all";
                var file = parts.Length > 1 ? parts[1] : string.Empty;
                var word = parts.Length > 2 ? parts[2] : string.Empty;

                var grepVm = new GrepViewModel(closeWindow: () => { });
                grepVm.SearchPattern = word;
                grepVm.CurrentFile = file;
                grepVm.SearchCurrentFile = scope == "file";
                grepVm.SearchCurrentProgram = scope == "program";
                grepVm.SearchAllFiles = scope == "all";

                var grepWin = new GrepWindow();
                grepWin.DataContext = grepVm;
                grepWin.Show(this);
            },
            showTableDefinitionWindow: (tableName) =>
            {
                var win = new TableDefinitionWindow();
                if (win.DataContext is TableDefinitionViewModel tdVm && !string.IsNullOrEmpty(tableName))
                    tdVm.SelectedTable = tableName;
                win.Show(this);
            },
            showCrudListWindow: (items, title) =>
            {
                var listVm = new GenericListViewModel(closeWindow: () => { });
                var listItems = items
                    .Select(i => new ListItemModel { DisplayText = i.DisplayText, Tag = i })
                    .ToList();
                listVm.SetItems(
                    new System.Collections.ObjectModel.ObservableCollection<ListItemModel>(listItems),
                    title);
                var win = new GenericListWindow();
                win.DataContext = listVm;
                win.Show(this);
            },
            getSelectedText: (_) => _sqlEditor?.SelectedText);

        // AvaloniaEditセットアップ
        _sqlEditor = this.FindControl<TextEditor>("SqlEditor");
        if (_sqlEditor != null)
        {
            _colorizer = new SqlTextColorizer
            {
                HighlightText1 = vm.HighlightText1,
                HighlightText2 = vm.HighlightText2,
                HighlightText3 = vm.HighlightText3
            };
            _sqlEditor.TextArea.TextView.LineTransformers.Add(_colorizer);

            // ダブルクリックでSQL識別子全体を選択（アンダースコア含む）
            _sqlEditor.TextArea.AddHandler(
                DoubleTappedEvent,
                OnSqlEditorDoubleTapped,
                RoutingStrategies.Tunnel);

            // 初期テキストを設定
            if (!string.IsNullOrEmpty(vm.SqlText))
                _sqlEditor.Text = vm.SqlText;
        }

        // ViewModelのプロパティ変更を監視
        vm.PropertyChanged += OnViewModelPropertyChanged;
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (sender is not AnalyzeQueryViewModel vm) return;

        switch (e.PropertyName)
        {
            case nameof(AnalyzeQueryViewModel.SqlText):
                if (_sqlEditor != null)
                    _sqlEditor.Text = vm.SqlText;
                break;
            case nameof(AnalyzeQueryViewModel.HighlightText1):
            case nameof(AnalyzeQueryViewModel.HighlightText2):
            case nameof(AnalyzeQueryViewModel.HighlightText3):
                UpdateHighlights(vm);
                break;
        }
    }

    private void UpdateHighlights(AnalyzeQueryViewModel vm)
    {
        if (_colorizer == null) return;
        _colorizer.HighlightText1 = vm.HighlightText1;
        _colorizer.HighlightText2 = vm.HighlightText2;
        _colorizer.HighlightText3 = vm.HighlightText3;
        _sqlEditor?.TextArea.TextView.Redraw();
    }

    private void OnSqlEditorDoubleTapped(object? sender, TappedEventArgs e)
    {
        if (_sqlEditor?.Document == null) return;

        var offset = _sqlEditor.TextArea.Caret.Offset;
        var text = _sqlEditor.Text;
        if (string.IsNullOrEmpty(text) || offset < 0 || offset >= text.Length) return;

        // SQL識別子の境界を検出（アンダースコア・英数字を含む）
        int start = offset;
        int end = offset;

        while (start > 0 && IsSqlIdentifierChar(text[start - 1]))
            start--;
        while (end < text.Length && IsSqlIdentifierChar(text[end]))
            end++;

        if (start < end)
        {
            _sqlEditor.SelectionStart = start;
            _sqlEditor.SelectionLength = end - start;
            e.Handled = true;
        }
    }

    private static bool IsSqlIdentifierChar(char c)
    {
        return char.IsLetterOrDigit(c) || c == '_';
    }

    private void OnTreeViewSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (DataContext is AnalyzeQueryViewModel vm && sender is TreeView tv)
        {
            vm.SelectedTreeNode = tv.SelectedItem as QueryTreeNode;
        }
    }
}
