using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using AvaloniaEdit;
using CRUDExplorer.UI.Controls;
using CRUDExplorer.UI.ViewModels;

namespace CRUDExplorer.UI.Views;

public partial class SettingsWindow : Window
{
    private TextEditor? _previewEditor;
    private SqlTextColorizer? _previewColorizer;

    private const string SampleSql =
        "SELECT\n    t1.column1,\n    t1.column2\nFROM\n    table1 t1\nWHERE\n    t1.status = 'ACTIVE'\n    -- 条件を追加\n    AND t1.id > 0";

    public SettingsWindow()
    {
        InitializeComponent();

        var vm = new SettingsViewModel(
            closeWindow: () => Close(),
            filePicker: async () =>
            {
                var result = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
                {
                    Title = "エディタ実行ファイルを選択",
                    AllowMultiple = false
                });
                return result.Count > 0 ? result[0].Path.LocalPath : null;
            },
            showError: async (message) =>
            {
                var dialog = new Window
                {
                    Title = "エラー",
                    Width = 400,
                    Height = 150,
                    Content = new TextBlock
                    {
                        Text = message,
                        Margin = new Thickness(20),
                        TextWrapping = TextWrapping.Wrap
                    }
                };
                await dialog.ShowDialog(this);
            });

        DataContext = vm;

        // プレビュー更新イベント
        vm.PreviewChanged += UpdatePreview;

        // TabControlの選択変更でプレビュー初期化
        Opened += (_, _) =>
        {
            InitializePreview();
            UpdatePreview();
        };
    }

    private void InitializePreview()
    {
        _previewEditor = this.FindControl<TextEditor>("SqlColorPreview");
        if (_previewEditor == null) return;

        _previewColorizer = new SqlTextColorizer();
        _previewEditor.TextArea.TextView.LineTransformers.Add(_previewColorizer);
        _previewEditor.Text = SampleSql;
    }

    private void UpdatePreview()
    {
        if (_previewEditor == null || _previewColorizer == null) return;
        if (DataContext is not SettingsViewModel vm) return;

        // フォント
        try { _previewEditor.FontFamily = new FontFamily(vm.SqlEditorFontFamily); } catch { }
        _previewEditor.FontSize = vm.SqlEditorFontSize;
        _previewEditor.WordWrap = vm.SqlEditorWordWrap;
        _previewEditor.Options.IndentationSize = vm.SqlEditorTabSize;

        // 文字色・背景色
        if (Color.TryParse(vm.SqlForegroundColor, out var fg))
            _previewEditor.Foreground = new SolidColorBrush(fg);
        if (Color.TryParse(vm.SqlBackgroundColor, out var bg))
            _previewEditor.Background = new SolidColorBrush(bg);

        // シンタックスカラー
        if (Color.TryParse(vm.SqlKeywordColor, out var kw))
            _previewColorizer.KeywordBrush = new SolidColorBrush(kw);
        if (Color.TryParse(vm.SqlStringLiteralColor, out var sl))
            _previewColorizer.StringLiteralBrush = new SolidColorBrush(sl);
        if (Color.TryParse(vm.SqlCommentColor, out var cm))
            _previewColorizer.CommentBrush = new SolidColorBrush(cm);
        _previewColorizer.SubqueryBrush = _previewColorizer.KeywordBrush;

        // 再描画
        _previewEditor.TextArea.TextView.Redraw();
    }

    private async void OnColorButtonClick(object? sender, RoutedEventArgs e)
    {
        if (sender is not Button btn || DataContext is not SettingsViewModel vm) return;

        var tag = btn.Tag?.ToString() ?? "";
        var currentHex = tag switch
        {
            "Foreground" => vm.SqlForegroundColor,
            "Background" => vm.SqlBackgroundColor,
            "Keyword" => vm.SqlKeywordColor,
            "Comment" => vm.SqlCommentColor,
            "StringLiteral" => vm.SqlStringLiteralColor,
            _ => "#000000"
        };

        Color.TryParse(currentHex, out var currentColor);

        var picker = new ColorPickerWindow(currentColor);
        await picker.ShowDialog(this);

        if (picker.SelectedColor.HasValue)
        {
            var c = picker.SelectedColor.Value;
            var hex = $"#{c.R:X2}{c.G:X2}{c.B:X2}";
            switch (tag)
            {
                case "Foreground": vm.SqlForegroundColor = hex; break;
                case "Background": vm.SqlBackgroundColor = hex; break;
                case "Keyword": vm.SqlKeywordColor = hex; break;
                case "Comment": vm.SqlCommentColor = hex; break;
                case "StringLiteral": vm.SqlStringLiteralColor = hex; break;
            }
        }
    }
}
