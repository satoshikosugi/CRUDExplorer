using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using AvaloniaEdit;
using CRUDExplorer.UI.Controls;
using CRUDExplorer.UI.ViewModels;
using CRUDExplorer.Core.Models;
using CRUDExplorer.Core.Utilities;

namespace CRUDExplorer.UI.Views;

public partial class AnalyzeQueryWindow : Window
{
    private TextEditor? _sqlEditor;
    private TextEditor? _sqlPreview;
    private SqlTextColorizer? _colorizer;
    private SqlTextColorizer? _previewColorizer;
    private SearchWindow? _searchWindow;

    public AnalyzeQueryWindow()
    {
        InitializeComponent();
        DataContextChanged += OnDataContextChanged;
        Loaded += OnWindowLoaded;
        Activated += OnWindowActivated;
    }

    private void OnWindowActivated(object? sender, EventArgs e)
    {
        this.FindControl<TreeView>("QueryTreeView")?.Focus();
    }

    private void OnWindowLoaded(object? sender, RoutedEventArgs e)
    {
        Loaded -= OnWindowLoaded;
        if (DataContext is AnalyzeQueryViewModel vm)
        {
            if (_sqlEditor != null)
                _sqlEditor.Text = vm.SqlText;
            if (_sqlPreview != null)
                _sqlPreview.Text = vm.SubquerySqlText;
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
            getSelectedText: (_) => _sqlEditor?.SelectedText,
            closeWindow: () => Close(),
            launchEditor: (fileName, lineNo) =>
            {
                // 外部エディタ起動（設定に基づく）
                try
                {
                    var sourcePath = GlobalState.Instance.LastAnalysisDestPath;
                    var filePath = System.IO.Path.Combine(sourcePath, fileName);
                    if (System.IO.File.Exists(filePath))
                    {
                        var psi = new System.Diagnostics.ProcessStartInfo
                        {
                            FileName = filePath,
                            UseShellExecute = true
                        };
                        System.Diagnostics.Process.Start(psi);
                    }
                }
                catch { /* エディタ起動失敗は無視 */ }
            },
            openSearchWindow: () => OpenSearchWindow(vm),
            setClipboard: async (text) =>
            {
                var clipboard = TopLevel.GetTopLevel(this)?.Clipboard;
                if (clipboard != null)
                    await clipboard.SetTextAsync(text);
            });

        // 検索カーソル移動コールバック
        vm.SelectTextRange = (offset, length) =>
        {
            if (_sqlEditor == null) return;
            _sqlEditor.SelectionStart = offset;
            _sqlEditor.SelectionLength = length;
            _sqlEditor.TextArea.Caret.Offset = offset;
            _sqlEditor.ScrollTo(
                _sqlEditor.Document.GetLineByOffset(offset).LineNumber, 0);
        };

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
            var editorSettings = Settings.Load();
            _colorizer.ApplySettingsColors(editorSettings);
            _sqlEditor.TextArea.TextView.LineTransformers.Add(_colorizer);

            // フォント設定を反映
            _sqlEditor.FontFamily = new FontFamily(editorSettings.SqlEditorFontFamily);
            _sqlEditor.FontSize = editorSettings.SqlEditorFontSize;
            _sqlEditor.WordWrap = editorSettings.SqlEditorWordWrap;
            _sqlEditor.Options.IndentationSize = editorSettings.SqlEditorTabSize;

            // 文字色・背景色を反映
            if (Color.TryParse(editorSettings.SqlForegroundColor, out var fg))
                _sqlEditor.Foreground = new SolidColorBrush(fg);
            if (Color.TryParse(editorSettings.SqlBackgroundColor, out var bg))
                _sqlEditor.Background = new SolidColorBrush(bg);

            // カーソル位置変更で括弧対応＋CRUDリスト連動ハイライト
            _sqlEditor.TextArea.Caret.PositionChanged += OnCaretPositionChanged;

            // %N%マーカーのツールチップ表示
            _sqlEditor.TextArea.TextView.PointerMoved += OnSqlEditorPointerMoved;

            // 初期テキストを設定
            if (!string.IsNullOrEmpty(vm.SqlText))
                _sqlEditor.Text = vm.SqlText;
        }

        // サブクエリSQLプレビューセットアップ
        _sqlPreview = this.FindControl<TextEditor>("SqlPreview");
        if (_sqlPreview != null)
        {
            _previewColorizer = new SqlTextColorizer();
            var previewSettings = Settings.Load();
            _previewColorizer.ApplySettingsColors(previewSettings);
            _sqlPreview.TextArea.TextView.LineTransformers.Add(_previewColorizer);

            // フォント設定を反映
            _sqlPreview.FontFamily = new FontFamily(previewSettings.SqlEditorFontFamily);
            _sqlPreview.FontSize = previewSettings.SqlEditorFontSize;
            _sqlPreview.WordWrap = previewSettings.SqlEditorWordWrap;
            _sqlPreview.Options.IndentationSize = previewSettings.SqlEditorTabSize;

            // 文字色・背景色を反映
            if (Color.TryParse(previewSettings.SqlForegroundColor, out var pfg))
                _sqlPreview.Foreground = new SolidColorBrush(pfg);
            if (Color.TryParse(previewSettings.SqlBackgroundColor, out var pbg))
                _sqlPreview.Background = new SolidColorBrush(pbg);

            // ダブルクリックで%N%マーカーのサブクエリノードにジャンプ
            _sqlPreview.TextArea.AddHandler(
                DoubleTappedEvent,
                OnSqlPreviewDoubleTapped,
                RoutingStrategies.Tunnel);

            if (!string.IsNullOrEmpty(vm.SubquerySqlText))
                _sqlPreview.Text = vm.SubquerySqlText;
        }

        // CrudDisplayItemのIsChecked変更を監視してcolorizerを更新
        vm.TableCrudList.CollectionChanged += (_, __) =>
        {
            SubscribeCheckedChanges(vm);
            UpdateCheckedItems(vm);
        };
        vm.ColumnCrudList.CollectionChanged += (_, __) =>
        {
            SubscribeCheckedChanges(vm);
            UpdateCheckedItems(vm);
        };
        vm.OnCheckedItemsChanged = () => UpdateCheckedItems(vm);

        // ViewModelのプロパティ変更を監視
        vm.PropertyChanged += OnViewModelPropertyChanged;
    }

    /// <summary>
    /// 検索ダイアログを開く（既存窓があれば再利用）
    /// </summary>
    private void OpenSearchWindow(AnalyzeQueryViewModel vm)
    {
        if (_searchWindow != null)
        {
            _searchWindow.Activate();
            return;
        }

        _searchWindow = new SearchWindow();
        _searchWindow.DataContext = new SearchViewModel(
            closeWindow: () => _searchWindow?.Close(),
            onFindNext: (text, matchCase, wholeWord, regex) =>
                vm.ExecuteSearch(text, matchCase, wholeWord, regex, forward: true),
            onFindPrevious: (text, matchCase, wholeWord, regex) =>
                vm.ExecuteSearch(text, matchCase, wholeWord, regex, forward: false));

        // 初期検索テキストを設定（選択テキストまたはHighlight1）
        var initial = _sqlEditor?.SelectedText;
        if (string.IsNullOrEmpty(initial))
            initial = vm.HighlightText1;
        if (!string.IsNullOrEmpty(initial) && _searchWindow.DataContext is SearchViewModel svm)
            svm.SearchText = initial;

        _searchWindow.Closed += (_, _) => _searchWindow = null;
        _searchWindow.Show(this);
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
            case nameof(AnalyzeQueryViewModel.SubquerySqlText):
                if (_sqlPreview != null)
                    _sqlPreview.Text = vm.SubquerySqlText;
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
        if (_colorizer != null)
        {
            _colorizer.HighlightText1 = vm.HighlightText1;
            _colorizer.HighlightText2 = vm.HighlightText2;
            _colorizer.HighlightText3 = vm.HighlightText3;
            _sqlEditor?.TextArea.TextView.Redraw();
        }
        if (_previewColorizer != null)
        {
            _previewColorizer.HighlightText1 = vm.HighlightText1;
            _previewColorizer.HighlightText2 = vm.HighlightText2;
            _previewColorizer.HighlightText3 = vm.HighlightText3;
            _sqlPreview?.TextArea.TextView.Redraw();
        }
    }

    /// <summary>
    /// CRUDリストのチェック状態に基づいてcolorizerのテーブル/カラムリストを更新
    /// </summary>
    private void UpdateCheckedItems(AnalyzeQueryViewModel vm)
    {
        if (_colorizer == null) return;

        var checkedTables = new List<CheckedTableEntry>();
        int colorIdx = 0;
        foreach (var item in vm.TableCrudList)
        {
            if (item.IsChecked)
            {
                checkedTables.Add(new CheckedTableEntry
                {
                    TableName = item.TableName,
                    AltName = item.AltName,
                    ColorIndex = colorIdx
                });
                colorIdx++;
            }
        }
        _colorizer.CheckedTables = checkedTables;

        var checkedColumns = new List<string>();
        foreach (var item in vm.ColumnCrudList)
        {
            if (item.IsChecked && !string.IsNullOrEmpty(item.ColumnName))
                checkedColumns.Add(item.ColumnName);
        }
        _colorizer.CheckedColumns = checkedColumns;

        _sqlEditor?.TextArea.TextView.Redraw();
    }

    /// <summary>
    /// 各CrudDisplayItemのPropertyChangedを購読してチェック変更を検知
    /// </summary>
    private void SubscribeCheckedChanges(AnalyzeQueryViewModel vm)
    {
        foreach (var item in vm.TableCrudList)
        {
            item.PropertyChanged -= OnCrudItemPropertyChanged;
            item.PropertyChanged += OnCrudItemPropertyChanged;
        }
        foreach (var item in vm.ColumnCrudList)
        {
            item.PropertyChanged -= OnCrudItemPropertyChanged;
            item.PropertyChanged += OnCrudItemPropertyChanged;
        }
    }

    private void OnCrudItemPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(CrudDisplayItem.IsChecked) && DataContext is AnalyzeQueryViewModel vm)
            UpdateCheckedItems(vm);
    }

    /// <summary>
    /// カーソル位置変更時 - 括弧対応ハイライト＋CRUDリスト連動
    /// </summary>
    private void OnCaretPositionChanged(object? sender, EventArgs e)
    {
        if (_sqlEditor?.Document == null || _colorizer == null) return;

        var offset = _sqlEditor.TextArea.Caret.Offset;
        var text = _sqlEditor.Text;

        // 括弧対応ハイライト
        var (openOff, closeOff) = SqlTextColorizer.FindMatchingBrackets(text, offset);
        if (_colorizer.BracketOpenOffset != openOff || _colorizer.BracketCloseOffset != closeOff)
        {
            _colorizer.BracketOpenOffset = openOff;
            _colorizer.BracketCloseOffset = closeOff;
            _sqlEditor.TextArea.TextView.Redraw();
        }

        // カーソル位置のテーブル/カラム名でCRUDリスト連動ハイライト
        if (DataContext is AnalyzeQueryViewModel vm)
        {
            var word = GetWordAtOffset(text, offset);
            UpdateCrudListHighlight(vm, word);
            UpdateGuideText(vm, word);
        }
    }

    /// <summary>
    /// %N%マーカーのツールチップ表示（ポインタ移動時）
    /// </summary>
    private void OnSqlEditorPointerMoved(object? sender, Avalonia.Input.PointerEventArgs e)
    {
        if (_sqlEditor?.Document == null || DataContext is not AnalyzeQueryViewModel vm) return;

        var pos = e.GetPosition(_sqlEditor.TextArea.TextView);
        var textPos = _sqlEditor.TextArea.TextView.GetPosition(pos + _sqlEditor.TextArea.TextView.ScrollOffset);
        if (textPos == null)
        {
            ToolTip.SetTip(_sqlEditor, null);
            return;
        }

        var offset = _sqlEditor.Document.GetOffset(textPos.Value.Location);
        var text = _sqlEditor.Text;
        var word = GetWordAtOffset(text, offset);

        if (System.Text.RegularExpressions.Regex.IsMatch(word, @"^%\d+%$"))
        {
            var indexStr = word.Trim('%');
            var node = FindNodeBySubQueryIndex(vm.QueryTreeData, indexStr);
            if (node?.Tag is CRUDExplorer.Core.Models.Query q)
            {
                var preview = q.Arrange();
                if (preview.Length > 300)
                    preview = preview[..300] + "...";
                ToolTip.SetTip(_sqlEditor, preview);
                return;
            }
        }

        ToolTip.SetTip(_sqlEditor, null);
    }

    /// <summary>
    /// カーソル位置の単語を取得
    /// </summary>
    private static string GetWordAtOffset(string text, int offset)
    {
        if (string.IsNullOrEmpty(text) || offset < 0 || offset >= text.Length)
            return string.Empty;

        int start = offset;
        int end = offset;

        while (start > 0 && IsWordChar(text[start - 1]))
            start--;
        while (end < text.Length && IsWordChar(text[end]))
            end++;

        return start < end ? text[start..end] : string.Empty;
    }

    private static bool IsWordChar(char c)
    {
        return char.IsLetterOrDigit(c) || c == '_' || c == '.' || c == '%';
    }

    /// <summary>
    /// カーソル位置のテーブル/カラム名に対応するCRUDリスト項目を黄色ハイライト
    /// </summary>
    private static void UpdateCrudListHighlight(AnalyzeQueryViewModel vm, string word)
    {
        if (string.IsNullOrEmpty(word)) return;

        // テーブル名部分を抽出（TABLE.COLUMN形式の場合）
        var dotIdx = word.IndexOf('.');
        var tablePart = dotIdx >= 0 ? word[..dotIdx] : word;

        vm.TableName = string.Empty;
        vm.AltName = string.Empty;

        foreach (var item in vm.TableCrudList)
        {
            if (string.Equals(item.TableName, tablePart, StringComparison.OrdinalIgnoreCase)
                || string.Equals(item.AltName, tablePart, StringComparison.OrdinalIgnoreCase))
            {
                vm.TableName = item.TableName;
                vm.AltName = item.AltName;
                break;
            }
        }
    }

    /// <summary>
    /// ガイドバーにSELECT句の展開テキストを表示
    /// </summary>
    private static void UpdateGuideText(AnalyzeQueryViewModel vm, string word)
    {
        if (string.IsNullOrEmpty(word) || vm.SelectedTreeNode?.Tag is not CRUDExplorer.Core.Models.Query query)
        {
            vm.GuideText = string.Empty;
            return;
        }
        vm.GuideText = query.ExpandSelect(word);
    }

    private void OnSqlPreviewDoubleTapped(object? sender, TappedEventArgs e)
    {
        if (_sqlPreview?.Document == null) return;

        var text = _sqlPreview.Text;
        if (string.IsNullOrEmpty(text)) return;

        // クリック位置からオフセットを計算（ReadOnlyエディタではCaretが更新されないため）
        var pos = e.GetPosition(_sqlPreview.TextArea.TextView);
        var textPos = _sqlPreview.TextArea.TextView.GetPosition(
            pos + _sqlPreview.TextArea.TextView.ScrollOffset);
        if (textPos == null) return;

        var offset = _sqlPreview.Document.GetOffset(textPos.Value.Location);
        if (offset < 0 || offset >= text.Length) return;

        int start = offset;
        int end = offset;

        while (start > 0 && IsSqlIdentifierChar(text[start - 1]))
            start--;
        while (end < text.Length && IsSqlIdentifierChar(text[end]))
            end++;

        if (start < end)
        {
            var selectedWord = text[start..end];

            // %N%パターンの場合 → TreeViewのサブクエリノードにジャンプ
            if (System.Text.RegularExpressions.Regex.IsMatch(selectedWord, @"^%\d+%$"))
            {
                JumpToSubQueryNode(selectedWord);
                _sqlPreview.SelectionStart = start;
                _sqlPreview.SelectionLength = end - start;
                e.Handled = true;
            }
        }
    }

    /// <summary>
    /// %N%からTreeViewのサブクエリノードにジャンプ
    /// </summary>
    private void JumpToSubQueryNode(string subQueryRef)
    {
        var treeView = this.FindControl<TreeView>("QueryTreeView");
        if (treeView == null || DataContext is not AnalyzeQueryViewModel vm) return;

        // サブクエリインデックスを抽出
        var indexStr = subQueryRef.Trim('%');

        // TreeViewノードを再帰検索
        var targetNode = FindNodeBySubQueryIndex(vm.QueryTreeData, indexStr);
        if (targetNode != null)
        {
            vm.SelectedTreeNode = targetNode;
        }
    }

    private static QueryTreeNode? FindNodeBySubQueryIndex(
        System.Collections.ObjectModel.ObservableCollection<QueryTreeNode> nodes, string index)
    {
        foreach (var node in nodes)
        {
            if (node.Tag is CRUDExplorer.Core.Models.Query q && q.SubQueryIndex == index)
                return node;

            var found = FindNodeBySubQueryIndex(node.Children, index);
            if (found != null) return found;
        }
        return null;
    }

    private static bool IsSqlIdentifierChar(char c)
    {
        return char.IsLetterOrDigit(c) || c == '_' || c == '%';
    }

    private void OnTreeViewSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (DataContext is AnalyzeQueryViewModel vm && sender is TreeView tv)
        {
            vm.SelectedTreeNode = tv.SelectedItem as QueryTreeNode;
        }
    }

    /// <summary>
    /// TreeViewでEnterキー → SQLエディタにフォーカス移動
    /// </summary>
    private void OnTreeViewKeyDown(object? sender, Avalonia.Input.KeyEventArgs e)
    {
        if (e.Key == Avalonia.Input.Key.Enter)
        {
            _sqlEditor?.Focus();

            // CRUDリストの先頭を選択（何も選択されていない場合）
            if (DataContext is AnalyzeQueryViewModel vm)
            {
                var tableCrudGrid = this.FindControl<DataGrid>("TableCrudGrid");
                if (tableCrudGrid?.SelectedIndex < 0 && vm.TableCrudList.Count > 0)
                    tableCrudGrid.SelectedIndex = 0;
            }

            e.Handled = true;
        }
    }

    /// <summary>
    /// テーブルCRUDリスト選択時にカラムCRUDリストの対応行をハイライト
    /// </summary>
    private void OnTableCrudSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (sender is not DataGrid grid) return;
        if (grid.SelectedItem is not CrudDisplayItem selectedTable) return;
        if (DataContext is not AnalyzeQueryViewModel vm) return;

        var columnGrid = this.FindControl<DataGrid>("ColumnCrudGrid");
        if (columnGrid == null) return;

        // カラムCRUDリストの対応行を視覚的にハイライト
        // テーブル名が一致するカラムのIsCheckedを一時的に設定
        foreach (var col in vm.ColumnCrudList)
        {
            col.IsChecked = string.Equals(col.TableName, selectedTable.TableName,
                StringComparison.OrdinalIgnoreCase);
        }
    }
}
